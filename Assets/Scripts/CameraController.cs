using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CameraController {
    Camera camera;

    [Header("Board to Bottle")]
    [SerializeField]
    float btbMoveTime;

    [SerializeField]
    Vector3 boardFrontPos;
    [SerializeField]
    Vector3 boardFrontRot;
    [SerializeField]
    int boardFrontFieldOfView;

    [SerializeField]
    Vector3 bottleFrontPos;
    [SerializeField]
    Vector3 bottleFrontRot;
    [SerializeField]
    int bottleFrontFieldOfView;

    Transform corkTF;
    Vector3   cameraAndCorkDistance;

    // 遮蔽物非表示
    List<Renderer> hitRendereres = new List<Renderer>();
    List<Renderer> hitRendereresPrev;

    public void Start() {
        this.camera = Camera.main;

        this.camera.transform.position    = this.boardFrontPos;
        this.camera.transform.eulerAngles = this.boardFrontRot;
        this.camera.fieldOfView           = this.boardFrontFieldOfView;
    }

    public void UpdatePos() {
        this.camera.transform.position = this.corkTF.position - this.cameraAndCorkDistance;
    }

    public void UpdateRenderer() {
        // 遮蔽物を取り除く
        this.hitRendereresPrev = new List<Renderer>(this.hitRendereres);
        this.hitRendereres.Clear();

        Vector3 dir = this.corkTF.position - this.camera.transform.position;
        RaycastHit[] hits = Physics.RaycastAll(this.camera.transform.position, dir, 1f);
        foreach (RaycastHit hit in hits)
        {
            // 被写体、グラウンドであれば無視
            if (
                hit.collider.gameObject == this.corkTF.gameObject ||
                hit.collider.gameObject.layer == 8
            ) continue;

            // 遮蔽物のRendererを無効
            Renderer renderer = hit.collider.gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                this.hitRendereres.Add(renderer);
                renderer.enabled = false;
            }
        }

        // 差集合(PrevHit - Hit = HitにないPrevHit)
        foreach (Renderer renderer in this.hitRendereresPrev.Except<Renderer>(this.hitRendereres))
            if (renderer != null) renderer.enabled = true;
    }

    public void ChaseToCork(GameObject cork) {
        this.corkTF = cork.transform;
        this.cameraAndCorkDistance = this.corkTF.position - this.camera.transform.position;
    }

    public IEnumerator BoardToBottle() {
        float timer = 0;
        while (true) {
            timer += Time.deltaTime;

            // カメラ移動
            var t = timer / this.btbMoveTime;
            this.camera.transform.position    = Vector3.Lerp(this.boardFrontPos, this.bottleFrontPos, t);
            this.camera.transform.eulerAngles = Vector3.Lerp(this.boardFrontRot, this.bottleFrontRot, t);
            this.camera.fieldOfView           = Mathf.Lerp(this.boardFrontFieldOfView, this.bottleFrontFieldOfView, t);

            if (t >= 1.0f) break;
            yield return null;
        }

        // 補正
        this.camera.transform.position    = this.bottleFrontPos;
        this.camera.transform.eulerAngles = this.bottleFrontRot;
        this.camera.fieldOfView           = this.bottleFrontFieldOfView;

        MainGameManager.Phase = Phase.Countdown;
    }

    public IEnumerator ToRankingMove() {
        float timer = 0f;
        var pos = this.camera.transform.position;
        var x = this.camera.transform.position.x;
        while (true) {
            timer += Time.deltaTime;
            pos.x = Mathf.Lerp(x, x - 0.5f, timer / 1f);
            this.camera.transform.position = pos;
            if (timer / 1f >= 1f) break;
            yield return null;
        }
        MainGameManager.Phase = Phase.LoadWait;
    }
}
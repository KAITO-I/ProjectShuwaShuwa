using System;
using System.Collections;
using UnityEngine;

class Bottle : MonoBehaviour {
    GameObject animationObject; 

    [Header("Animation")]
    [SerializeField]
    float deceleration;

    Animator animator;
    float    speed;

    [Header("Init")]
    [SerializeField]
    float initPosTime;

    public void Init() {
        this.animationObject = this.gameObject.transform.Find("Animator").gameObject;

        this.animator = this.animationObject.GetComponent<Animator>();
        this.speed = 0.0f;
    }

    public void Updating() {
        this.speed -= Time.deltaTime * deceleration;
        if (this.speed < 0f) this.speed = 0f;

        this.animator.SetFloat("speed", this.speed);
    }

    public void Shake() {
        this.speed = 1f;
    }

    public IEnumerator InitPos() {
        this.animator.enabled = false;
        var timer = 0f;
        var pos   = this.animationObject.transform.localPosition;
        while (true) {
            timer += Time.deltaTime;
            this.animationObject.transform.localPosition = Vector3.Lerp(pos, Vector3.zero, timer / this.initPosTime);
            if (timer / this.initPosTime >= 1f) break;
            yield return null;
        }
        MainGameManager.Phase = Phase.ChangeAngleII;
    }

    public void Rotate(float x, float z) {
        this.gameObject.transform.Rotate(x, 0f, z);
    }

    public void ResetRotation() {
        this.gameObject.transform.eulerAngles = Vector3.zero;
    }

    public Vector3 GetTransformUp() {
        return this.gameObject.transform.up;
    }
}
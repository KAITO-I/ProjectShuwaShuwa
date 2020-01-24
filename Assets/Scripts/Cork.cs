using System;
using UnityEngine;

[Serializable]
class Cork
{
    [SerializeField]
    GameObject cork;

    Rigidbody rb;

    bool    shooted;
    float   distance    = 0f;
    Vector3 oldFramePos;

    //==============================
    // スタート
    //==============================
    public void Start()
    {
        this.rb = this.cork.GetComponent<Rigidbody>();

        this.shooted     = false;
        this.oldFramePos = this.cork.transform.position;
    }

    //==============================
    // アップデート
    //==============================
    public void Update()
    {
        if (!this.shooted) return;

        // 移動距離
        var distance = this.cork.transform.position - this.oldFramePos;
        this.distance += distance.z;

        // カメラ
        var cameraPos = Camera.main.transform.position;
        cameraPos.x += distance.x;
        cameraPos.y += distance.y;
        cameraPos.z += distance.z;
        Camera.main.transform.position = cameraPos;

        this.oldFramePos = this.cork.transform.position;
    }

    //==============================
    // コルクを放つ
    //==============================
    public void Shoot(float power)
    {
        this.shooted = true;

        this.rb.AddForce(this.cork.transform.up * power * 0.025f, ForceMode.Impulse);
        this.rb.useGravity = true;
    }
}
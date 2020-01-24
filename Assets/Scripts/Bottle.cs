using System;
using UnityEngine;

[Serializable]
class Bottle
{
    GameManager game;
    Joycon joycon;

    [SerializeField]
    GameObject bottle;

    [Header("Shake")]
    [SerializeField]
    float shakeSpeed;
    [SerializeField]
    float shakePositionRange;

    float shakePositionY;
    float defaultPositionY;
    float maxShakePositionY;
    float minShakePositionY;
    float shakePositionYAddDirection;

    [Header("Ramble")]
    [SerializeField]
    float minAmp;
    [SerializeField]
    float maxAmp;
    [SerializeField]
    float rambleTime;
    [SerializeField]
    float waitingMultipleAmp;

    float amp;
    float rambleTimer = 0f;

    [Header("Random Ramble")]
    [SerializeField]
    float maxInterval;

    [Header("Exprode")]
    [SerializeField]
    GameObject boomParticle;

    float randomRambleTimer;

    //==============================
    // スタート
    //==============================
    public void Start(GameManager game, Joycon joycon)
    {
        this.game = game;
        this.joycon = joycon;

        this.shakePositionY = this.defaultPositionY = this.bottle.transform.position.y;
        this.maxShakePositionY = this.shakePositionY + this.shakePositionRange;
        this.minShakePositionY = this.shakePositionY - this.shakePositionRange;
        this.shakePositionYAddDirection = 1f;

        this.amp = this.minAmp;

        this.randomRambleTimer = UnityEngine.Random.value * this.maxInterval;
    }

    //==============================
    // アップデート
    //==============================
    public void Update()
    {
        // ランダム振動生成
        this.randomRambleTimer -= Time.deltaTime;
        if (this.randomRambleTimer < 0f)
        {
            this.randomRambleTimer = UnityEngine.Random.value * this.maxInterval;
            this.rambleTimer = this.rambleTime * this.waitingMultipleAmp;
        }

        // 振動
        this.rambleTimer -= Time.deltaTime;
        if (this.rambleTimer < 0f) this.rambleTimer = 0f;

        this.joycon.SetRumble(160f, 320f, Mathf.Lerp(this.minAmp, this.amp, this.rambleTimer / this.rambleTime), 15);
    }

    //==============================
    // 上下運動
    //==============================
    public void Shake()
    {
        var pos = this.bottle.transform.position;
        pos.y = (this.shakePositionY += this.shakeSpeed * this.shakePositionYAddDirection * Time.deltaTime);
        this.bottle.transform.position = pos;

        if (pos.y > this.maxShakePositionY || pos.y < this.minShakePositionY) this.shakePositionYAddDirection *= -1f;
    }

    //==============================
    // 位置リセット
    //==============================
    public void PosReset()
    {
        var pos = this.bottle.transform.position;
        pos.y = this.shakePositionY = this.defaultPositionY;
        this.bottle.transform.position = pos;
    }

    //==============================
    // 振動させる
    //==============================
    public void Rumble(float ampPower)
    {
        this.amp = Mathf.Lerp(this.minAmp, this.maxAmp, ampPower);
        this.rambleTimer = this.rambleTime;
    }

    //==============================
    // 爆発させる
    //==============================
    public void Boom()
    {
        this.game.InstanceParticle(this.boomParticle, this.bottle.transform);
        this.bottle.SetActive(false);
    }

    //==============================
    // 角度を設定する
    //==============================
    public void SetEularAngle(Quaternion qua)
    {
        this.bottle.transform.rotation = qua;
    }
}

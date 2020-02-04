using System;
using System.Collections;
using UnityEngine;

[Serializable]
class PlayerController
{
    Joycon joycon;

    [Header("Bottle")]
    [SerializeField]
    Bottle bottle;
    [SerializeField]
    Cork cork;

    // 汎用計測タイマー
    [Header("Timer")]
    [SerializeField]
    float keepTime;

    float timer = 0f;

    [Header("Shake Power")]
    [SerializeField]
    float powerLimit;
    [SerializeField]
    float[] powerStageLine;
    [SerializeField]
    int[] powerStageAddPower;

    float power = 0f;
    float warningPercent;

    bool setedLowPassFilter = false;
    float lowPassFilterX = 0f;
    float lowPassFilterZ = 0f;

    [Header("Rumble")]
    [SerializeField]
    float minRumbleAmp;
    [SerializeField]
    float maxRumbleAmp;
    [SerializeField]
    float rumbleTime;
    [SerializeField]
    float mostMinInterval;
    [SerializeField]
    float maxInterval;
    [SerializeField]
    float waitingMultipleAmp;

    float rumbleAmp;
    float rumbleTimer;
    float randomRumbleTimer;

    [Header("Shooted")]
    [SerializeField]
    int shootedRumbleTime;

    Vector3 defCorkPos;
    float[] oldMag = { 0f, 0f, 0f, 0f, 0f };

    public int ResultDistance { get; private set; }

    public void Start(Joycon joycon) {
        this.joycon = joycon;
        this.bottle.Init();
        this.cork.Init();

        this.rumbleAmp = this.minRumbleAmp;
        this.rumbleTimer = this.rumbleTime;
        this.randomRumbleTimer = MainGameManager.GetRandomFloat(0.0f, this.maxInterval);
    }

    public void Update() {
        switch (MainGameManager.Phase) {
            case Phase.HowToPlayI:
                // 連続して押し続けたら次のフェーズへ
                if (this.joycon.GetButton(Joycon.Button.SL) && this.joycon.GetButton(Joycon.Button.SR))
                {
                    this.timer += Time.deltaTime;

                    // SHAKE表示
                    if (this.timer >= this.keepTime)
                    {
                        this.timer = 0f;
                        MainGameManager.Phase = Phase.HowToPlayII1;
                        break;
                    }
                }
                else this.timer = 0f;
                break;

            case Phase.HowToPlayII1:
                // SL、SRを離すとリセット
                if (!(this.joycon.GetButton(Joycon.Button.SL) && this.joycon.GetButton(Joycon.Button.SR))) {
                    this.timer = 0f;
                    MainGameManager.Phase = Phase.HowToPlayI;
                }
                break;

            case Phase.HowToPlayII2:
                // SL、SRを離すとリセット
                if (!(this.joycon.GetButton(Joycon.Button.SL) && this.joycon.GetButton(Joycon.Button.SR)))
                {
                    this.timer = 0f;
                    MainGameManager.Phase = Phase.HowToPlayI;
                    MainGameManager.SetMoreShakeAlertDisplayPercent(0f);
                    break;
                }

                // 必要時間振り続けたらゲーム開始
                if (Mathf.Abs(this.joycon.GetGyro().z) >= this.powerStageLine[0])
                {
                    this.joycon.SetRumble(160f, 320f, 0.5f, 15);

                    this.timer += Time.deltaTime;
                    if (this.timer >= this.keepTime / 2f)
                    {
                        this.timer = 0f;
                        MainGameManager.Phase = Phase.MoveCamera;
                        SoundManager.PlaySE(SEID.Okay, 1.0f);
                    }
                }
                else
                {
                    this.timer -= Time.deltaTime;
                    if (this.timer <= -this.keepTime / 2f) this.timer = -this.keepTime / 2f;
                }

                // 弱いと正しく遊べないので「大きく振ろう」と表示
                if (this.timer <= 0f) MainGameManager.SetMoreShakeAlertDisplayPercent(Mathf.Abs(this.timer) / (this.keepTime / 2f));
                break;

            case Phase.ShakeI:
            case Phase.ShakeII:
                // SL、SRを押してる間だけ振る
                if (this.joycon.GetButton(Joycon.Button.SL) && this.joycon.GetButton(Joycon.Button.SR))
                {
                    // 握ったらRELEASEキャンセル
                    if (MainGameManager.Phase == Phase.ShakeI) MainGameManager.Phase = Phase.ShakeII; // ShakeIをIIにするための行
                    this.timer = 0f;

                    // パワー計算
                    var power = 0;
                    var gyro  = this.joycon.GetGyro();
                    for (int i = 0; i < this.powerStageLine.Length; i++)
                        if (gyro.z > this.powerStageLine[i]) power += this.powerStageAddPower[i];

                    if (power != 0) {
                        this.power += power;

                        // 50%突破
                        if (this.power / this.powerLimit >= 0.5f)
                        {
                            MainGameManager.ShowReleaseBoard();
                            MainGameManager.ShowCautionSign();
                        }

                        // 80%突破
                        if (this.power / this.powerLimit >= 0.8f) MainGameManager.ShowDangerSign();

                        // 暴発
                        if (this.power >= this.powerLimit) {
                            MainGameManager.Phase = Phase.Explode;
                            break;
                        }

                        // 振動値設定
                        this.rumbleAmp = Mathf.Lerp(this.minRumbleAmp, this.maxRumbleAmp, this.power / this.powerLimit);
                        this.rumbleTimer = this.rumbleTime;

                        this.bottle.Shake();
                    }
                } else if (MainGameManager.Phase == Phase.ShakeII) {
                    // RELEASE判断 (ShakeIはゲーム開始後にまだ握ってないため実行させない)
                    MainGameManager.ShowReleaseBoard();

                    // 上を向いているかどうか
                    var accelShakeII = this.joycon.GetAccel();
                    var x     = 0.8f  < accelShakeII.x;
                    var y     = -0.1f < accelShakeII.y || accelShakeII.y < 0.1f;
                    var z     = -0.1f < accelShakeII.z || accelShakeII.z < 0.1f;
                    if (x && y && z) {
                        this.timer += Time.deltaTime;
                        if (this.timer >= this.keepTime) {
                            MainGameManager.Phase = Phase.ChangeAngleI;
                            this.timer = 0f;
                        }
                    } else
                    {
                        // 経過時間リセット
                        MainGameManager.Phase = Phase.ShakeII;
                        this.timer = 0f;
                    }
                }

                this.bottle.Updating();
                UpdateRumble();
                break;

            case Phase.ChangeAngleII:
                var gyroChangeAngleII = this.joycon.GetGyro();

                // 角度リセット
                if (this.joycon.GetButtonDown(Joycon.Button.SHOULDER_2)) this.bottle.ResetRotation();

                // ジャイロで角度変更
                this.bottle.Rotate(
                    Mathf.Abs(gyroChangeAngleII.y) > 0.1f ? -gyroChangeAngleII.y * 2f : 0f, 
                    Mathf.Abs(gyroChangeAngleII.z) > 0.1f ? -gyroChangeAngleII.z * 2f : 0f
                );

                // 倒したら放つ
                if (this.joycon.GetStick()[1] >= 0.5f) {
                    this.cork.Shoot(this.bottle.GetTransformUp(), this.power);
                    MainGameManager.GetCamera().ChaseToCork(this.cork.gameObject);
                    MainGameManager.Phase = Phase.Shooted;
                    SoundManager.PlaySE(SEID.Shoot, 1.0f);
                    this.joycon.SetRumble(160f, 320f, 1.0f, this.shootedRumbleTime);
                    this.defCorkPos = this.cork.transform.position;
                    break;
                }
                UpdateRumble();
                break;

            case Phase.HittedFirst:
                for (int i = 0; i < 5; i++) {
                    if (Mathf.Approximately(this.oldMag[i], 0f))
                    {
                        this.oldMag[i] = this.cork.Rigidbody.velocity.magnitude;
                        break;
                    }

                    // 全部値が埋まってたら一つずつずらして平均化
                    for (int j = 0; j < 5 - 1; j++) this.oldMag[j] = this.oldMag[j + 1];
                    this.oldMag[4] = this.cork.Rigidbody.velocity.magnitude;

                    var sum = 0f;
                    for (int j = 0; j < 5; j++) sum += this.oldMag[j];

                    if (sum / 5f < 0.01f) {
                        this.ResultDistance = (int)Vector3.Distance(this.cork.transform.position, this.defCorkPos);
                        MainGameManager.Phase = Phase.Result;
                    }
                }
                break;

            case Phase.LoadWait:
                if (this.joycon.GetButtonDown(Joycon.Button.STICK)) MainGameManager.Phase = Phase.Loading;
                break;
        }
    }

    public void HideBottle()
    {
        this.bottle.gameObject.SetActive(false);
    }

    public float GetKeepTimePercent() {
        return this.timer / this.keepTime;
    }

    public IEnumerator BottlePosInit() {
        return this.bottle.InitPos();
    }

    void UpdateRumble() {
        // 振動生成
        this.randomRumbleTimer -= Time.deltaTime;
        if (this.randomRumbleTimer < 0f) {
            this.randomRumbleTimer = MainGameManager.GetRandomFloat(0.0f, Mathf.Lerp(this.maxInterval, this.mostMinInterval, this.power / this.powerLimit));

            // 通常の振動時間にwaitingMultipleAmpをかけて、通常より短い時間
            this.rumbleTimer = this.rumbleTime * this.waitingMultipleAmp;
        }

        // 振動
        this.rumbleTimer -= Time.deltaTime;
        if (this.rumbleTimer <= 0f) this.rumbleTimer = 0f;

        this.joycon.SetRumble(160f, 320f, Mathf.Lerp(this.minRumbleAmp, this.rumbleAmp, this.rumbleTimer / this.rumbleTime), 15);
    }
}

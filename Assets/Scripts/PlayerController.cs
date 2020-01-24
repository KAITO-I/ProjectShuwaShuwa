using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==============================
// プレイヤー操作クラス
//==============================
[Serializable]
class PlayerController
{
    GameManager game;
    Joycon      joycon;

    [Header("Bottle")]
    public Bottle bottle;
    public Cork   cork;

    [Header("Shake Power")]
    [SerializeField]
    float powerLimit;
    [SerializeField]
    float[] powerStageLine;
    [SerializeField]
    float[] powerStageAddPower;

    float power = 0f;

    float resetAccelTimer;

    public void Init(GameManager game, Joycon joycon) {
        this.game   = game;
        this.joycon = joycon;

        this.bottle.Start(game, joycon);
        this.cork.Start();
    }

    public void Update() {
        switch (this.game.phase)
        {
            case Phase.HowToPlay:
                if (this.joycon.GetButton(Joycon.Button.SL) && this.joycon.GetButton(Joycon.Button.SR))
                    this.game.phase = Phase.Countdown;
                break;

            case Phase.Countdown:
                if (!(this.joycon.GetButton(Joycon.Button.SL) && this.joycon.GetButton(Joycon.Button.SR)))
                    this.game.phase = Phase.HowToPlay;
                break;

            case Phase.Shake:
                if (!(this.joycon.GetButton(Joycon.Button.SL) && this.joycon.GetButton(Joycon.Button.SR)))
                {
                    this.game.phase = Phase.ResetAccel;
                    this.bottle.PosReset();
                    this.resetAccelTimer = 0f;
                    return;
                }

                if (this.power > this.powerLimit)
                {
                    this.game.phase = Phase.Boom;
                    this.bottle.Boom();
                    return;
                }

                // パワー計算
                var power = 0f;
                var gyro = this.joycon.GetGyro();
                for (int i = 0; i < this.powerStageLine.Length; i++)
                {
                    var stageLine  = this.powerStageLine[i];
                    var stagePower = this.powerStageAddPower[i];
                    if (Math.Abs(gyro.z) > stageLine) power += stagePower;
                }

                if (power != 0)
                {
                    this.power += power;
                    this.bottle.Shake();
                    this.bottle.Rumble(Mathf.Lerp(0.05f, 0.75f, this.power / this.powerLimit));
                }

                // ボトルを更新
                this.bottle.Update();

                Debug.Log(this.power);

                break;

            case Phase.ResetAccel:
                if (this.joycon.GetButton(Joycon.Button.SL) && this.joycon.GetButton(Joycon.Button.SR))
                {
                    this.game.phase = Phase.Shake;
                    break;
                }

                this.bottle.Update();

                var accel = this.joycon.GetAccel();

                if (0.8f < accel.x) this.resetAccelTimer += Time.deltaTime;
                else                this.resetAccelTimer = 0f;

                if (this.resetAccelTimer > 3f) this.game.phase = Phase.Shoot;

                break;

            case Phase.Shoot:
                // 角度調整
                var vec = this.joycon.GetVector();
                this.bottle.SetEularAngle(this.joycon.GetVector());
                break;
        }
    }
}

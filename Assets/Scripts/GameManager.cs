using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//==============================
// フェーズ
//==============================
enum Phase
{
    HowToPlay,  // 操作説明
    Countdown,  // カウントダウン
    Shake,      // 振る
    Boom,       // ボトル爆発
    ResetAccel, // ジョイコンの加速度リセット
    Shoot,      // 放つ
    Measurement // 計測
}

class GameManager : MonoBehaviour
{
    //==============================
    // static
    //==============================
    static GameManager instance;

    //==============================
    // フェーズ設定
    //==============================
    static Phase phase
    {
        get { return phase; }
        set
        {
            if (phase == value) return;
            phase = value;
            switch (phase)
            {
                case Phase.HowToPlay:
                    instance.StopCoroutine(countdown);
                    break;

                case Phase.Countdown:
                    countdown = instance.StartCoroutine(instance.Countdown());
                    break;
            }
        }
    }

    static Coroutine countdown;

    //==============================
    // 中央に文字設定
    //==============================
    public static void ShowTextToCenter(string text)
    {
        instance.centerText.text = text;
    }

    //==============================
    // ゲーム中のテキストを画像で表示
    //==============================
    public static void SetMessageInGame(Image image)
    {

    }

    //==============================
    // instance
    //==============================
    [SerializeField]
    PlayerController playerController;

    [SerializeField]
    Text centerText;

    // カウントダウン
    [Header("Countdown")]
    [SerializeField]
    string startText;

    void Start()
    {
        instance = this;

        // プレイヤーコントローラー設定
        var joycon = JoyconManager.Instance.j.Find(c => c.isLeft);
        this.playerController.Init(this, joycon);

        GameManager.phase = Phase.HowToPlay;
        
    }

    void Update()
    {
        this.playerController.Update();
    }

    //==============================
    // カウントダウン
    //==============================
    IEnumerator Countdown()
    {
        int timer = 3;
        while (true)
        {
            this.centerText.text = timer.ToString();

            yield return new WaitForSeconds(1f);

            timer--;
            if (timer == 0) break;
        }

        GameManager.phase = Phase.Shake;
        this.centerText.text = this.startText;

        yield return new WaitForSeconds(1f);

        this.centerText.text = "";
    }

    public void InstanceParticle(GameObject particle, Transform spawnTF) {
        Instantiate(particle, spawnTF);
    }
}
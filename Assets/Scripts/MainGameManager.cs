using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//==============================
// フェーズ
//==============================
public enum Phase
{
    Title,         // タイトル
    HowToPlayI,    // 操作説明(HOLD)
    HowToPlayII1,  // 操作説明(SHAKE表示中)
    HowToPlayII2,  // 操作説明(SHAKE表示)
    MoveCamera,    // カメラ動かす
    Countdown,     // カウントダウン
    ShakeI,        // 握る前
    ShakeII,       // 握る後
    Explode,       // ボトル爆発
    ChangeAngleI,  // 角度調整(フェーズ切り替わり)
    ChangeAngleII, // 角度調整
    Shooted,       // 放つ
    HittedFirst,   // 着弾
    Result,       // 結果表示
    LoadWait,
    Loading
}

public class MainGameManager : MonoBehaviour
{
    //==============================
    // static
    //==============================
    public static MainGameManager Instance { get; private set; }

    static Phase phase;
    static Coroutine showShakeCoroutine = null;
    public static Phase Phase {
        get { return phase; }
        set {
            if (phase != value) {
                phase = value;

                switch (phase) {
                    case Phase.HowToPlayI:
                        if (showShakeCoroutine != null)
                        {
                            Instance.StopCoroutine(showShakeCoroutine);
                            showShakeCoroutine = null;
                        }
                        break;

                    case Phase.HowToPlayII1:
                        if (showShakeCoroutine == null) showShakeCoroutine = Instance.StartCoroutine(Instance.board.ShowShake());
                        break;

                    case Phase.MoveCamera:
                        Instance.moreShakeAlert.gameObject.SetActive(false);
                        Instance.StartCoroutine(Instance.camera.BoardToBottle());
                        break;

                    case Phase.Countdown:
                        Instance.StartCoroutine(Instance.Countdown());
                        SoundManager.PlaySE(SEID.Countdown, 1.0f);
                        break;

                    case Phase.Explode:
                        Instance.StartCoroutine(Instance.BottleExprode());
                        break;

                    case Phase.ChangeAngleI:
                        Instance.StartCoroutine(Instance.player.BottlePosInit());
                        Instance.StartCoroutine(Instance.board.ShowShoot());
                        break;

                    case Phase.Result:
                        SoundManager.PlayBGM(BGMID.Result);
                        int rank = Instance.ranking.SetRecord(Instance.player.ResultDistance);
                        Instance.ranking.Show(rank);
                        Instance.StartCoroutine(Instance.camera.ToRankingMove());
                        break;

                    case Phase.Loading:
                        Instance.StartCoroutine(SceneLoader.Load());
                        break;
                }
            }
        }
    }

    public static void SetMoreShakeAlertDisplayPercent(float t) {
        var pos = Instance.moreShakeAlert.rectTransform.localPosition;
        pos.y = Mathf.Lerp(Instance.offScreenY, 0f, t);
        Instance.moreShakeAlert.rectTransform.localPosition = pos;
    }

    public static float GetRandomFloat(float min, float max)
    {
        return Instance.RandomFloat(min, max);
    }

    public static void ShowReleaseBoard() {
        if (Instance.board.ShowedReleasedImage) return;
        Instance.StartCoroutine(Instance.board.ShowRelease());
    }

    public static void ShowCautionSign() {
        if (Instance.showedCaution) return;
        Instance.showedCaution = true;
        Instance.ShowInfoSign(Instance.caution);
        SoundManager.PlaySE(SEID.ShowSign, 1.0f);
    }

    public static void ShowDangerSign() {
        if (Instance.showedWarning) return;
        Instance.showedWarning = true;
        Instance.ShowInfoSign(Instance.warning);
        SoundManager.PlaySE(SEID.ShowSign, 1.0f);
    }

    public static CameraController GetCamera() {
        return Instance.camera;
    }

    //==============================
    // instance
    //==============================
    [SerializeField]
    PlayerController player;

    [SerializeField]
    CameraController camera;

    [SerializeField]
    Board board;

    [Header("How to Play")]
    [SerializeField]
    Image moreShakeAlert;
    [SerializeField]
    float offScreenY;

    [Header("Countdown")]
    [SerializeField]
    TextMeshProUGUI countdown;

    [Header("Caution / Warning")]
    [SerializeField]
    Image sign;
    [SerializeField]
    Sprite caution;
    [SerializeField]
    Sprite warning;

    [Header("Explode")]
    [SerializeField]
    ParticleSystem explodeParticle;

    Animation signAnimation;
    bool showedCaution = false;
    bool showedWarning = false;

    [Header("Ranking")]
    [SerializeField]
    Ranking ranking;

    void Start()
    {
        if (Instance != null)
        {
            phase = Phase.HowToPlayI;
            return;
        }

        Instance = this;

        phase = Phase.HowToPlayI;

        this.camera.Start();
        this.player.Start(JoyconManager.Instance.j.Find(c => c.isLeft));
        this.board.Start();

        this.countdown.gameObject.SetActive(false);

        this.signAnimation = this.sign.GetComponent<Animation>();
    }

    void Update() {
        this.player.Update();

        switch (Phase) {
            case Phase.HowToPlayI:
                this.board.SetHoldToShakeArrowPercent(this.player.GetKeepTimePercent());
                break;

            case Phase.ShakeII:
                this.board.SetStandKeepingArrowPercent(this.player.GetKeepTimePercent());
                break;

            case Phase.Shooted:
            case Phase.HittedFirst:
                this.camera.UpdatePos();
                this.camera.UpdateRenderer();
                break;

            case Phase.Result:
                this.camera.UpdateRenderer();
                break;
        }
    }

    IEnumerator Countdown() {
        this.countdown.gameObject.SetActive(true);

        var timer = 3;
        while (timer > 0) {
            this.countdown.text = timer.ToString();
            yield return new WaitForSeconds(1f);
            timer--;
        }
        Phase = Phase.ShakeI;
        this.countdown.text = "SHAKE!";

        SoundManager.PlayBGM(BGMID.InGame);
        yield return new WaitForSeconds(1f);
        this.countdown.gameObject.SetActive(false);
    }

    float RandomFloat(float min, float max) {
        return UnityEngine.Random.Range(min, max);
    }

    void ShowInfoSign(Sprite sprite) {
        this.sign.sprite = sprite;
        this.signAnimation.Play();
    }

    IEnumerator BottleExprode()
    {
        this.player.HideBottle();
        this.explodeParticle.Play();
        yield return new WaitForSeconds(1.0f);
        this.countdown.gameObject.SetActive(true);
        this.countdown.text = "GAME OVER";
        Phase = Phase.LoadWait;
    }
}

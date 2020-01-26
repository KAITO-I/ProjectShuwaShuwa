using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public enum MessageType {
    None = -1,
    Shake,
    Shake50POver,
    Shake75POver
}

//==============================
// フェーズ
//==============================
enum Phase
{
    HowToPlay,  // 操作説明
    Countdown,  // カウントダウン
    Shake,      // 振る
    Explode,    // ボトル爆発
    ResetAccel, // ジョイコンの加速度リセット
    Shoot,      // 放つ
    Measurement // 計測
}

public class MainGameManager : MonoBehaviour
{
    //==============================
    // static
    //==============================
    public static MainGameManager instance { get; set; }

    //==============================
    // instance
    //==============================
    [SerializeField]
    PlayerController player;

    

    [SerializeField]
    Sprite[] messages;

    void Start()
    {
        instance = this;
    }

    void Update() {

    }
}

using System.Collections;

using UnityEngine;
using UnityEngine.UI;

public enum MessageType {
    None = -1,
    Shake,
    Shake50POver,
    Shake75POver
}

public class MainGameManager : MonoBehaviour
{
    //==============================
    // static
    //==============================
    public static MainGameManager instance { get; set; }

    //==============================
    // メッセージ設定
    //==============================
    public static void SetLeftMessage(MessageType type){
        if ((int)type == -1) instance.leftMessage.gameObject.SetActive(false);
        else
        {
            instance.leftMessage.gameObject.SetActive(true);
            instance.leftMessage.sprite = instance.messages[(int)type];
        }
    }

    public static void SetRightMessage(MessageType type) {
        if ((int)type == -1) instance.rightMessage.gameObject.SetActive(false);
        else
        {
            instance.rightMessage.gameObject.SetActive(true);
            instance.rightMessage.sprite = instance.messages[(int)type];
        }
    }

    //==============================
    // instance
    //==============================
    [SerializeField]
    PlayerController player;

    [SerializeField]
    Image leftMessage;
    [SerializeField]
    Image rightMessage;

    [SerializeField]
    Sprite[] messages;

    void Start()
    {
        instance = this;
    }

    void Update() {

    }
}

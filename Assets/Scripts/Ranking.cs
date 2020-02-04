using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ranking : MonoBehaviour
{
    TextMeshProUGUI[] rankingText = new TextMeshProUGUI[5];
    Animation[]       rankingAnim = new Animation[5];

    TextMeshProUGUI norankText;
    Animation       norankAnim;

    void Awake() {
        transform.gameObject.SetActive(false);
        for (int i = 1; i <= 5; i++)
        {
            var tf = transform.Find(i.ToString());
            rankingText[i - 1] = tf.GetComponent<TextMeshProUGUI>();
            rankingAnim[i - 1] = tf.GetComponent<Animation>();
        }

        var norankTF = transform.Find("-1");
        this.norankText = norankTF.GetComponent<TextMeshProUGUI>();
        this.norankAnim = norankTF.GetComponent<Animation>();
    }

    public void Show() {
        transform.gameObject.SetActive(true);
        for (int i = 0; i < 5; i++)
        {
            var score = PlayerPrefs.GetInt(i.ToString(), 0);
            rankingText[i].text = (score > 0 ? score.ToString() : " ") + " m";
        }
        this.norankText.text = " ";
    }

    public void Show(int animationRank) {
        transform.gameObject.SetActive(true);
        for (int i = 0; i < 5; i++)
            rankingText[i].text = PlayerPrefs.GetInt(i.ToString(), 0) + " m";
        if (animationRank != -1)
        {
            rankingAnim[animationRank].Play();
            norankText.text = " ";
        }
        else norankAnim.Play();
    }

    public void Hide() {
        transform.gameObject.SetActive(false);
    }

    //==============================
    // [返り値]
    // int : 順位
    //==============================
    public int SetRecord(int record) {
        for (int i = 0; i < 5; i++) {
            // ランキングの数値があり、ランキングの数値より記録が低ければ次のループへ
            if (record < PlayerPrefs.GetInt(i.ToString(), 0)) continue;

            // 記録更新
            // 下全て書き換え
            for (int j = 4; j > i; j--)
                PlayerPrefs.SetInt(j.ToString(), PlayerPrefs.GetInt((j - 1).ToString(), 0));

            // 更新
            PlayerPrefs.SetInt(i.ToString(), record);

            return i;
        }
        this.norankText.text = record.ToString() + " m";
        return -1;
    }
}

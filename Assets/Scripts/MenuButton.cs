using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    int selected;

    Button[] button;   

    void Start()
    {
        this.selected = 0;

        this.button = new Button[]
        {
            new Button(transform.Find("Start")),
            new Button(transform.Find("Ranking")),
            new Button(transform.Find("End")),
        };

        this.button[0].Select(true);
    }

    public void Up()
    {
        if (this.selected == 0) return;

        // 項目を上に移動
        this.selected--;
        for (int i = 0; i < 3; i++) this.button[i].Select(i == this.selected);
    }

    public void Down()
    {
        if (this.selected == 2) return;

        // 項目を下に移動
        this.selected++;
        for (int i = 0; i < 3; i++) this.button[i].Select(i == this.selected);
    }

    public void Selected()
    {
        SoundManager.PlaySE(SEID.Select, 1f);
        this.button[selected].Selected();
    }

    public void Execute()
    {
        switch (this.selected)
        {
            case 0:
                // ゲーム開始
                MainGameManager.GameStart();
                SoundManager.StopBGM();
                break;

            case 1:
                // 何も実行しない
                break;

            case 2:
                // ゲーム終了
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPaused = true;
#endif
#if UNITY_STANDALONE
                Application.Quit();
#endif
                break;
        }
    }
}

class Button
{
    Image[] bottleImage;

    public Button(Transform tf)
    {
        this.bottleImage = new Image[] {
            tf.Find("Default").GetComponent<Image>(),
            tf.Find("Select").GetComponent<Image>(),
            tf.Find("Selected").GetComponent<Image>()
        };

        this.bottleImage[0].gameObject.SetActive(true);
        this.bottleImage[1].gameObject.SetActive(false);
        this.bottleImage[2].gameObject.SetActive(false);
    }

    public void Select(bool select)
    {
        this.bottleImage[1].gameObject.SetActive(select);
    }

    public void Selected()
    {
        this.bottleImage[2].gameObject.SetActive(true);
    }
}
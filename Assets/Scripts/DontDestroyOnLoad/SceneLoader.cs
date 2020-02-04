using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    static SceneLoader Instance;
    public static bool IsLoading { get; private set; }

    public static void Load() {
        Instance.StartCoroutine(Instance.LoadTitle());
    }

    Image image;

    void Awake() {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);

        this.image = transform.Find("Image").GetComponent<Image>();
    }

    IEnumerator LoadTitle() {
        IsLoading = true;

        float timer = 0f;
        var color = this.image.color;
        while (timer < 1f) {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, timer);
            this.image.color = color;
            yield return null;
        }

        SoundManager.StopBGM();

        // 読み込み
        AsyncOperation load = SceneManager.LoadSceneAsync(0);
        load.allowSceneActivation = false;

        while (load.progress < 0.9f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        load.allowSceneActivation = true;

        timer = 0f;
        while (timer < 1f) {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, timer);
            this.image.color = color;
            yield return null;
        }

        IsLoading = false;
        MainGameManager.Phase = Phase.HowToPlayI;
    }
}

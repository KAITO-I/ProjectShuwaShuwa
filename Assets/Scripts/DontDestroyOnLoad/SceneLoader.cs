using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    static SceneLoader Instance;
    public static bool IsLoading { get; private set; }

    public static IEnumerator Load() {
        return Instance.LoadTitle();
    }

    Image image;

    void Awake() {
        if (Instance != null) return;

        Instance = this;
        this.image = transform.Find("Image").GetComponent<Image>();
    }

    IEnumerator LoadTitle() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        yield return null;
        /*
        IsLoading = true;

        float timer = 0f;
        var color = this.image.color;
        while (timer / 3f < 1f) {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, timer / 3f);
            this.image.color = color;
            yield return null;
        }

        SoundManager.StopBGM();

        // 読み込み破棄
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(0));

        // 読み込み
        AsyncOperation load = SceneManager.LoadSceneAsync(0);
        load.allowSceneActivation = false;
        while (load.progress < 0.9f) yield return null;

        timer = 0f;
        while (timer / 3f < 1f) {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, timer / 3f);
            this.image.color = color;
            yield return null;
        }

        load.allowSceneActivation = true;
        IsLoading = false;
        */
    }
}

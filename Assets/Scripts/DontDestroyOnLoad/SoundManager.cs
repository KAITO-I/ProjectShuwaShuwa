using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BGMID {
    Title,
    InGame,
    Result
}

public enum SEID {
    Select,
    Okay,
    Countdown,
    Shake,
    ShowSign,
    Exprode,
    Shoot,
    CorkFirstHitGround,
    CorkBound
}

public class SoundManager : MonoBehaviour
{
    static SoundManager Instance;

    public static void PlayBGM(BGMID id) {
        Instance.Play(id);
    }

    public static void StopBGM()
    {
        Instance.StopBackGroundMusic();
    }

    public static void PlaySE(SEID id, float vol) {
        Instance.Play(id, vol);
    }

    public static void EndPlaySE(SEManager se) {
        Instance.EndPlaySoundEffect(se);
    }

    // 楽曲情報
    [SerializeField]
    AudioClip[] bgm;

    [SerializeField]
    AudioClip[] se;

    // 再生ソース
    [SerializeField]
    int simultaneousPlayableSounds;

    AudioSource bgmSource;
    List<SEManager> unplayedSESources;
    List<SEManager> playedSESources;

    void Awake() {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);

        this.bgmSource = transform.Find("BGMPlayer").GetComponent<AudioSource>();

        this.unplayedSESources = new List<SEManager>();
        this.playedSESources = new List<SEManager>();

        var sePlayer = transform.Find("SEPlayer");
        for (int i = 0; i < this.simultaneousPlayableSounds; i++) {
            GameObject seSource = new GameObject("SE");
            seSource.transform.parent = sePlayer;
            
            var source = seSource.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop        = false;

            var manager = seSource.AddComponent<SEManager>();
            manager.Init(source);

            this.unplayedSESources.Add(manager);
        }
    }

    void Play(BGMID id) {
        this.bgmSource.clip = this.bgm[(int)id];
        this.bgmSource.Play();
    }

    void StopBackGroundMusic() {
        this.bgmSource.Stop();
    }

    void Play(SEID id, float vol) {
        if (this.unplayedSESources.Count == 0) return;

        var seManager = this.unplayedSESources[0];
        seManager.Play(this.se[(int)id], vol);

        this.unplayedSESources.Remove(seManager);
        this.playedSESources.Add(seManager);
    }

    void EndPlaySoundEffect(SEManager se) {
        this.playedSESources.Remove(se);
        this.unplayedSESources.Add(se);
    }
}

public class SEManager : MonoBehaviour
{
    AudioSource seSource;

    public void Init(AudioSource source) {
        this.seSource = source;
    }

    public void Play(AudioClip clip, float vol) {
        this.seSource.clip = clip;
        this.seSource.volume = vol;
        this.seSource.Play();
        StartCoroutine(EndPlay(clip.length));
    }

    IEnumerator EndPlay(float length) {
        yield return new WaitForSeconds(length + 0.01f);
        SoundManager.EndPlaySE(this);
    }
}
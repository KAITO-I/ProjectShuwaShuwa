using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

enum BoardContentsType {
    HoldAndShake,
    Release
}


[Serializable]
class Board {
    [SerializeField]
    float changeTime;

    [Header("Hold and Shake")]
    [SerializeField]
    Image hasImage;
    [SerializeField]
    float holdFillAmount;
    [SerializeField]
    float andFillAmount;

    [Header("Release")]
    [SerializeField]
    Image releaseImage;
    [SerializeField]
    Image keepingArrowImage;

    public bool ShowedReleasedImage { get; private set; }

    [Header("Shoot")]
    [SerializeField]
    Image shootImage;

    public void Start() {
        this.hasImage.fillAmount = this.holdFillAmount;

        this.ShowedReleasedImage = false;
    }

    public void SetHoldToShakeArrowPercent(float t) {
        this.hasImage.fillAmount = Mathf.Lerp(this.holdFillAmount, this.andFillAmount, t);
    }

    public IEnumerator ShowShake() {
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;

            // 動作
            this.hasImage.fillAmount = Mathf.Lerp(this.andFillAmount, 1f, timer / this.changeTime);

            if (timer / this.changeTime >= 1.0f) break;
            yield return null;
        }
        MainGameManager.Phase = Phase.HowToPlayII2;
    }

    public IEnumerator ShowRelease() {
        this.ShowedReleasedImage = true;

        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;
            this.releaseImage.fillAmount = Mathf.Lerp(0f, 1f, timer / this.changeTime);
            if (timer / this.changeTime >= 1.0f) break;
            yield return null;
        }
    }

    public void SetStandKeepingArrowPercent(float t) {
        this.keepingArrowImage.fillAmount = Mathf.Lerp(0f, 1f, t);
    }

    public IEnumerator ShowShoot() {
        float timer = 0f;
        while (true) {
            timer += Time.deltaTime;
            this.shootImage.fillAmount = Mathf.Lerp(0f, 1f, timer / this.changeTime);
            if (timer / this.changeTime >= 1.0f) break;
            yield return null;
        }
        // フェーズを変えるのはボトル側で行う(ボトル側の操作ができてからフェーズを変えなければならないため)
    }
}
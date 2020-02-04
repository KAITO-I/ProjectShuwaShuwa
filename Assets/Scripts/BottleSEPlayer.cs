using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleSEPlayer : MonoBehaviour
{
    public void PlaySE() {
        SoundManager.PlaySE(SEID.Shake, 0.5f);
    }
}

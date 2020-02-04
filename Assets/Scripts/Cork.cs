using System;
using UnityEngine;

class Cork : MonoBehaviour
{
    [SerializeField]
    float multiplePower;

    public Rigidbody Rigidbody { get; private set;}
    bool firstGroundHit = true;

    public void Init() {
        this.Rigidbody = transform.GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision) {
        if (!this.firstGroundHit)
        {
            MainGameManager.Phase = Phase.HittedFirst;
            SoundManager.PlaySE(SEID.CorkBound, 0.5f);
            return;
        }
        this.firstGroundHit = false;
        SoundManager.PlaySE(SEID.CorkFirstHitGround, 1.0f);
    }

    public void Shoot(Vector3 up, float power)
    {
        this.Rigidbody.AddForce(up * power * this.multiplePower, ForceMode.Impulse);
        this.Rigidbody.useGravity = true;
    }
}
using UnityEngine;
using System.Collections;

public class PoppingChampagne : MonoBehaviour
{

    public GameObject champagneFX;
    public GameObject cork;
    public ParticleSystem corkSpray;

    private bool bottleActive = false;


    void Start()
    {

        champagneFX.SetActive(false);
        corkSpray.Stop();

        cork.GetComponent<Rigidbody>().useGravity = false;
        cork.GetComponent<Rigidbody>().detectCollisions = false;

        cork.transform.localPosition = new Vector3(0, 0, 0.0947f);
        cork.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

    }


    void Update()
    {

        if (Input.GetButtonDown("Fire1"))
        {


            if (bottleActive == false)
            {


                champagneFX.SetActive(false);
                corkSpray.Stop();
                cork.GetComponent<Rigidbody>().useGravity = false;
                cork.GetComponent<Rigidbody>().detectCollisions = false;
                cork.transform.localPosition = new Vector3(0, 0, 0.0947f);
                cork.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

                StartCoroutine("PopChampagne");

                cork.GetComponent<Rigidbody>().useGravity = true;
                cork.GetComponent<Rigidbody>().detectCollisions = true;               

                Rigidbody rb = cork.GetComponent<Rigidbody>();

                var locVel = transform.InverseTransformDirection(rb.velocity);
                locVel.z = 3;
                rb.velocity = transform.TransformDirection(locVel);

                
            }
            

        }

        // Reset effect

        if (Input.GetButtonDown("Fire2"))
        {

            if (bottleActive == false)
            {
                champagneFX.SetActive(false);
                cork.transform.localPosition = new Vector3(0, 0, 0.0947f);
                cork.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                cork.GetComponent<Rigidbody>().useGravity = false;
                cork.GetComponent<Rigidbody>().detectCollisions = false;
            }

        }

    }


    IEnumerator PopChampagne()
    {

        bottleActive = true;

        champagneFX.SetActive(true);

        corkSpray.Play();

        yield return new WaitForSeconds(4.0f);

        bottleActive = false;
        corkSpray.Stop();

    }

}

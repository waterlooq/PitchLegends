using LightningPoly.FootballEssentials3D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BallScript : MonoBehaviour
{

    public Rigidbody rb;
    public Pass pass;
    public bool shooting;

    private void OnCollisionEnter(Collision other)
    {
        // Stop ball immediately if it touches the teammate
        if (other.gameObject.tag == "Teammate")
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            Debug.Log("✅ Ball reached teammate!");
        }

        if (other.gameObject.tag == "Defender" && shooting == false)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            pass.FailedPass();
        }



    }

    private void Update()
    {
        if (shooting == false) { 
        if (pass.passFailed == true)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}

}

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
    public bool corner;
    public AudioSource postSound;
    public AudioSource kickSound;

    private void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.tag == "Defender" && shooting == false)
        {
            pass.kickSound.Play();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            pass.FailedPass();
        }

        if (other.gameObject.tag == "woodwork")
        {
            postSound.Play();
        }

        if (other.gameObject.tag == "goalie")
        {
            kickSound.Play();
        }
    }

    private void Update()
    {
        if (shooting == false && corner == false)
        {
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

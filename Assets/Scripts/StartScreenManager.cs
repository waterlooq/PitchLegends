using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour
{

    public GameObject homeScreen;
    public GameObject controlScreen;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void Controls()
    {
        homeScreen.SetActive(false);
        controlScreen.SetActive(true);
    }

    public void Back()
    {
        homeScreen.SetActive(true);
        controlScreen.SetActive(false);
    }

}

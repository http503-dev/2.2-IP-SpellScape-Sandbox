using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    public Database database;

    public Canvas signUpCanvas;
    public Canvas signInCanvas;

    private void Awake()
    {
        Database database = FindObjectOfType<Database>();

        if (signUpCanvas.gameObject.activeSelf)
        {
            Time.timeScale = 0;
            signUpCanvas.gameObject.SetActive(true);
            signInCanvas.gameObject.SetActive(false);
        }
    }

    public void StartGame()
    {
        Debug.Log("starting game");
        Time.timeScale = 1;
        signUpCanvas.gameObject.SetActive(false);
        signInCanvas.gameObject.SetActive(false);
        Debug.Log("started game");
    }

    public void ToSignIn()
    {
        signUpCanvas.gameObject.SetActive(false);
        signInCanvas.gameObject.SetActive(true);
    }

    public void ToSignUp()
    {
        signUpCanvas.gameObject.SetActive(true);
        signInCanvas.gameObject.SetActive(false);
    }
}

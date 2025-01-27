using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller : MonoBehaviour
{
    public Database database;

    public GameObject signUpCanvas;
    public GameObject signInCanvas;
    public GameObject mainMenu;

    public static Controller Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Database database = FindObjectOfType<Database>();

        if (signUpCanvas.gameObject.activeSelf)
        {
            Time.timeScale = 0;
            signUpCanvas.gameObject.SetActive(true);
            signInCanvas.gameObject.SetActive(false);
            mainMenu.gameObject.SetActive(false);
        }
    }

    /*public void StartGame()
    {
        Debug.Log("starting game");
        Time.timeScale = 1;
        signUpCanvas.gameObject.SetActive(false);
        signInCanvas.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(false);
        Debug.Log("started game");
    }*/

    public void ToSignIn()
    {
        signUpCanvas.gameObject.SetActive(false);
        signInCanvas.gameObject.SetActive(true);
        mainMenu.gameObject.SetActive(false);
    }

    public void ToSignUp()
    {
        signUpCanvas.gameObject.SetActive(true);
        signInCanvas.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(false);
    }

    public void ToMainMenu()
    {
        signUpCanvas.gameObject.SetActive(false);
        signInCanvas.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
    }

    public void ToImage()
    {
        SceneManager.LoadScene("ImageSpelling");
    }

    public void ToChallenge()
    {
        SceneManager.LoadScene("Challenge");
    }

    public void ToSandbox()
    {
        SceneManager.LoadScene("Sandbox");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

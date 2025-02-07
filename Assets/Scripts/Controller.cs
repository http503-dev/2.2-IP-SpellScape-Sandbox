using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class Controller : MonoBehaviour
{
    public GameObject signUpCanvas;
    public GameObject signInCanvas;
    public GameObject mainMenu;

    private void Start()
    {
        if (signUpCanvas.gameObject.activeSelf)
        {
            signUpCanvas.gameObject.SetActive(true);
            signInCanvas.gameObject.SetActive(false);
            mainMenu.gameObject.SetActive(false);
        }
    }
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
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();
        SceneManager.LoadScene("ImageSpelling", LoadSceneMode.Single);
    }

    public void ToChallenge()
    {
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();
        SceneManager.LoadScene("Challenge", LoadSceneMode.Single);
    }

    public void ToSandbox()
    {
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();
        SceneManager.LoadScene("Sandbox", LoadSceneMode.Single);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

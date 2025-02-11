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
    public GameObject createProfile;
    public GameObject profilePage;
    public UpdateProfile updateProfile;

    public WebCam webCam;

    private void Start()
    {
        ToSignIn();
    }
    public void ToSignIn()
    {
        signUpCanvas.gameObject.SetActive(false);
        signInCanvas.gameObject.SetActive(true);
        mainMenu.gameObject.SetActive(false);
        createProfile.gameObject.SetActive(false);
        profilePage.gameObject.SetActive(false);
    }

    public void ToSignUp()
    {
        signUpCanvas.gameObject.SetActive(true);
        signInCanvas.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(false);
        createProfile.gameObject.SetActive(false);
    }

    public void ToCreateProfile()
    {
        signUpCanvas.gameObject.SetActive(false);
        createProfile.gameObject.SetActive(true);
        Debug.Log("starting webcam");
        webCam.StartWebCam();
        Debug.Log("webcam started");
    }

    public void ToMainMenu()
    {
        signUpCanvas.gameObject.SetActive(false);
        signInCanvas.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
        createProfile.gameObject.SetActive(false);
        profilePage.gameObject .SetActive(false);
        webCam.StopWebCam();
    }

    public void ToProfile()
    {
        updateProfile.Read();
        createProfile.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(false);
        profilePage.gameObject.SetActive(true);
    }

    public void ToImage()
    {
        Debug.Log("Deinitialize/Initialize");
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();
        Debug.Log("Loading ImageSpelling scene...");
        SceneManager.LoadScene("ImageSpelling", LoadSceneMode.Single);
    }

    public void ToChallenge()
    {
        Debug.Log("Deinitialize/Initialize");
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();
        Debug.Log("Loading Challenge scene...");
        SceneManager.LoadScene("Challenge", LoadSceneMode.Single);
    }

    public void ToSandbox()
    {
        Debug.Log("Deinitialize/Initialize");
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();
        Debug.Log("Loading Sandbox scene...");
        SceneManager.LoadScene("Sandbox", LoadSceneMode.Single);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}

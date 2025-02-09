/*
 * Author: Muhammad Farhan
 * Date: 22/1/2024
 * Description: Script that handles changing of scenes while in game
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.ARFoundation;

public class SceneChanger : MonoBehaviour
{
    /// <summary>
    /// Function to load into Image Tracking scene
    /// </summary>
    public void ToImage()
    {
        Debug.Log("Deinitialize/Initialize");
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();
        Debug.Log("Loading ImageSpelling scene...");
        SceneManager.LoadScene("ImageSpelling", LoadSceneMode.Single);
    }

    /// <summary>
    /// Function to load into Challenge scene
    /// </summary>
    public void ToChallenge()
    {
        Debug.Log("Deinitialize/Initialize");
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();
        Debug.Log("Loading Challenge scene...");
        SceneManager.LoadScene("Challenge", LoadSceneMode.Single);
    }

    /// <summary>
    /// Function to load into Sandbox scene
    /// </summary>
    public void ToSandbox()
    {
        Debug.Log("Deinitialize/Initialize");
        LoaderUtility.Deinitialize();
        LoaderUtility.Initialize();
        Debug.Log("Loading Sandbox scene...");
        SceneManager.LoadScene("Sandbox", LoadSceneMode.Single);
    }

    /// <summary>
    /// Function to quit out of the game
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
}

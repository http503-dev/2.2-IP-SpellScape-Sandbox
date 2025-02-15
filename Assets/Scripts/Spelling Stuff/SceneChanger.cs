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
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class SceneChanger : MonoBehaviour
{
    /// <summary>
    /// Reference to menu panel
    /// </summary>
    public GameObject menuPanel;

    /// <summary>
    /// Called on start to hide menu panel
    /// </summary>
    public void Start()
    {
        menuPanel.SetActive(false);
    }

    /// <summary>
    /// Fucntion to hide menu panel and continue the game
    /// </summary>
    public void HideMenuPanel()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }

    /// <summary>
    /// Function to show menu panel and pause the game
    /// </summary>
    public void ShowMenuPanel()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    /// <summary>
    /// Function to load into Image Tracking scene
    /// </summary>
    public void ToImage()
    {
        Time.timeScale = 1.0f;
        ObjectSpawner.DestroyAllSpawnedBlocks(); // Clean up spawned sandbox blocks
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
        Time.timeScale = 1.0f;
        ObjectSpawner.DestroyAllSpawnedBlocks(); // Clean up spawned sandbox blocks
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
        Time.timeScale = 1.0f;
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

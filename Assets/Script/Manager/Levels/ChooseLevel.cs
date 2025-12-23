using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.Controls;

public class ChooseLevel : MonoBehaviour
{
    public Image loadingBar;
    public GameObject loadingScreen;
    public GameObject chooseCanvas;
    public GameObject mainCanvas;
    public void Level1()
    {
        StartCoroutine(LaunchGame("Level 1"));
    }
    public void Level2()
    {
        StartCoroutine(LaunchGame("Level 2 Final"));
    }
    public void Level3()
    {
        StartCoroutine(LaunchGame("Level 3 Final"));
    }
    public void Level4()
    {
        StartCoroutine(LaunchGame("Level 4"));
    }
    public void Level5()
    {
        StartCoroutine(LaunchGame("Level 5 new"));
    }
    public void Level6()
    {
        StartCoroutine(LaunchGame("Level 6 v2"));
    }
    IEnumerator LaunchGame(string levelName)
    {
        chooseCanvas.SetActive(false);
        mainCanvas.SetActive(false);
        loadingScreen.SetActive(true);
        AsyncOperation loading = SceneManager.LoadSceneAsync(levelName);  

        while (!loading.isDone)
        {
            yield return null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class UiManager : MonoBehaviour
{
    #region Script Parameters
    [Header("UI Parameters")]
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Animator transition;

    [Header("First Selected Options")]
    [SerializeField] private GameObject pauseButton;

    private FileManager fileManager;
    private bool pauseState = false;
    #endregion

    #region Methods 
    public void Pause()
    {
        if (pauseMenu != null)
        {
            if (pauseState)
            {
                pauseState = false;
                pauseMenu.SetActive(false);
                Time.timeScale = 1f;
            }
            else
            {
                pauseState = true;
                pauseMenu.SetActive(true);
                Time.timeScale = 0f;
            }
        }
    }

    public void Return()
    {
        Time.timeScale = 1f;
        pauseState = false;
        SceneManager.LoadScene("Menu");
    }

    public void FirstTransition()
    {
        transition.Play("firstTransition");
    }

    public void SecondTransition() 
    {
        transition.Play("secondtransition");
    }

    public void LoadMenu()
    {
        SceneManager.LoadSceneAsync("Menu");
        Time.timeScale = 1f;
    }

    public void OpenPause()
    {
        EventSystem.current.SetSelectedGameObject(pauseButton);
    }
    #endregion
}

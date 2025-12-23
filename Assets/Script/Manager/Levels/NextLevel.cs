using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

public class NextLevel : MonoBehaviour
{
    public string nextLevel;
    public Animator transition;
    public GameObject loadingScreen;
    private PlayerController player;

    private void Start()
    {
        player = GameManager.Get.playerController;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            player.enabled = false;
            StartCoroutine(TransitionBetweenScenes());
        }
    }
    IEnumerator TransitionBetweenScenes()
    {
        transition.Play("firstTransition");
        yield return new WaitForSeconds(1.5f);
        loadingScreen.SetActive(true); 
        AsyncOperation loading = SceneManager.LoadSceneAsync(nextLevel);
        
        while (!loading.isDone) 
        {
            yield return null;
        }
    }
}

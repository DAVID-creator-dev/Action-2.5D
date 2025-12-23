using System.Collections;
using UnityEngine.Events; 
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Timeline;

public class GameManager : MonoBehaviour
{
    #region Script Parameters
    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public GeneratorManager generatorManager;
    [HideInInspector] public UiManager uiManager;
    [HideInInspector] public CameraManager cameraManager;  

    [HideInInspector] public int lastVisitedCheckpointID = 0;
    [HideInInspector] public bool isInCinematic;
    [HideInInspector] public bool isInMenu;
    [HideInInspector] public bool spawnWithGenerator = true;
    #endregion

    #region Events
    [HideInInspector] public UnityEvent<string> OnSFXPlayerUpdate;
    [HideInInspector] public UnityEvent<string, AudioSource, bool> OnSFXMachinesUpdate;
    [HideInInspector] public UnityEvent<int> OnHealthUpdate;
    #endregion

    #region Static
    private static GameManager Instance;
    public static GameManager Get
    {
        get
        {
            return Instance;
        }
    }
    #endregion

    #region Unity Methods
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        if (uiManager != null)
        {
            if (!isInMenu)
                uiManager.SecondTransition();
        }

        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();

        if (generatorManager == null)
            generatorManager = FindObjectOfType<GeneratorManager>();

        if (uiManager == null)
            uiManager = FindObjectOfType<UiManager>();

        if (cameraManager == null)
            cameraManager = FindObjectOfType<CameraManager>();

        if (playerController == null || generatorManager == null || uiManager == null || cameraManager == null)
        {
            Debug.Log("One components missing: Player, generatorManager, uiManager, cameraManager");
        }
    }

    private void Start()
    {
        OnHealthUpdate.AddListener(isPlayerDead);
    }
    private void OnDisable()
    {
        OnHealthUpdate.RemoveListener(isPlayerDead);
    }
    #endregion

    #region Methods
    private void isPlayerDead(int health)
    {
        if (playerController != null)
        {
            if (playerController && playerController.health <= 0)
            {
                OnSFXPlayerUpdate.Invoke("No_More_Energy"); 
                RunRetry();
            }
        }
    }
    public void RunRetry()
    {
        if (playerController == null || cameraManager == null || uiManager == null || generatorManager == null)
            Debug.Log("One components missing: Player, generatorManager, uiManager, cameraManager");
        else
            StartCoroutine(Retry());

    }

    IEnumerator Retry( )
    {
        isInCinematic = true;

        playerController.transform.gameObject.SetActive(false);

        uiManager.FirstTransition();
        yield return new WaitForSeconds(1.5f);

        playerController.ResetParameters();
        playerController.transform.gameObject.SetActive(true);

        cameraManager.ResetPriority();
        generatorManager.ResetPosition();

        isInCinematic = false;

        uiManager.SecondTransition();
    }

    public void Quit()
    {
        Application.Quit();
    }
    #endregion
}

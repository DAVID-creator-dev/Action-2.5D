using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    #region Script Parameters
    [Header("Menu Parameters")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider interfaceVolumeSlider;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;

    [Header("First Selected Options")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject selectLevelMenu;

    [Header("Sound Volume")]
    [SerializeField] private AudioSource musicVolume;
    [SerializeField] private AudioSource interfaceVolume;

    //State Menu 
    private bool isInMenu;
    private bool isInOption;
    private bool isInLevelSelect;
    #endregion

    #region Methods Unity
    private void Start()
    {
        isInMenu = true; 

        InitResolutionDropdown();
        initGraphicsQuality();

        IniSettingsFile(); 
    }
    #endregion

    private void IniSettingsFile()
    {
        if (musicVolumeSlider != null)
        {
            SetSliderValue(musicVolumeSlider, FileManager.MusicVolume);
            SetMusicVolumeSettings(FileManager.MusicVolume); 
        }
        if (sfxVolumeSlider != null)
        {
            SetSliderValue(sfxVolumeSlider, FileManager.SfxVolume);
            SetSfxVolumeSettings(FileManager.SfxVolume); 
        }
        if (interfaceVolumeSlider != null)
        {
            SetSliderValue(interfaceVolumeSlider, FileManager.Interface);
        }
    }

    #region Methods
    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f)
            {
                if (isInMenu)
                {
                    OpenMainMenu();
                }
                else if (isInOption)
                {
                    Debug.Log("Tet");
                    OpenSettingsMenu();
                }
                else if (isInLevelSelect)
                {
                    OpenSelectLevelMenu();
                }
            }
        }
    }

    [Tooltip("OptionSettings")]
    //Graphics Quality
    private void initGraphicsQuality()
    {
        if (qualityDropdown == null)
        {
            Debug.Log("Quality Dropdown is not assigned!");
            return;
        }

        qualityDropdown.ClearOptions();

        int currentQualityLevel = QualitySettings.GetQualityLevel();

        string[] qualityNames = QualitySettings.names;

        var options = new List<string>(qualityNames);

        qualityDropdown.AddOptions(options);

        qualityDropdown.value = currentQualityLevel;
    }

    public void OnQualityChange(TMP_Dropdown dropdown)
    {
        int selectedQualityIndex = dropdown.value;

        if (selectedQualityIndex >= 0 && selectedQualityIndex < QualitySettings.names.Length)
        {
            QualitySettings.SetQualityLevel(selectedQualityIndex, true);
        }
    }

    //Resolution
    private void InitResolutionDropdown()
    {
        if (resolutionDropdown == null)
        {
            Debug.Log("Resolution Dropdown is not assigned!");
            return;
        }

        resolutionDropdown.ClearOptions();

        Resolution[] resolutions = Screen.resolutions;

        var options = new List<string>();

        foreach (Resolution res in resolutions)
        {
            string option = res.width + " x " + res.height;

            options.Add(option);
        }

        resolutionDropdown.AddOptions(options);

        int storedWidth = FileManager.ResolutionWidth;
        int storedHeight = FileManager.ResolutionHeight;
        string storedResolution = storedWidth + " x " + storedHeight;

        int storedIndex = options.IndexOf(storedResolution);
        if (storedIndex != -1)
        {
            resolutionDropdown.value = storedIndex;
        }

        Screen.SetResolution(storedWidth, storedHeight, true); 
    }

    public void OnResolutionChange(TMP_Dropdown dropdown)
    {
        int selectedResolutionIndex = dropdown.value;

        Resolution[] resolutions = Screen.resolutions;

        if (selectedResolutionIndex >= 0 && selectedResolutionIndex < resolutions.Length)
        {
            Resolution selectedResolution = resolutions[selectedResolutionIndex];
            Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);

            FileManager.ResolutionWidth = selectedResolution.width;
            FileManager.ResolutionHeight = selectedResolution.height;
        }
    }

    //secretLevel
    public void LevelInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            SceneManager.LoadSceneAsync("Level Demo");
        }
    }
    private void SetSliderValue(Slider slider, float value)
    {
        slider.value = value;
    }

    //Volume && Brightness
    public void SetMusicVolumeSettings(float value)
    {
        FileManager.MusicVolume = value;
        musicVolume.volume = value; 
    }

    public void SetSfxVolumeSettings(float value)
    {
        FileManager.SfxVolume = value;  
    }

    public void SetInterfaceSettings(float value)
    {
        FileManager.Interface = value;
        interfaceVolume.volume = value;
    }
    private void OpenMainMenu()
    {
        isInMenu = true; 
        isInOption = false; 
        isInLevelSelect = false;

        EventSystem.current.SetSelectedGameObject(mainMenu); 
    }

    private void OpenSettingsMenu()
    {
        isInMenu = false;
        isInOption = true; 
        isInLevelSelect = false;

        EventSystem.current.SetSelectedGameObject(settingsMenu);
    }

    private void OpenSelectLevelMenu()
    {
        isInMenu = false; 
        isInOption = false;
        isInLevelSelect = true;

        EventSystem.current.SetSelectedGameObject(selectLevelMenu);
    }

    #endregion
}

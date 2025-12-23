using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    #region Script Parameters
    private const string KEY_INTERFACE = "InterfaceVolume";
    private const float DEFAULT_INTERFACE = 1f;

    private const string KEY_MUSIC = "MusicVolume";
    private const float DEFAULT_MUSIC = 1f;

    private const string KEY_SFX = "SfxVolume";
    private const float DEFAULT_SFX = 1f;

    private const string KEY_RESOLUTION_WIDTH = "ResolutionWidth";
    private const string KEY_RESOLUTION_HEIGHT = "ResolutionHeight";

    private const int DEFAULT_RESOLUTION_WIDTH = 1920;
    private const int DEFAULT_RESOLUTION_HEIGHT = 1080;
    #endregion

    #region Methods
    public static float Interface
    {
        get
        {
            if (!PlayerPrefs.HasKey(KEY_INTERFACE))
            {
                PlayerPrefs.SetFloat(KEY_INTERFACE, DEFAULT_INTERFACE);
            }
            return PlayerPrefs.GetFloat(KEY_INTERFACE);
        }
        set
        {
            PlayerPrefs.SetFloat(KEY_INTERFACE, value);
        }
    }

    public static float MusicVolume
    {
        get
        {
            if (!PlayerPrefs.HasKey(KEY_MUSIC))
            {
                PlayerPrefs.SetFloat(KEY_MUSIC, DEFAULT_MUSIC);
            }
            return PlayerPrefs.GetFloat(KEY_MUSIC);
        }
        set
        {
            PlayerPrefs.SetFloat(KEY_MUSIC, value);
        }
    }

    public static float SfxVolume
    {
        get
        {
            if (!PlayerPrefs.HasKey(KEY_SFX))
            {
                PlayerPrefs.SetFloat(KEY_SFX, DEFAULT_SFX);
            }
            return PlayerPrefs.GetFloat(KEY_SFX);
        }
        set
        {
            PlayerPrefs.SetFloat(KEY_SFX, value);
        }
    }

    public static int ResolutionWidth
    {
        get
        {
            if (!PlayerPrefs.HasKey(KEY_RESOLUTION_WIDTH))
            {
                PlayerPrefs.SetInt(KEY_RESOLUTION_WIDTH, DEFAULT_RESOLUTION_WIDTH);
            }
            return PlayerPrefs.GetInt(KEY_RESOLUTION_WIDTH);
        }
        set
        {
            PlayerPrefs.SetInt(KEY_RESOLUTION_WIDTH, value);
        }
    }

    public static int ResolutionHeight
    {
        get
        {
            if (!PlayerPrefs.HasKey(KEY_RESOLUTION_HEIGHT))
            {
                PlayerPrefs.SetInt(KEY_RESOLUTION_HEIGHT, DEFAULT_RESOLUTION_HEIGHT);
            }
            return PlayerPrefs.GetInt(KEY_RESOLUTION_HEIGHT);
        }
        set
        {
            PlayerPrefs.SetInt(KEY_RESOLUTION_HEIGHT, value);
        }
    }
    #endregion
}
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

public class SoundManager : MonoBehaviour
{
    #region Struct
    [System.Serializable]
    public class SFXData
    {
        public string KeyWord;
        public AudioClip SfxSound;
    }
    #endregion

    #region Script Parameters
    [SerializeField] private SFXData[] SfxPlayer;
    [SerializeField] private SFXData[] SfxMachinesLoop;
    [SerializeField] private SFXData[] SfxMachinesOneShoot;

    [SerializeField] private AudioSource MusicVolume, SoundEffectVolumePlayer;

    private Coroutine soundCoroutine;
    #endregion

    #region Unity Methods
    private void Start()
    {
        ApplyGlobalVolume(); 

        GameManager.Get.OnSFXPlayerUpdate.AddListener(PlaySfxPlayerSound);
        GameManager.Get.OnSFXMachinesUpdate.AddListener(PlaySfxMachineLoopSound);
        GameManager.Get.OnSFXMachinesUpdate.AddListener(PlaySfxMachineOneShootSound);
    }

    private void OnDisable()
    {
        GameManager.Get.OnSFXPlayerUpdate.RemoveListener(PlaySfxPlayerSound);
        GameManager.Get.OnSFXMachinesUpdate.RemoveListener(PlaySfxMachineLoopSound);
        GameManager.Get.OnSFXMachinesUpdate.RemoveListener(PlaySfxMachineOneShootSound);
    }
    #endregion

    #region Methods
    private void PlaySfxPlayerSound(string name)
    {
        if (SfxPlayer.Length != 0)
        {
            foreach (SFXData data in SfxPlayer)
            {
                if (data.KeyWord == name)
                {
                    SoundEffectVolumePlayer.PlayOneShot(data.SfxSound);
                    break;
                }
            }
        }
    }

    private void PlaySfxMachineOneShootSound(string name, AudioSource audio, bool loop)
    {
        if (SfxMachinesOneShoot.Length != 0)
        {
            foreach (SFXData data in SfxMachinesOneShoot)
            {
                if (data.KeyWord == name)
                {
                    audio.PlayOneShot(data.SfxSound);
                    break;
                }
            }
        }
    }

    private void PlaySfxMachineLoopSound(string name, AudioSource audio, bool loop)
    {
        if (SfxMachinesLoop.Length != 0)
        {
            foreach (SFXData data in SfxMachinesLoop)
            {
                if (data.KeyWord == name)
                {
                    StopSoundCoroutine();
                    StartSoundCoroutine(audio, data.SfxSound, loop);
                    break;
                }

                if (name == "Stop")
                {
                    StartCoroutine(WaitForSoundCompletion(audio));
                }
            }
        }
    }

    void StopSoundCoroutine()
    {
        if (soundCoroutine != null)
        {
            StopCoroutine(soundCoroutine);
            soundCoroutine = null;
        }
    }

    void StartSoundCoroutine(AudioSource audio, AudioClip clip, bool loop)
    {
        soundCoroutine = StartCoroutine(WaitForSoundCompletion(audio, clip, loop));
    }
    private IEnumerator WaitForSoundCompletion(AudioSource audio)
    {
        audio.loop = false; 
        while (audio.isPlaying)
        {
            yield return null;
        }
        audio.Stop(); 
    }
    private IEnumerator WaitForSoundCompletion(AudioSource audio, AudioClip clip, bool loop)
    {
        audio.loop = false;

        while (audio.isPlaying)
        {
            yield return null;
        }

        audio.clip = clip;
        audio.loop = loop;
        audio.Play();
    }

    private void ApplyGlobalVolume()
    {
        GameObject[] sfxObj = GameObject.FindGameObjectsWithTag("Sfx");
        GameObject[] musicObj = GameObject.FindGameObjectsWithTag("Music");

        if (sfxObj != null)
        {
            foreach (GameObject obj in sfxObj)
            {
                AudioSource sfxVolume = obj.GetComponent<AudioSource>();
                sfxVolume.volume = FileManager.SfxVolume;
            }
        }

        if (musicObj != null)
        {
            foreach (GameObject obj in musicObj)
            {
                AudioSource musicVolume = obj.GetComponent<AudioSource>();
                musicVolume.volume *= FileManager.MusicVolume;
            }
        }
    }
    #endregion
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject optionsPanel;

    [Header("Options UI")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public TMP_Dropdown resolutionDropdown;

    [Header("Audio Source")]
    public AudioSource bgmAudioSource;
    public AudioClip clickSFX;

    private Resolution[] resolutions;

    private void Start()
    {
        // Set active panels
        if (mainPanel != null) mainPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);

        // Setup BGM AudioSource
        if (bgmAudioSource == null) bgmAudioSource = GetComponent<AudioSource>();
        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        }

        // Load saved audio levels
        if (musicSlider != null)
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        if (sfxSlider != null)
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
        }

        // Set up resolutions dropdown
        if (resolutionDropdown != null)
        {
            SetupResolutions();
        }
    }

    public void PlayGame()
    {
        Debug.Log("[MainMenu] Play game clicked - Loading Assignment scene...");
        SceneManager.LoadScene("Assignment");
    }

    public void OpenOptions()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        if (mainPanel != null) mainPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        PlayerPrefs.Save();
    }

    public void ExitGame()
    {
        Debug.Log("[MainMenu] Exit game clicked.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = value;
        }
        Debug.Log($"[MainMenu] Music Volume set to: {value:F2}");
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        Debug.Log($"[MainMenu] SFX Volume set to: {value:F2}");
    }

    public void PlayClickSFX()
    {
        if (bgmAudioSource != null && clickSFX != null)
        {
            float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
            bgmAudioSource.PlayOneShot(clickSFX, sfxVol);
        }
    }

    private void SetupResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @" + resolutions[i].refreshRateRatio.value.ToString("F0") + "Hz";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    public void SetResolution(int index)
    {
        if (index >= 0 && index < resolutions.Length)
        {
            Resolution resolution = resolutions[index];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreenMode);
            Debug.Log($"[MainMenu] Resolution set to: {resolution.width}x{resolution.height}");
        }
    }
}

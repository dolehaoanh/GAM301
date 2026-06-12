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
    public Toggle vsyncToggle;

    [Header("Audio Source")]
    public AudioSource bgmAudioSource;
    public AudioClip clickSFX;

    private Resolution[] resolutions;

    private void Start()
    {
        
        if (mainPanel != null) mainPanel.SetActive(true);
        if (optionsPanel != null) optionsPanel.SetActive(false);

        
        if (mainPanel != null)
        {
            Button playBtn = mainPanel.transform.Find("Play Game")?.GetComponent<Button>();
            if (playBtn != null)
            {
                playBtn.onClick.AddListener(PlayGame);
                playBtn.onClick.AddListener(PlayClickSFX);
            }

            Button optionsBtn = mainPanel.transform.Find("Options")?.GetComponent<Button>();
            if (optionsBtn != null)
            {
                optionsBtn.onClick.AddListener(OpenOptions);
                optionsBtn.onClick.AddListener(PlayClickSFX);
            }

            Button exitBtn = mainPanel.transform.Find("Exit Game")?.GetComponent<Button>();
            if (exitBtn != null)
            {
                exitBtn.onClick.AddListener(ExitGame);
                exitBtn.onClick.AddListener(PlayClickSFX);
            }

            
            Image logoImg = mainPanel.transform.Find("Logo")?.GetComponent<Image>();
            if (logoImg != null)
            {
                logoImg.raycastTarget = false;
            }
        }

        if (optionsPanel != null)
        {
            Button backBtn = optionsPanel.transform.Find("Back")?.GetComponent<Button>();
            if (backBtn != null)
            {
                backBtn.onClick.AddListener(CloseOptions);
                backBtn.onClick.AddListener(PlayClickSFX);
            }
        }


        
        if (vsyncToggle != null)
        {
            vsyncToggle.isOn = QualitySettings.vSyncCount > 0;
            vsyncToggle.onValueChanged.AddListener(SetVSync);
        }

        
        if (bgmAudioSource == null) bgmAudioSource = GetComponent<AudioSource>();
        if (bgmAudioSource != null)
        {
            bgmAudioSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        }

        
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

        
        if (resolutionDropdown != null)
        {
            SetupResolutions();
        }
    }

    public void PlayGame()
    {
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
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    public void PlayClickSFX()
    {
        if (bgmAudioSource != null && clickSFX != null)
        {
            float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
            bgmAudioSource.PlayOneShot(clickSFX, sfxVol);
        }
    }

    public void SetVSync(bool isOn)
    {
        QualitySettings.vSyncCount = isOn ? 1 : 0;
        PlayerPrefs.SetInt("VSync", isOn ? 1 : 0);
        PlayerPrefs.Save();
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
        }
    }
}

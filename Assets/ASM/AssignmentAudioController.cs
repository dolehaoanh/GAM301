using UnityEngine;

public class AssignmentAudioController : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip bgmClip;
    public AudioClip clickSFX;

    private AudioSource bgmAudioSource;
    private AudioSource sfxAudioSource;

    private void Start()
    {
        // Setup BGM AudioSource
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        bgmAudioSource.clip = bgmClip;
        bgmAudioSource.loop = true;
        bgmAudioSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        
        if (bgmClip != null)
        {
            bgmAudioSource.Play();
            Debug.Log($"[AssignmentAudio] Playing BGM: {bgmClip.name} with volume {bgmAudioSource.volume}");
        }
        else
        {
            Debug.LogWarning("[AssignmentAudio] BGM clip is not assigned!");
        }

        // Setup SFX AudioSource
        sfxAudioSource = gameObject.AddComponent<AudioSource>();
    }

    private Vector3 startMousePos0;
    private Vector3 startMousePos1;
    private Vector3 startMousePos2;

    private void Update()
    {
        // Record mouse down positions
        if (Input.GetMouseButtonDown(0)) startMousePos0 = Input.mousePosition;
        if (Input.GetMouseButtonDown(1)) startMousePos1 = Input.mousePosition;
        if (Input.GetMouseButtonDown(2)) startMousePos2 = Input.mousePosition;

        // Play SFX on mouse button release only if it was a click (not a drag)
        if (Input.GetMouseButtonUp(0))
        {
            if (Vector3.Distance(startMousePos0, Input.mousePosition) < 5f)
            {
                PlayClickSFX();
            }
        }
        if (Input.GetMouseButtonUp(1))
        {
            if (Vector3.Distance(startMousePos1, Input.mousePosition) < 5f)
            {
                PlayClickSFX();
            }
        }
        if (Input.GetMouseButtonUp(2))
        {
            if (Vector3.Distance(startMousePos2, Input.mousePosition) < 5f)
            {
                PlayClickSFX();
            }
        }
    }

    private void PlayClickSFX()
    {
        if (sfxAudioSource != null && clickSFX != null)
        {
            float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
            sfxAudioSource.PlayOneShot(clickSFX, sfxVol);
        }
    }
}

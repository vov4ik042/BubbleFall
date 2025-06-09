using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField] private Button btnExit;
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private Slider sliderSounds;
    [SerializeField] private AudioMixer audioMixer;

    private void Awake()
    {
        btnExit.onClick.AddListener(() =>
        {
            Hide();
        });
    }
    public void SetValueMusic()
    {
        float volume = sliderMusic.value;
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume * 2) * 20);
        PlayerPrefs.SetFloat("MusicVolumeKey", volume);
    }
    public void SetValueSFX()
    {
        float volume = sliderSounds.value;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolumeKey", volume);
    }
    public void LoadVolume()
    {
        sliderMusic.value = PlayerPrefs.GetFloat("MusicVolumeKey");
        sliderSounds.value = PlayerPrefs.GetFloat("SFXVolumeKey");
        SetValueMusic();
        SetValueSFX();
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}

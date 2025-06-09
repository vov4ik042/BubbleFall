using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu Instance;

    [SerializeField] private Button btnPlay;
    [SerializeField] private Button btnSettings;
    [SerializeField] private Button btnExit;
    [SerializeField] private Settings settingsWindow;
    private void Start()
    {
        if (PlayerPrefs.HasKey("MusicVolumeKey") && PlayerPrefs.HasKey("SFXVolumeKey"))
        {
            settingsWindow.LoadVolume();
        }
        else
        {
            settingsWindow.SetValueMusic();
            settingsWindow.SetValueSFX();
        }
    }
    private void Awake()
    {
        Instance = this;
        btnPlay.onClick.AddListener(() =>
        {
            Audio.Instance.PlaySFX(4);
            SceneManager.LoadScene("Game");
        });
        btnSettings.onClick.AddListener(() =>
        {
            Audio.Instance.PlaySFX(4);
            settingsWindow.Show();
        });
        btnExit.onClick.AddListener(() =>
        {
            Audio.Instance.PlaySFX(4);
            Application.Quit();
        });
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnRestart;
    [SerializeField] private Button btnMainMenu;

    private void Awake()
    {
        btnClose.onClick.AddListener(() =>
        {
            Audio.Instance.PlaySFX(4);
            CustomPool.Instance.SetStopOrGoAllBalls(true);
            gameObject.SetActive(false);
        });
        btnMainMenu.onClick.AddListener(() =>
        {
            Audio.Instance.PlaySFX(4);
            SceneManager.LoadScene("Menu");
        });
        btnRestart.onClick.AddListener(() =>
        {
            Audio.Instance.PlaySFX(4);
            SceneManager.LoadScene("Game");
        });
    }

    private void Start()
    {
        CustomPool.Instance.SetStopOrGoAllBalls(false);
    }
}

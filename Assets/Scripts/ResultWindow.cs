using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultWindow : MonoBehaviour
{
    public static ResultWindow Instance;

    [SerializeField] private GameObject line;
    [SerializeField] private GameObject btnClose;
    [SerializeField] private Button btnPlayAgain;
    [SerializeField] private Button btnMainMenu;
    [SerializeField] private GameObject score;
    [SerializeField] private TextMeshProUGUI textScore;
    [SerializeField] private TextMeshProUGUI textWithResult;

    private void Awake()
    {
        Instance = this;
        btnPlayAgain.onClick.AddListener(() =>
        {
            Audio.Instance.PlaySFX(4);
            SceneManager.LoadScene("Game");
        });
        btnMainMenu.onClick.AddListener(() =>
        {
            Audio.Instance.PlaySFX(4);
            SceneManager.LoadScene("Menu");
        });
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Show(int score, bool result)
    {
        CustomPool.Instance.SetStopOrGoAllBalls(false);
        line.gameObject.SetActive(false);
        btnClose.gameObject.SetActive(false);
        this.score.gameObject.SetActive(false);

        if (result)
        {
            textWithResult.text = "Victory!";
        }
        else
        {
            textWithResult.text = "Game Over";
        }
        textScore.text = $"Your score: {score}";
        gameObject.SetActive(true);
    }
}

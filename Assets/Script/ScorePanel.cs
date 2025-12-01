using UnityEngine;
using TMPro;

public class ScorePanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI correctScoreText;
    [SerializeField] TextMeshProUGUI wrongScoreText;
    [SerializeField] CountDown countDown;
    [SerializeField] GameManager gameManager;

    private int correctNum;
    private int wrongNum;

    void Start()
    {
        //  GetComponent<CountDown>() と GetComponent<GameManager>() は削除
    }

    void Update()
    {
        if (countDown.isFinished == true)
        {
            GetScore();
            UpdateUI();
        }
    }

    void GetScore()
    {
        // 修正: プロパティから直接値を取得
        correctNum = gameManager.CorrectScoreNum;
        wrongNum = gameManager.WrongScoreNum;
    }

    void UpdateUI()
    {
        correctScoreText.text = correctNum.ToString();
        wrongScoreText.text = wrongNum.ToString();
    }
}
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    void OnEnable()
    {
        ScoreManager.OnCountChanged += UpdateUI;
        UpdateUI(ScoreManager.Count); // 初期表示
    }

    void OnDisable()
    {
        ScoreManager.OnCountChanged -= UpdateUI;
    }

    void UpdateUI(int count)
    {
        scoreText.text = $"Score : {count}";
    }
}

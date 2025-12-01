using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] PanelManager panelManager;
    [SerializeField] GameObject resultPanel;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip correctSE;
    [SerializeField] AudioClip correctVoice;
    [SerializeField] AudioClip wrongSE;
    [SerializeField] CountDown countDown;

    [SerializeField] public Material[] cubeMaterials;

    private List<int?> inputNumbers;
    private int correctResult;
    private int correctScoreNum;
    private int wrongScoreNum;
    private string currentEquationString;

    void Start()
    {
        panelManager.CubeMaterials = cubeMaterials;
        InitializeGame();
    }
    void OnEnable()
    {
        // イベントリスナーを登録
        countDown.OnGameStart.AddListener(InitializeGame);
    }
    
    void OnDisable()
    {
        // イベントリスナーを解除
        countDown.OnGameStart.RemoveListener(InitializeGame);
    }
    
    void Update()
    {
        if (countDown.isFinished == true)
        {
            resultPanel.SetActive(true);
        }
    }

    private void InitializeGame()
    {
        SetNewProblem();
    }
    
    public int CorrectScoreNum
    {
        get { return correctScoreNum; }
    }

    public int WrongScoreNum
    {
        get { return wrongScoreNum; }
    }

    public void ProcessNumber(int cubeIndex, int num)
    {
        if (inputNumbers == null || cubeIndex >= inputNumbers.Count) return;
        
        inputNumbers[cubeIndex] = num;

        panelManager.UpdateUI(inputNumbers, currentEquationString, correctResult);
        
        bool allNumbersEntered = true;
        for (int i = 0; i < inputNumbers.Count; i++)
        {
            if (!inputNumbers[i].HasValue)
            {
                allNumbersEntered = false;
                break;
            }
        }
        
        if (allNumbersEntered)
        {
            CheckAnswer();
        }
    }
    
    private void SetNewProblem()
    {
        int numCount = 3;
        inputNumbers = new List<int?>(new int?[numCount]);

        EquationGenerator generator = new EquationGenerator();
        List<int> correctNumbers;

        currentEquationString = generator.Generate(numCount, out correctNumbers, out correctResult);

        panelManager.InitializeUI();
        panelManager.UpdateUI(inputNumbers, currentEquationString, correctResult);
    }
    
    private void CheckAnswer()
    {
        ExpressionEvaluator evaluator = new ExpressionEvaluator();
        int calculatedAnswer = evaluator.Evaluate(currentEquationString, inputNumbers);

        if (calculatedAnswer == correctResult)
        {
            audioSource.PlayOneShot(correctSE, 3f);
            audioSource.PlayOneShot(correctVoice, 2f);
            correctScoreNum++;
            Debug.Log("正解です！次の問題へ。");
            Invoke("SetNewProblem", 2f);
        }
        else
        {
            audioSource.PlayOneShot(wrongSE, 3f);
            wrongScoreNum++;
            Debug.Log("不正解です。");
        }

        Invoke("ResetInputNumbers", 0.5f);
    }
    
    private void ResetInputNumbers()
    {
        for (int i = 0; i < inputNumbers.Count; i++)
        {
            inputNumbers[i] = null;
        }
        panelManager.UpdateUI(inputNumbers, currentEquationString, correctResult);
    }
}
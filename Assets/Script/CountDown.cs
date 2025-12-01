using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class CountDown : MonoBehaviour
{
    [SerializeField] float MaxCountDownTime;
    [SerializeField] TextMeshProUGUI CountDownText;
    [SerializeField] TextMeshProUGUI StartCountDownText;
    [SerializeField] GameObject StartCountDownPanel;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] countDownSE;
    [SerializeField] AudioClip countDownEndSE;
    [SerializeField] AudioClip countDownFinishSE;

    public UnityEvent OnGameStart;
    
    private float timer;
    public bool isFinished = false;
    private bool isGameStarted = false;
    private bool isFlashing = false; // 点滅中かどうかのフラグ

    void Start()
    {
        timer = MaxCountDownTime;
        UpdateUI();
        StartCoroutine(StartOfCountDown());
    }

    IEnumerator StartOfCountDown()
    {
        StartCountDownPanel.SetActive(true);
        CountDownText.enabled = false;
        yield return new WaitForSeconds(1f);

        StartCountDownText.text = "3";
        audioSource.PlayOneShot(countDownSE[0]);
        yield return new WaitForSeconds(1f);


        StartCountDownText.text = "2";
        audioSource.PlayOneShot(countDownSE[1]);
        yield return new WaitForSeconds(1f);

        StartCountDownText.text = "1";
        audioSource.PlayOneShot(countDownSE[2]);
        yield return new WaitForSeconds(1f);


        StartCountDownText.text = "Go!!";
        audioSource.PlayOneShot(countDownSE[3]);
        yield return new WaitForSeconds(1f);


        StartCountDownPanel.SetActive(false);
        CountDownText.enabled = true;
        
        isGameStarted = true;
        OnGameStart.Invoke();
    }
    
    void Update()
    {
        if (isFinished || !isGameStarted) return;

        timer -= Time.deltaTime;

        // --- 修正箇所：残り10秒以下になったらコルーチンを開始 ---
        if (timer <= 11f && !isFlashing)
        {
            isFlashing = true;
            audioSource.PlayOneShot(countDownEndSE);
            StartCoroutine(FlashText());
        }
        
        if (timer <= 0)
        {
            timer = 0;
            isFinished = true;
            Debug.Log("Finish");
            CountDownText.text = "終了！";
            CountDownText.color = Color.white;
            audioSource.PlayOneShot(countDownFinishSE);
            StopCoroutine(FlashText()); // 終了したら点滅を停止
        }
        
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (isFinished) return;
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        CountDownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // --- 追加：1秒ごとにテキストを点滅させるコルーチン ---
    IEnumerator FlashText()
    {
        while (timer > 0)
        {
            // 色を切り替え
            if (CountDownText.color == Color.white)
            {
                CountDownText.color = Color.red;
            }
            else
            {
                CountDownText.color = Color.white;
            }
            yield return new WaitForSeconds(1f); // 1秒待機
        }
    }
}
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CountdownTimePanelController : MonoBehaviour
{
    [SerializeField] CountDown countDown;

    CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        Hide();
    }

    void Update()
    {
        if (countDown == null) return;

        // ゲーム中のみ表示
        if (!countDown.isFinished && IsGameCountdownRunning())
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    bool IsGameCountdownRunning()
    {
        // スタートの 3,2,1 が終わり、
        // メインのカウントダウンが動いているか
        return countDown.enabled && !countDown.isFinished;
    }

    void Show()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}

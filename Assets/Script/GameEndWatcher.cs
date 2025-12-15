using UnityEngine;

public class GameEndWatcher : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] CountDown countDown;
    [SerializeField] GameObject scorePanel;
    [SerializeField] RandomLauncher ballLauncher;

    [Header("終了時処理")]
    [SerializeField] string ballTag = "Ball";

    private bool handled = false;

    void Start()
    {
        if (scorePanel != null)
            scorePanel.SetActive(false);
    }

    void Update()
    {
        if (handled) return;
        if (countDown == null) return;

        if (countDown.isFinished)
        {
            handled = true;
            HandleGameEnd();
        }
    }

    void HandleGameEnd()
    {
        // ① 既存のボールを全削除
        GameObject[] balls = GameObject.FindGameObjectsWithTag(ballTag);
        foreach (var ball in balls)
        {
            Destroy(ball);
        }

        // ② 発射SEを止める（RandomLauncherにAudioSourceがある場合）
        if (ballLauncher != null)
        {
            AudioSource launcherAudio = ballLauncher.GetComponent<AudioSource>();
            if (launcherAudio != null)
            {
                launcherAudio.Stop();
            }
        }

        // ③ BallLauncher を無効化
        if (ballLauncher != null)
        {
            ballLauncher.gameObject.SetActive(false);
        }

        // ④ スコアパネル表示
        if (scorePanel != null)
        {
            scorePanel.SetActive(true);
        }

        Debug.Log("=== GAME END: Cleanup Completed ===");
    }
}

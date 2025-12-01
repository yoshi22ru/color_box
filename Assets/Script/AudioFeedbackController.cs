using UnityEngine;

public class AudioFeedbackController : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Pitch Mapping Settings")]
    [Tooltip("シーン全体で最も小さいオブジェクトのスケール")]
    public float globalMinScale = 1.0f; 
    [Tooltip("シーン全体で最も大きいオブジェクトのスケール")]
    public float globalMaxScale = 15.0f; 
    
    [Tooltip("最小スケール時のピッチ（小さいオブジェクト = 高い音）")]
    public float minPitch = 3.0f; 
    [Tooltip("最大スケール時のピッチ（大きいオブジェクト = 低い音）")]
    public float maxPitch = 0.1f; 
    
    [Header("Distance Mapping Settings")]
    [Tooltip("最小スケール時の最大聞こえる距離 (Max Distance)")]
    public float minMaxDistance = 1.0f;
    [Tooltip("最大スケール時の最大聞こえる距離 (Max Distance)")]
    public float maxMaxDistance = 3.0f;

    public AudioClip rayAudioClip;


    void Awake()
    {
        // AudioSource を取得。なければ自動追加。
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1.0f; // 3D サウンド

        UpdateAudioParameters();
    }


    // -------------------------------
    //  サイズに基づく音量・距離調整
    // -------------------------------
    public void UpdateAudioParameters()
    {
        if (audioSource == null) return;

        float currentScale = Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z);

        float normalizedScale = Mathf.InverseLerp(globalMinScale, globalMaxScale, currentScale);
        normalizedScale = Mathf.Clamp01(normalizedScale);

        // Pitch : 小さいほど高い音 → 大きいほど低い音
        float targetPitch = Mathf.Lerp(minPitch, maxPitch, normalizedScale);
        audioSource.pitch = targetPitch;

        // MaxDistance : 大きいオブジェクトほど遠くでも聞こえる
        float targetMaxDistance = Mathf.Lerp(minMaxDistance, maxMaxDistance, normalizedScale);
        audioSource.maxDistance = targetMaxDistance;

        Debug.Log($"[AudioFeedback] Update => Scale:{currentScale}, Norm:{normalizedScale:F2}, Pitch:{targetPitch:F2}, MaxDist:{targetMaxDistance:F2}");
    }


    // サイズ正規化値を取得（必要なら使う）
    public float GetNormalizedScale()
    {
        float currentScale = Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        float normalized = Mathf.InverseLerp(globalMinScale, globalMaxScale, currentScale);
        return Mathf.Clamp01(normalized);
    }
}

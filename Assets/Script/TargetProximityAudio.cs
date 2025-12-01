using UnityEngine;


public class TargetProximityAudio : MonoBehaviour
{
    [Header("Audio References")]
    public AudioSource inductionAudioSource;
    public MetalSurfaceAudio metalSurfaceAudio;

    [Header("Ray References")]
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor[] handRayInteractors;

    private AudioFeedbackController sizeController;

    [Header("Distance Mapping")]
    public float maxDistance = 3f;          // Ray の最大長に合わせる
    public float minDistanceThreshold = 0.1f;

    [Header("Pitch Settings")]
    public float baseMinPitch = 0.1f;
    public float baseMaxPitch = 8.0f;       // 劇的変化用に高め

    [Header("Volume Settings")]
    public float minVolume = 0.0f;
    public float maxVolume = 1.0f;

    private float currentPitch = 1f;
    private float currentVolume = 0f;

    void Start()
    {
        sizeController = GetComponent<AudioFeedbackController>();

        if (inductionAudioSource != null)
        {
            inductionAudioSource.spatialBlend = 1.0f;
            inductionAudioSource.loop = true;

            if (inductionAudioSource.clip != null)
                inductionAudioSource.Play();
        }
    }

    void Update()
    {
        if (handRayInteractors == null || handRayInteractors.Length == 0) return;

        float closestDistance = float.MaxValue;
        bool hitDetected = false;

        // -----------------------------
        // 左右の手のRayで最短距離を取得
        // -----------------------------
        foreach (var ray in handRayInteractors)
        {
            if (ray == null) continue;

            if (ray.TryGetCurrent3DRaycastHit(out RaycastHit hit) && hit.collider != null)
            {
                float d = Vector3.Distance(ray.transform.position, hit.point);

                // Debug.Log($"[RayHit] ray={ray.name}, origin={ray.transform.position}, hit={hit.point}, dist={d}");

                if (d < closestDistance)
                {
                    closestDistance = d;
                    hitDetected = true;
                }

                // Ray音源も更新
                if (metalSurfaceAudio != null && metalSurfaceAudio.audioRayPivot != null)
                    metalSurfaceAudio.LateUpdateRayHit(hit.point);
            }
        }

        // -----------------------------
        // ヒットなし or 近すぎ → 音停止
        // -----------------------------
        if (!hitDetected || closestDistance < minDistanceThreshold)
        {
            if (inductionAudioSource.isPlaying)
                inductionAudioSource.Stop();
            return;
        }

        if (!inductionAudioSource.isPlaying)
            inductionAudioSource.Play();

        // -----------------------------
        // 非線形距離カーブ
        // -----------------------------
        float n = (closestDistance - minDistanceThreshold) / (maxDistance - minDistanceThreshold);
        n = Mathf.Clamp01(n);

        // 距離が近いほど爆発的に変化
        float curved = Mathf.Pow(1f - n, 6f);   // 6乗で劇的変化
        curved = Mathf.Pow(curved, 1.5f);       // さらに指数補正
        curved = Mathf.SmoothStep(0f, 1f, curved);

        // -----------------------------
        // ピッチ / 音量
        // -----------------------------
        float targetPitch = Mathf.Lerp(baseMinPitch, baseMaxPitch, curved);
        float targetVolume = Mathf.Lerp(minVolume, maxVolume, curved);

        // -----------------------------
        // 滑らかに変化
        // -----------------------------
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * 12f);
        currentVolume = Mathf.Lerp(currentVolume, targetVolume, Time.deltaTime * 12f);

        inductionAudioSource.pitch = currentPitch;
        inductionAudioSource.volume = currentVolume;

        // Ray音源にも適用
        if (metalSurfaceAudio != null && metalSurfaceAudio.audioRaySource != null)
            metalSurfaceAudio.audioRaySource.pitch = currentPitch * 0.8f;

        // -----------------------------
        // デバッグ出力
        // -----------------------------
        // Debug.Log($"[Distance] closestDistance={closestDistance:F3}, n={n:F3}, curved={curved:F3}, pitch={currentPitch:F3}, volume={currentVolume:F3}");
    }
}

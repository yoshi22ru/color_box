using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MetalSurfaceAudio : MonoBehaviour
{
    [Header("Child Audio Sources")]
    public Transform audioRayPivot;
    public AudioSource audioRaySource;

    public Transform audioDirectPivot;
    public AudioSource audioDirectSource; // 手のひら検知用（単一音源）

    [Header("Palm Touch AudioClip (Single)")]
    public AudioClip palmTouchClip; // ★手のひら専用のAudioClip

    [Header("Direct Touch Audio Clips (Fingertips)")]
    public AudioClip[] directTouchClips = new AudioClip[10]; // 指先10本用のAudioClip配列

    private bool rayWasHitting = false;
    private bool palmIsTouching = false; 
    private Vector3 directHitPoint;

    void Awake()
    {
        // Spatial Blend の設定
        if (audioRaySource != null) audioRaySource.spatialBlend = 1.0f;
        if (audioDirectSource != null) audioDirectSource.spatialBlend = 1.0f;
    }

    // ------------ Ray -------------------
    public void LateUpdateRayHit(Vector3 hitPoint)
    {
        if (audioRayPivot == null || audioRaySource == null) return;

        audioRayPivot.position = hitPoint;

        if (!audioRaySource.isPlaying)
            audioRaySource.Play();

        rayWasHitting = true;
    }

    public void ResetRayHit()
    {
        if (rayWasHitting && audioRaySource.isPlaying)
        {
            audioRaySource.Stop();
        }
        rayWasHitting = false;
    }

    // ------------ Direct Touch: 指先検知 (複数の指) -------------------
    public void SetDirectHit(AudioSource fingerAudioSource, int fingerIndex) 
    {
        if (fingerIndex < 0 || fingerIndex >= directTouchClips.Length) return;

        // クリップが異なれば設定
        if (fingerAudioSource.clip != directTouchClips[fingerIndex])
        {
             fingerAudioSource.clip = directTouchClips[fingerIndex];
        }

        // 音を再生
        if (!fingerAudioSource.isPlaying)
        {
            fingerAudioSource.Play();
        }
    }

    // 指先検知用：指が離れた時の音停止
    public void ReleaseDirectHit(AudioSource fingerAudioSource)
    {
        if (fingerAudioSource.isPlaying)
            fingerAudioSource.Stop();
    }
    
    // ------------ Direct Touch: 手のひら検知 (単一音源) -------------------
    public void SetPalmTouch(Vector3 hitPoint)
    {
        if (audioDirectSource == null || palmTouchClip == null) return;
        
        palmIsTouching = true;
        directHitPoint = hitPoint;
        
        // palmTouchClip を audioDirectSource に設定
        if (audioDirectSource.clip != palmTouchClip)
        {
            audioDirectSource.clip = palmTouchClip;
        }
        
        if (!audioDirectSource.isPlaying)
            audioDirectSource.Play();
    }

    // 手のひら検知用：手が離れた時の音停止
    public void ReleasePalmTouch()
    {
        palmIsTouching = false;
        if (audioDirectSource.isPlaying)
            audioDirectSource.Stop();
    }
    
    // -------------------------------------------------------------------

    void LateUpdate()
    {
        // 手のひら検知の場合のみ audioDirectPivot の位置を更新
        if (palmIsTouching && audioDirectPivot != null)
        {
            audioDirectPivot.position = directHitPoint;
        }
    }
}
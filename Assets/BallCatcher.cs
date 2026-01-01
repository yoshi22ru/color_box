using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BallCatcher : MonoBehaviour
{
    [Header("キャッチ時の音")]
    public AudioClip catchSound;
    [Range(0, 1)] public float soundVolume = 1.0f;

    [Header("エフェクト")]
    public GameObject catchEffectPrefab;
    [Tooltip("エフェクトの表示時間（秒）")]
    public float effectLifeTime = 2.0f;

    [Header("デバッグ")]
    public bool showDebugLog = true;
    public bool showDebugGizmos = true;

    // 現在触れているゾーン
    private HashSet<HandZone> touchingZones = new HashSet<HandZone>();

    private bool isCaught = false;

    void Update()
    {
        if (isCaught) return;

        CheckState(HandZone.HandSide.Right);
        CheckState(HandZone.HandSide.Left);
    }

    // ================================
    // 状態チェック
    // ================================
    void CheckState(HandZone.HandSide side)
    {
        bool palm = IsZoneTouching(side, HandZone.ZoneType.Palm);
        bool finger = IsZoneTouching(side, HandZone.ZoneType.Finger);

        if (palm && finger)
        {
            if (showDebugLog)
                Debug.Log($"<color=green>[BallCatcher] CATCH READY ({side})</color>");

            CatchSuccess(side);
        }
    }

    bool IsZoneTouching(HandZone.HandSide side, HandZone.ZoneType type)
    {
        foreach (var zone in touchingZones)
        {
            if (zone == null) continue;
            if (zone.handSide == side && zone.zoneType == type)
                return true;
        }
        return false;
    }

    // ================================
    // Trigger
    // ================================
    void OnTriggerEnter(Collider other)
    {
        HandZone zone = other.GetComponent<HandZone>();
        if (zone != null)
        {
            touchingZones.Add(zone);

            if (showDebugLog)
                Debug.Log($"[BallCatcher] Enter: {zone.handSide} {zone.zoneType}");
        }
    }

    void OnTriggerExit(Collider other)
    {
        HandZone zone = other.GetComponent<HandZone>();
        if (zone != null)
        {
            touchingZones.Remove(zone);

            if (showDebugLog)
                Debug.Log($"[BallCatcher] Exit: {zone.handSide} {zone.zoneType}");
        }
    }

    // ================================
    // Catch Success
    // ================================
    void CatchSuccess(HandZone.HandSide side)
    {
        if (isCaught) return;
        isCaught = true;

        Debug.Log($"<color=cyan>=== NICE CATCH ({side}) ===</color>");

        // スコア
        ScoreManager.AddCount(1);

        // SE
        if (catchSound != null)
        {
            AudioSource.PlayClipAtPoint(
                catchSound,
                transform.position,
                soundVolume);
        }
        else
        {
            Debug.LogWarning("[BallCatcher] catchSound is NULL");
        }

        // エフェクト
        SpawnCatchEffect();

        // ボールは1フレーム後に消す
        StartCoroutine(DestroyNextFrame());
    }

    // ================================
    // Effect Spawn
    // ================================
    void SpawnCatchEffect()
    {
        if (catchEffectPrefab == null)
        {
            Debug.LogError("[BallCatcher] catchEffectPrefab is NULL");
            return;
        }

        Vector3 spawnPos = transform.position + Vector3.up * 0.2f;

        Debug.Log($"[BallCatcher] Effect Spawn Pos: {spawnPos}");

        GameObject effect = Instantiate(
            catchEffectPrefab,
            spawnPos,
            Quaternion.identity);

        Debug.Log($"[BallCatcher] Effect Instantiated: {effect.name}");

        Destroy(effect, effectLifeTime);
    }

    IEnumerator DestroyNextFrame()
    {
        yield return null;
        Destroy(gameObject);
    }

    // ================================
    // Gizmos（Scene可視化）
    // ================================
    void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.15f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 0.2f, 0.1f);
    }
}

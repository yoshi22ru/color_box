using UnityEngine;
using System.Collections.Generic;

public class BallCatcher : MonoBehaviour
{
    [Header("キャッチ時の音")]
    public AudioClip catchSound;
    [Range(0, 1)] public float soundVolume = 1.0f;

    [Header("エフェクト")]
    public GameObject catchEffectPrefab;

    [Header("デバッグ表示")]
    [Tooltip("チェックを入れると、現在の接触状態をコンソールに出し続けます")]
    public bool showDebugLog = true;

    // 現在触れているゾーンの管理
    private HashSet<HandZone> touchingZones = new HashSet<HandZone>();

    void Update()
    {
        // 毎フレーム、状態を監視してログを出す
        CheckState(HandZone.HandSide.Right);
        CheckState(HandZone.HandSide.Left);
    }

    void CheckState(HandZone.HandSide side)
    {
        // 1. 今、ボールは何に触れているか？
        bool isPalmTouching = IsZoneTouching(side, HandZone.ZoneType.Palm);
        bool isFingerTouching = IsZoneTouching(side, HandZone.ZoneType.Finger);

        // 2. 状態判定
        if (isPalmTouching && isFingerTouching)
        {
            // 【状態：キャッチ可能】
            // ボールが「手のひら」と「指」の両方に同時に触れている（＝3つが重なっている）
            if (showDebugLog)
            {
                Debug.Log($"<color=green>【{side}】掴める状態です！ (Palm & Finger Touching)</color>");
            }

            // ここでキャッチ実行！
            CatchSuccess(side);
        }
        else
        {
            // 【状態：キャッチ不可】
            // どちらか片方、あるいは両方に触れていない
            if (showDebugLog)
            {
                // うるさすぎる場合はここをコメントアウトしてください
                string state = "None";
                if (isPalmTouching) state = "Palm Only";
                if (isFingerTouching) state = "Finger Only";

                if (state != "None") // 何にも触れてない時はログを出さない
                {
                    Debug.Log($"<color=yellow>【{side}】掴めない状態です... (Current: {state})</color>");
                }
            }
        }
    }

    // 指定した部位が今触れているかチェックする関数
    bool IsZoneTouching(HandZone.HandSide side, HandZone.ZoneType type)
    {
        foreach (var zone in touchingZones)
        {
            // リストの中に null (消滅したオブジェクト) が混ざっていたら除去
            if (zone == null) continue;

            if (zone.handSide == side && zone.zoneType == type) return true;
        }
        return false;
    }

    void OnTriggerEnter(Collider other)
    {
        HandZone zone = other.GetComponent<HandZone>();
        if (zone != null)
        {
            touchingZones.Add(zone);
        }
    }

    void OnTriggerExit(Collider other)
    {
        HandZone zone = other.GetComponent<HandZone>();
        if (zone != null)
        {
            touchingZones.Remove(zone);
        }
    }

    void CatchSuccess(HandZone.HandSide side)
    {
        // 多重発動防止
        if (!gameObject.activeSelf) return;

        Debug.Log($"<color=cyan>=== NICE CATCH! ({side}) ===</color>");

        if (catchSound != null) AudioSource.PlayClipAtPoint(catchSound, transform.position, soundVolume);
        if (catchEffectPrefab != null) Instantiate(catchEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
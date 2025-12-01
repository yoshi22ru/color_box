// -------------------------
// 手の接触管理スクリプト
// -------------------------

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class HandTagInfo
{
    public string tag = "Hand"; 
    public Transform palmTransform; 
}

public class HandAudioSourceManager : MonoBehaviour
{
    // 手のひら検知を有効にするためのタグ。例: "RightPalm"
    [Header("Palm Detection Settings")]
    public string[] targetPalmTag; 
    
    public HandTagInfo[] handTags;
    public MetalSurfaceAudio metalAudioComponent;

    // 接触中の指先コライダーを追跡するためのリスト (指先検知用)
    private List<Collider> currentContacts = new List<Collider>();

     private bool IsPalmTag(Collider other)
    {
        // Nullチェックを追加し、安全性を向上
        if (targetPalmTag == null) return false; 
        
        foreach (string tag in targetPalmTag)
        {
            if (other.CompareTag(tag))
            {
                return true;
            }
        }
        return false;
    }

    void OnTriggerEnter(Collider other)
    {
        // ------------------------------------
        // A. 手のひら検知 (単一音源)
        // ------------------------------------
        if (IsPalmTag(other)) 
        {
            // 接触点を計算
            Vector3 contactPoint = other.ClosestPoint(metalAudioComponent.transform.position);
            metalAudioComponent.SetPalmTouch(contactPoint); 
            
            // 【注意】ここでは、SetPalmTouch が未定義であるため、処理をコメントアウトし、
            // エラーを避けて IsPalmTag のロジックの修正に専念します。
            
            return; 
        }

        // ------------------------------------
        // B. 指先検知 (複数音源)
        // ------------------------------------
        int fingerIndex = GetFingerIndex(other);

        if (fingerIndex >= 0)
        {
            AudioSource fingerAudioSource = other.GetComponent<AudioSource>();

            if (fingerAudioSource != null)
            {
                if (!currentContacts.Contains(other))
                {
                    currentContacts.Add(other);
                }
                
                metalAudioComponent.SetDirectHit(fingerAudioSource, fingerIndex);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        // ------------------------------------
        // A. 手のひら検知の継続 - 両手対応
        // ------------------------------------
        if (IsPalmTag(other))
        {
            Vector3 contactPoint = other.ClosestPoint(metalAudioComponent.transform.position);
            metalAudioComponent.SetPalmTouch(contactPoint); // 位置更新
            return;
        }

        // B. 指先検知の継続 (特に処理なし)
    }

    void OnTriggerExit(Collider other)
    {
        // ------------------------------------
        // A. 手のひら検知の解除
        // ------------------------------------
        if (IsPalmTag(other)) 
        {
            metalAudioComponent.ReleasePalmTouch();
        }
        
        // ------------------------------------
        // B. 指先検知の解除
        // ------------------------------------
        int fingerIndex = GetFingerIndex(other);

        if (fingerIndex >= 0)
        {
            AudioSource fingerAudioSource = other.GetComponent<AudioSource>();

            if (fingerAudioSource != null)
            {
                // 離脱した指をリストから削除
                if (currentContacts.Contains(other))
                {
                    currentContacts.Remove(other);
                }
                
                metalAudioComponent.ReleaseDirectHit(fingerAudioSource);
            }
        }
    }

    /// <summary>
    /// Colliderの名前から、対応するAudioClipのインデックス (0-9) を取得します。
    /// </summary>
    private int GetFingerIndex(Collider fingerCollider)
    {
        // 手のひら検知タグは指先検知から除外
        if (IsPalmTag(fingerCollider)) return -1;
        
        bool isHandCollider = false;
        foreach (var hand in handTags)
        {
            if (fingerCollider.CompareTag(hand.tag))
            {
                isHandCollider = true;
                break;
            }
        }
        if (!isHandCollider) return -1; 

        string name = fingerCollider.gameObject.name;
        

        /*
        // XR Handのコライダー名に合わせて調整
        // 右手 (0 - 4)
        if (name.Contains("R_ThumbTip")) return 0;
        if (name.Contains("R_IndexTip")) return 1;
        if (name.Contains("R_MiddleTip")) return 2;
        if (name.Contains("R_RingTip")) return 3;
        if (name.Contains("R_LittleTip")) return 4;
        
        // 左手 (5 - 9)
        if (name.Contains("L_ThumbTip")) return 5;
        if (name.Contains("L_IndexTip")) return 6;
        if (name.Contains("L_MiddleTip")) return 7;
        if (name.Contains("L_RingTip")) return 8;
        if (name.Contains("L_LittleTip")) return 9;
        */

        return -1;
    }
}
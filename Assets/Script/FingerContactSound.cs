using UnityEngine;
using System;

// Inspectorで設定するためのデータ構造
[Serializable]
public struct FingerContactSound
{
    [Tooltip("指先のコライダーのタグまたは名前 (例: L_Index)")]
    public string fingerId;
    [Tooltip("この指が触れたときに鳴らす AudioClip")]
    public AudioClip contactSound;
    [Tooltip("この指専用のAudioSource (動的生成推奨)")]
    public AudioSource audioSource;
}
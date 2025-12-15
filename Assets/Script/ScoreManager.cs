using UnityEngine;
using System;

public class ScoreManager : MonoBehaviour
{
    // 現在のカウント（どこからでも参照可能）
    public static int Count { get; private set; } = 0;

    // カウントが変わったときの通知イベント
    public static event Action<int> OnCountChanged;

    public static void AddCount(int value = 1)
    {
        Count += value;
        OnCountChanged?.Invoke(Count);
    }

    // リセット用（必要なら）
    public static void ResetCount()
    {
        Count = 0;
        OnCountChanged?.Invoke(Count);
    }
}

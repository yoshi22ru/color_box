using UnityEngine;

public class HandZone : MonoBehaviour
{
    public enum HandSide { Left, Right }
    public enum ZoneType { Palm, Finger }

    [Header("部位の設定")]
    public HandSide handSide; // 左手か右手か
    public ZoneType zoneType; // 手のひらか指か
}
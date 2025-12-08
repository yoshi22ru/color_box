using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class CurvingBall : MonoBehaviour
{
    [Header("動きの設定")]
    public float curveAmount = 0f; // カーブの強さ

    [Header("音の設定")]
    [Tooltip("プレイヤーの頭（Main Camera）。ランチャーから自動セットされます")]
    public Transform listenerHead; 
    
    [Tooltip("ピッチ変化の最小値（遠い時）")]
    public float minPitch = 0.5f;
    [Tooltip("ピッチ変化の最大値（至近距離）")]
    public float maxPitch = 2.0f;
    [Tooltip("この距離より遠いと最小ピッチ、近いと最大ピッチになります")]
    public float maxDistance = 20f;

    private Rigidbody rb;
    private AudioSource audioSource;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        // AudioSourceの基本設定をコードで強制（ミス防止）
        audioSource.spatialBlend = 1.0f; // 完全3Dサウンド
        audioSource.dopplerLevel = 1.0f; // Unity標準ドップラーも有効化
        audioSource.loop = true;         // ループ再生
        
        if (!audioSource.isPlaying) audioSource.Play();
    }

    void FixedUpdate()
    {
        // 1. カーブ処理
        if (rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            Vector3 sideVector = Vector3.Cross(rb.linearVelocity.normalized, Vector3.up);
            rb.AddForce(sideVector * curveAmount, ForceMode.Acceleration);
        }
    }

    void Update()
    {
        // 2. 音の動的制御（プロキシミティ・ピッチ）
        if (listenerHead != null)
        {
            float distance = Vector3.Distance(transform.position, listenerHead.position);

            // 距離を 0～1 の値に変換（遠い=0, 近い=1）
            // Mathf.InverseLerp(a, b, v) は vがaなら0, bなら1を返します
            float t = Mathf.InverseLerp(maxDistance, 0.5f, distance);

            // 距離に応じてピッチを変化させる (近づくほど高音)
            // これにより風切り音のような緊張感を演出
            audioSource.pitch = Mathf.Lerp(minPitch, maxPitch, t);
            
            // ついでに近づくと少し音量も上げる（迫力用）
            // Spatial Blendで自然に音量は変わりますが、さらに強調します
            audioSource.volume = Mathf.Lerp(0.5f, 1.0f, t);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 衝突したら消える
        Destroy(gameObject);
    }
}
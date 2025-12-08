using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HybridSoundBall : MonoBehaviour
{
    [Header("動きの設定")]
    public float curveAmount = 0f;

    [Header("ターゲット情報")]
    public Transform listenerHead; // 発射時にセット

    [Header("音源の割り当て（重要）")]
    [Tooltip("電子音・ビープ音用のAudioSource")]
    public AudioSource signalSource;
    [Tooltip("風切り音・ノイズ用のAudioSource")]
    public AudioSource windSource;

    [Header("【Signal】接近アラーム設定")]
    public float maxSignalDistance = 20f; // この距離からピッチ変化開始
    public float choppingStartDistance = 5f; // この距離から切れ始める
    public float minSignalPitch = 0.8f;
    public float maxSignalPitch = 3.0f;
    public float minChopSpeed = 10f;
    public float maxChopSpeed = 50f;

    [Header("【Wind】風切り音設定")]
    public float minWindPitch = 0.8f;
    public float maxWindPitch = 1.5f;
    [Tooltip("この速度(m/s)で風切り音が最大音量になる")]
    public float maxSpeedForWind = 20f; 

    private Rigidbody rb;
    private float chopTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        // エラー防止：AudioSourceがセットされてなければ自分自身から探す
        if (signalSource == null || windSource == null)
        {
            var sources = GetComponents<AudioSource>();
            if (sources.Length >= 2)
            {
                signalSource = sources[0];
                windSource = sources[1];
            }
            else
            {
                Debug.LogError("AudioSourceが2つ必要です！");
            }
        }

        // --- Signal音（距離用）の初期設定 ---
        signalSource.spatialBlend = 1.0f; 
        signalSource.dopplerLevel = 0.2f; // 電子音はドップラー少なめで聞きやすく
        signalSource.loop = true;
        if (!signalSource.isPlaying) signalSource.Play();

        // --- Wind音（風圧用）の初期設定 ---
        windSource.spatialBlend = 1.0f;
        windSource.dopplerLevel = 1.5f;   // 風はドップラー強めで「通り過ぎた感」を出す
        windSource.loop = true;
        if (!windSource.isPlaying) windSource.Play();
    }

    void FixedUpdate()
    {
        // カーブ処理
        if (rb.linearVelocity.sqrMagnitude > 0.1f)
        {
            Vector3 sideVector = Vector3.Cross(rb.linearVelocity.normalized, Vector3.up);
            rb.AddForce(sideVector * curveAmount, ForceMode.Acceleration);
        }
    }

    void Update()
    {
        if (listenerHead == null) return;

        float distance = Vector3.Distance(transform.position, listenerHead.position);
        float speed = rb.linearVelocity.magnitude;

        // ==========================================
        // 1. Signal Source (接近アラーム) の制御
        // ==========================================
        
        // ピッチ制御（距離ベース）
        float t_dist = Mathf.InverseLerp(maxSignalDistance, 0.5f, distance);
        signalSource.pitch = Mathf.Lerp(minSignalPitch, maxSignalPitch, t_dist);

        // ボリューム制御（チョッピング）
        if (distance > choppingStartDistance)
        {
            signalSource.volume = 1.0f; // 遠い時は鳴りっぱなし
        }
        else
        {
            // 近い時は高速点滅
            float t_chop = Mathf.InverseLerp(choppingStartDistance, 0.1f, distance);
            float currentChopSpeed = Mathf.Lerp(minChopSpeed, maxChopSpeed, t_chop);
            
            chopTimer += Time.deltaTime * currentChopSpeed;
            // 矩形波でON/OFF
            signalSource.volume = (Mathf.Sin(chopTimer) > 0) ? 1.0f : 0.0f;
        }

        // ==========================================
        // 2. Wind Source (風切り音) の制御
        // ==========================================
        
        // 速度が速いほど音が大きくなる
        // Mathf.Clamp01 は値を 0~1 に制限します
        float speedFactor = Mathf.Clamp01(speed / maxSpeedForWind);
        
        // 基本音量に速度を反映
        windSource.volume = speedFactor;

        // 速度が速いと風の音程も少し高くする（より鋭い音に）
        windSource.pitch = Mathf.Lerp(minWindPitch, maxWindPitch, speedFactor);
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
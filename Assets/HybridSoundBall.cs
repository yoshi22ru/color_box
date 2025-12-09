using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HybridSoundBall : MonoBehaviour
{
    private Rigidbody rb;

    [Header("ターゲット情報")]
    public Transform listenerHead;

    [Header("音源設定")]
    public AudioSource signalSource;
    public AudioSource windSource;

    [Header("Signal - 接近アラーム設定")]
    public float maxSignalDistance = 20f;
    public float choppingStartDistance = 10f;
    public float minSignalPitch = 0.7f;
    public float maxSignalPitch = 1.3f;
    public float minChopSpeed = 10f;
    public float maxChopSpeed = 50f;

    [Header("Wind - 風切り音設定")]
    public float minWindPitch = 0.8f;
    public float maxWindPitch = 1.5f;

    [Header("弾の物理 / カーブ")]
    public float curveAmount = 0f;

    [Header("消滅設定（ミス判定）")]
    [Tooltip("プレイヤーを通り過ぎてから何メートル飛んだら消えるか")]
    public float destroyMargin = 3.0f; 
    [Tooltip("この高さより下に落ちたら消える（床判定）")]
    public float floorHeight = -0.5f;

    private TrajectoryPredictor trajectory;
    private bool initialized = false;
    private float chopTimer;
    
    // 距離計算用
    private Vector3 startPosition;
    private float distanceToTarget; // 発射地点からターゲットまでの距離

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trajectory = GetComponent<TrajectoryPredictor>();

        if (signalSource != null) signalSource.Play();
        if (windSource != null) windSource.Play();
    }

    public void Initialize(Vector3 velocity, Transform target, float curve)
    {
        listenerHead = target;
        curveAmount = curve;
        
        // スタート地点と、ターゲットまでの距離を記録
        startPosition = transform.position;
        if (target != null)
        {
            distanceToTarget = Vector3.Distance(startPosition, target.position);
        }

        rb.linearVelocity = velocity;
        
        if (trajectory != null)
        {
            trajectory.ShowTrajectory(transform.position, velocity, curveAmount);
        }

        initialized = true;
    }

    void FixedUpdate()
    {
        if (curveAmount != 0f)
        {
            Vector3 sideVector = Vector3.Cross(rb.linearVelocity.normalized, Vector3.up);
            rb.AddForce(sideVector * curveAmount, ForceMode.Acceleration);
        }
    }

    void Update()
    {
        if (!initialized || listenerHead == null) return;

        // --- 1. ミス（通り過ぎ・落下）判定 ---
        CheckMissAndDestroy();

        // --- 2. 音の処理 ---
        UpdateAudio();
    }

    // ★追加：ボールが役割を終えたかチェックして消す
    void CheckMissAndDestroy()
    {
        // A. 床に落ちた場合
        if (transform.position.y < floorHeight)
        {
            Destroy(gameObject);
            return;
        }

        // B. プレイヤーの後ろに通り過ぎた場合
        // 「発射地点からの現在の距離」が「発射地点からターゲットまでの距離 + 余白」を超えたら消す
        float currentDistanceFromStart = Vector3.Distance(startPosition, transform.position);

        if (currentDistanceFromStart > distanceToTarget + destroyMargin)
        {
            // ここで消すことで、軌跡も一緒に消えます
            Destroy(gameObject);
        }
    }

    void UpdateAudio()
    {
        float distance = Vector3.Distance(transform.position, listenerHead.position);
        float speed = rb.linearVelocity.magnitude;

        // Signal (接近音)
        float t_dist = Mathf.InverseLerp(maxSignalDistance, 0.5f, distance);
        signalSource.pitch = Mathf.Lerp(minSignalPitch, maxSignalPitch, t_dist);

        if (distance > choppingStartDistance)
        {
             signalSource.volume = 1.0f; 
        }
        else
        {
            float t_chop = Mathf.InverseLerp(choppingStartDistance, 0.5f, distance);
            float currentChopSpeed = Mathf.Lerp(minChopSpeed, maxChopSpeed, t_chop);
            chopTimer += Time.deltaTime * currentChopSpeed;
            signalSource.volume = (Mathf.Sin(chopTimer) * 0.5f) + 0.5f;
        }

        // Wind (風切り音)
        float speedFactor = Mathf.Clamp01(speed / 20f);
        windSource.volume = speedFactor;
        windSource.pitch = Mathf.Lerp(minWindPitch, maxWindPitch, speedFactor);
    }

    void OnDestroy()
    {
        if (trajectory != null) trajectory.Hide();
    }
}
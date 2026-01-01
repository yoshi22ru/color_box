using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HybridSoundBall : MonoBehaviour
{
    private Rigidbody rb;
    private TrajectoryPredictor trajectory;

    [Header("ターゲット")]
    public Transform listenerHead;

    [Header("音源")]
    public AudioSource signalSource;
    public AudioSource windSource;

    [Header("警告音")]
    public AudioSource warningSource;
    public float warningDistance = 4.0f;   // ★ 近づく前に鳴る
    [Range(0f, 1f)]
    public float minWarningVolume = 0.5f;

    [Header("Signal設定")]
    public float maxSignalDistance = 20f;
    public float choppingStartDistance = 10f;
    public float minSignalPitch = 0.7f;
    public float maxSignalPitch = 1.3f;
    public float minChopSpeed = 10f;
    public float maxChopSpeed = 50f;

    [Header("Wind設定")]
    public float minWindPitch = 0.8f;
    public float maxWindPitch = 1.5f;

    [Header("物理")]
    public float curveAmount = 0f;

    [Header("消滅")]
    public float destroyMargin = 3.0f;
    public float floorHeight = -0.5f;

    private bool initialized;
    private float chopTimer;

    private Vector3 startPosition;
    private float distanceToTarget;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trajectory = GetComponent<TrajectoryPredictor>();

        if (signalSource != null) signalSource.Play();
        if (windSource != null) windSource.Play();

        if (warningSource != null)
        {
            warningSource.loop = true;
            warningSource.Stop();
        }
    }

    public void Initialize(Vector3 velocity, Transform target, float curve)
    {
        listenerHead = target;
        curveAmount = curve;

        startPosition = transform.position;
        if (target != null)
            distanceToTarget = Vector3.Distance(startPosition, target.position);

        rb.linearVelocity = velocity;

        if (trajectory != null)
            trajectory.ShowTrajectory(transform.position, velocity, curveAmount);

        initialized = true;
    }

    void FixedUpdate()
    {
        if (curveAmount != 0f && rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            Vector3 side = Vector3.Cross(rb.linearVelocity.normalized, Vector3.up);
            rb.AddForce(side * curveAmount, ForceMode.Acceleration);
        }
    }

    void Update()
    {
        if (!initialized || listenerHead == null) return;

        CheckMissAndDestroy();
        UpdateWarning();
        UpdateAudio();
    }

    // -------------------------
    // 警告ゾーン処理（核心）
    // -------------------------
    void UpdateWarning()
    {
        if (warningSource == null || listenerHead == null) return;

        Vector3 toUser = listenerHead.position - transform.position;
        Vector3 velocity = rb.linearVelocity;

        if (velocity.sqrMagnitude < 0.01f) return;

        Vector3 velocityDir = velocity.normalized;

        // ★ ユーザーに向かっているか
        float approaching = Vector3.Dot(velocityDir, toUser.normalized);

        // ★ 将来の最近接距離（軌跡ベース）
        float futureClosestDistance =
            Vector3.Cross(toUser, velocityDir).magnitude;

        bool willBeDangerous =
            approaching > 0.5f &&                 // 向かってきている
            futureClosestDistance <= warningDistance; // 将来近くを通る

        if (willBeDangerous)
        {
            if (!warningSource.isPlaying)
                warningSource.Play();

            float t =
                Mathf.InverseLerp(warningDistance, 0.3f, futureClosestDistance);

            warningSource.pitch = Mathf.Lerp(0.9f, 1.6f, t);
            warningSource.volume = Mathf.Lerp(0.4f, 1.0f, t);

            if (trajectory != null)
                trajectory.SetWarningColor();
        }
        else
        {
            if (warningSource.isPlaying)
                warningSource.Stop();

            if (trajectory != null)
                trajectory.SetNormalColor();
        }
    }

    // -------------------------
    // 通常音
    // -------------------------
    void UpdateAudio()
    {
        float distance =
            Vector3.Distance(transform.position, listenerHead.position);

        float speed = rb.linearVelocity.magnitude;

        float tDist =
            Mathf.InverseLerp(maxSignalDistance, 0.5f, distance);

        signalSource.pitch =
            Mathf.Lerp(minSignalPitch, maxSignalPitch, tDist);

        if (distance > choppingStartDistance)
        {
            signalSource.volume = 1f;
        }
        else
        {
            float tChop =
                Mathf.InverseLerp(choppingStartDistance, 0.5f, distance);

            float chopSpeed =
                Mathf.Lerp(minChopSpeed, maxChopSpeed, tChop);

            chopTimer += Time.deltaTime * chopSpeed;
            signalSource.volume =
                Mathf.Sin(chopTimer) * 0.5f + 0.5f;
        }

        float speedFactor = Mathf.Clamp01(speed / 20f);
        windSource.volume = speedFactor;
        windSource.pitch =
            Mathf.Lerp(minWindPitch, maxWindPitch, speedFactor);
    }

    // -------------------------
    // 消滅判定
    // -------------------------
    void CheckMissAndDestroy()
    {
        if (transform.position.y < floorHeight)
        {
            Destroy(gameObject);
            return;
        }

        float traveled =
            Vector3.Distance(startPosition, transform.position);

        if (traveled > distanceToTarget + destroyMargin)
            Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (trajectory != null) trajectory.Hide();
        if (warningSource != null && warningSource.isPlaying)
            warningSource.Stop();
    }
}

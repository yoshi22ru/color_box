using UnityEngine;

public class BallLauncher : MonoBehaviour
{
    [Header("設定")]
    public GameObject ballPrefab;
    public Transform target; // プレイヤーのカメラ(Head)
    public Transform firePoint;

    [Header("投球パラメータ")]
    [Range(10f, 80f)]
    public float launchAngle = 45f;
    [Range(-20f, 20f)]
    public float curveAmount = 0f;
    public float autoFireInterval = 4f;

    [Header("ターゲットのばらつき")]
    [Tooltip("ターゲット中心から上下左右にずらす範囲(m)")]
    public Vector3 targetOffsetRange = new Vector3(1.0f, 0.5f, 0f);

    private float timer;

    void Start()
    {
        if (firePoint == null) firePoint = transform;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) ThrowBall();

        timer += Time.deltaTime;
        if (timer >= autoFireInterval)
        {
            ThrowBall();
            timer = 0;
        }
    }

    public void ThrowBall()
    {
        if (target == null || ballPrefab == null) return;

        // 1. 狙う位置をランダムにずらす計算
        // ターゲットのローカル座標系ではなく、ワールド座標で単純にずらす例
        // (必要に応じて target.right * random などに変更可)
        float offsetX = Random.Range(-targetOffsetRange.x, targetOffsetRange.x);
        float offsetY = Random.Range(-targetOffsetRange.y, targetOffsetRange.y);
        
        // ターゲットの現在位置 + ランダムなズレ = 実際の到達点
        Vector3 aimPosition = target.position + new Vector3(offsetX, offsetY, 0);

        // 2. ボール生成
        GameObject ballObj = Instantiate(ballPrefab, firePoint.position, firePoint.rotation);
        HybridSoundBall ballScript = ballObj.GetComponent<HybridSoundBall>();
        Rigidbody rb = ballObj.GetComponent<Rigidbody>();

        // 3. ボールに情報を渡す
        if (ballScript != null)
        {
            ballScript.curveAmount = curveAmount;
            ballScript.listenerHead = target; // 音の計算用にターゲット（耳）の位置を教える
        }

        // 4. 物理計算で初速を与える
        Vector3 velocity = CalculateVelocity(firePoint.position, aimPosition, launchAngle);

        if (!float.IsNaN(velocity.x))
        {
            rb.linearVelocity = velocity;
        }
        else
        {
            Destroy(ballObj);
        }
    }

    private Vector3 CalculateVelocity(Vector3 start, Vector3 end, float angle)
    {
        Vector3 direction = end - start;
        Vector3 groundDirection = new Vector3(direction.x, 0, direction.z);
        float distance = groundDirection.magnitude;
        float heightDifference = direction.y;
        float angleRad = angle * Mathf.Deg2Rad;
        float gravity = Physics.gravity.y;

        float tanAlpha = Mathf.Tan(angleRad);
        float cosAlpha = Mathf.Cos(angleRad);

        float denominator = 2 * cosAlpha * cosAlpha * (distance * tanAlpha - heightDifference);
        if (denominator <= 0) return Vector3.zero;

        float v = Mathf.Sqrt((Mathf.Abs(gravity) * distance * distance) / denominator);
        return groundDirection.normalized * (v * cosAlpha) + Vector3.up * (v * Mathf.Sin(angleRad));
    }
}
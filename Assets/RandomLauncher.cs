using UnityEngine;

public class RandomLauncher : MonoBehaviour
{
    [Header("設定")]
    public GameObject ballPrefab;
    public Transform target; // プレイヤーの頭（耳）
    public Transform firePoint;

    [Header("投球パラメータ")]
    [Range(10f, 80f)]
    public float launchAngle = 45f;
    [Range(-20f, 20f)]
    public float curveAmount = 0f;
    public float autoFireInterval = 4f;

    [Header("ターゲットのばらつき")]
    public Vector3 targetOffsetRange = new Vector3(1.0f, 0.5f, 0f);

    private float timer;

    void Start()
    {
        if (firePoint == null) firePoint = transform;
    }

    void Update()
    {
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

        // 1. ターゲット位置のランダム化
        float offsetX = Random.Range(-targetOffsetRange.x, targetOffsetRange.x);
        float offsetY = Random.Range(-targetOffsetRange.y, targetOffsetRange.y);
        Vector3 aimPosition = target.position + new Vector3(offsetX, offsetY, 0);

        // 2. ボール生成
        GameObject ballObj = Instantiate(ballPrefab, firePoint.position, firePoint.rotation);
        HybridSoundBall ballScript = ballObj.GetComponent<HybridSoundBall>();
        
        // 3. 初速計算
        Vector3 velocity = CalculateVelocity(firePoint.position, aimPosition, launchAngle);

        if (float.IsNaN(velocity.x))
        {
            Destroy(ballObj);
            return;
        }

        // 4. ★重要★ ボールを初期化（ここで軌跡描画や音の設定が走る）
        if (ballScript != null)
        {
            ballScript.Initialize(velocity, target, curveAmount);
        }
    }

    private Vector3 CalculateVelocity(Vector3 start, Vector3 end, float angle)
    {
        Vector3 direction = end - start;
        Vector3 groundDirection = new Vector3(direction.x, 0, direction.z);
        float distance = groundDirection.magnitude;
        float heightDifference = direction.y;
        float angleRad = angle * Mathf.Deg2Rad;
        
        // 重力のY成分を取得 (通常は -9.81)
        float gravity = Physics.gravity.y;

        float tanAlpha = Mathf.Tan(angleRad);
        float cosAlpha = Mathf.Cos(angleRad);

        // 物理公式による初速計算
        float denominator = 2 * cosAlpha * cosAlpha * (distance * tanAlpha - heightDifference);
        
        if (denominator <= 0) return Vector3.zero; // 計算不能（届かない角度など）

        float v = Mathf.Sqrt((Mathf.Abs(gravity) * distance * distance) / denominator);
        return groundDirection.normalized * (v * cosAlpha) + Vector3.up * (v * Mathf.Sin(angleRad));
    }
}
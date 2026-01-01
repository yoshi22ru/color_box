using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryPredictor : MonoBehaviour
{
    [Header("軌跡設定")]
    public float predictionTime = 1.5f;
    public int resolution = 30;

    [Header("色設定")]
    public Color normalColor = Color.white;
    public Color warningColor = Color.red;

    private LineRenderer lineRenderer;
    private Material runtimeMaterial;

    // ★ 追加：警告色ロック
    private bool isWarningLocked = false;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;

        // ★ ランタイム専用マテリアル
        runtimeMaterial = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material = runtimeMaterial;

        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.25f;

        SetNormalColor();
    }

    public void ShowTrajectory(Vector3 startPos, Vector3 initialVelocity, float curveForce)
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = resolution;

        Vector3[] points = new Vector3[resolution];
        Vector3 currentPos = startPos;
        Vector3 currentVel = initialVelocity;
        float timeStep = predictionTime / resolution;

        for (int i = 0; i < resolution; i++)
        {
            points[i] = currentPos;

            currentVel += Physics.gravity * timeStep;

            if (curveForce != 0 && currentVel.sqrMagnitude > 0.1f)
            {
                Vector3 sideVector =
                    Vector3.Cross(currentVel.normalized, Vector3.up);
                currentVel += sideVector * curveForce * timeStep;
            }

            currentPos += currentVel * timeStep;
        }

        lineRenderer.SetPositions(points);
    }

    // ===== 色制御 =====

    public void SetWarningColor()
    {
        // ★ すでに赤なら何もしない
        if (isWarningLocked) return;

        isWarningLocked = true;

        if (runtimeMaterial != null)
            runtimeMaterial.color = warningColor;
    }

    public void SetNormalColor()
    {
        // ★ 警告に入ったら二度と白に戻さない
        if (isWarningLocked) return;

        if (runtimeMaterial != null)
            runtimeMaterial.color = normalColor;
    }

    public void ResetColor()
    {
        // ★ 新しいボール用（再利用時）
        isWarningLocked = false;
        SetNormalColor();
    }

    public void Hide()
    {
        lineRenderer.enabled = false;
    }
}

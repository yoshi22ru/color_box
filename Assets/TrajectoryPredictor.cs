using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryPredictor : MonoBehaviour
{
    [Header("軌跡設定")]
    public float predictionTime = 1.5f; // 何秒先まで表示するか
    public int resolution = 30;         // 描画の滑らかさ

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false; // 最初は隠しておく
        
        // 見た目の設定（太さなど）
        lineRenderer.startWidth = 0.2f;
        lineRenderer.endWidth = 0.25f;
        // マテリアルがピンクにならないようデフォルトをセット
        if (lineRenderer.material == null)
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
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

            // --- 簡易物理シミュレーション ---
            // 1. 重力
            currentVel += Physics.gravity * timeStep;

            // 2. カーブ力 (HybridSoundBallのロジックを再現)
            if (curveForce != 0 && currentVel.sqrMagnitude > 0.1f)
            {
                Vector3 sideVector = Vector3.Cross(currentVel.normalized, Vector3.up);
                currentVel += sideVector * curveForce * timeStep;
            }

            // 3. 移動
            currentPos += currentVel * timeStep;
        }

        lineRenderer.SetPositions(points);
    }

    public void Hide()
    {
        lineRenderer.enabled = false;
    }
}
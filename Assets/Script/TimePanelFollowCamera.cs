using UnityEngine;

public class TimePanelFollowCamera : MonoBehaviour
{
    [Header("追従対象")]
    public Transform cameraTransform;

    [Header("オフセット（カメラ基準）")]
    public Vector3 positionOffset = new Vector3(0.25f, -0.25f, 1.4f);

    [Header("追従の滑らかさ")]
    [Range(1f, 20f)]
    public float followSpeed = 8f;

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // カメラ基準での目標位置
        Vector3 targetPos =
            cameraTransform.position +
            cameraTransform.right * positionOffset.x +
            cameraTransform.up * positionOffset.y +
            cameraTransform.forward * positionOffset.z;

        // スムーズ追従
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            Time.deltaTime * followSpeed
        );

        // 常にカメラ向き
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(transform.position - cameraTransform.position),
            Time.deltaTime * followSpeed
        );
    }
}

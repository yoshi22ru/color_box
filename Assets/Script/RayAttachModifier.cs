using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class RayAttachModifier : MonoBehaviour
{
    [SerializeField] float pullSpeed;      // 引き寄せ速度
    [SerializeField] float stopDistance; // 手元で止める距離

    UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;
    Transform pullingInteractor;
    bool isPulling = false;
    Rigidbody rb;

    void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        grab.selectEntered.AddListener(OnSelectEnter);
        grab.selectExited.AddListener(OnSelectExit);
    }

    void OnDestroy()
    {
        grab.selectEntered.RemoveListener(OnSelectEnter);
        grab.selectExited.RemoveListener(OnSelectExit);
    }

    void OnSelectEnter(SelectEnterEventArgs args)
    {
        pullingInteractor = args.interactorObject.transform;
        isPulling = true;

        // 引き寄せ中は物理オフ
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }

        // 引き寄せ完了までは手に追従させない
        grab.trackPosition = false;
        grab.trackRotation = false;
    }

    void OnSelectExit(SelectExitEventArgs args)
    {
        isPulling = false;
        pullingInteractor = null;

        // 離したら物理オンで落下
        if (rb != null)
            rb.isKinematic = false;

        // XRGrabInteractable の通常挙動に戻す
        grab.trackPosition = true;
        grab.trackRotation = true;
    }

    void Update()
    {
        if (pullingInteractor == null) return;

        // 手の動きに合わせてオブジェクトの位置を更新
        Vector3 targetPos = pullingInteractor.position;
        float distance = Vector3.Distance(transform.position, targetPos); // ここでdistanceを定義

        if (isPulling)
        {
            if (distance > stopDistance)
            {
                // 手元へスムーズに移動
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    pullSpeed * Time.deltaTime
                );
            }
            else
            {
                // 引き寄せが完了
                isPulling = false;
            }
        }
        else // 引き寄せ完了後の追従処理
        {
            transform.position = targetPos;
            transform.rotation = pullingInteractor.rotation;
        }
    }
}
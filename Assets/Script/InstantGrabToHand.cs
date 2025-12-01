using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Hands;
using UnityEngine.XR;


public class InstantGrabToHand : MonoBehaviour
{
    public XRHandSubsystem handSubsystem;
    public XRNode handNode = XRNode.RightHand; // 右手を使う場合。左手なら LeftHand
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
    }

    void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        // XR Handsから手の位置を取得
        if (handSubsystem != null && handSubsystem.running)
        {
            XRHand hand = handNode == XRNode.RightHand ? handSubsystem.rightHand : handSubsystem.leftHand;

            if (hand.isTracked)
            {
                // 手の中心（例：Palm）を使う
                XRHandJoint joint = hand.GetJoint(XRHandJointID.Palm);
                if (joint.TryGetPose(out Pose pose))
                {
                    transform.position = pose.position;
                    transform.rotation = pose.rotation;
                }
            }
        }
        else
        {
            Debug.LogWarning("XRHandSubsystemが動作していません");
        }
    }
}

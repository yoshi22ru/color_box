using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandAttachSwitcher : MonoBehaviour
{
    [SerializeField] private Transform rightHandAttachPoint;
    [SerializeField] private Transform leftHandAttachPoint;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab;

    void Awake()
    {
        grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grab.selectEntered.AddListener(OnSelectEnter);
    }

    void OnDestroy()
    {
        grab.selectEntered.RemoveListener(OnSelectEnter);
    }

    void OnSelectEnter(SelectEnterEventArgs args)
    {
        // インタラクター（掴んだ側）が右手か左手かを判定
        if (args.interactorObject.transform.name.Contains("Right"))
        {
            grab.attachTransform = rightHandAttachPoint;
        }
        else if (args.interactorObject.transform.name.Contains("Left"))
        {
            grab.attachTransform = leftHandAttachPoint;
        }
    }
}

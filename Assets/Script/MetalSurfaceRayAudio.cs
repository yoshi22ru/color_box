using UnityEngine;

public class MetalSurfaceRayAudio : MonoBehaviour
{
    public MetalSurfaceAudio metalAudio;
    public Transform[] rayOrigins;
    public float rayDistance = 3f;

    void LateUpdate()
    {
        if (metalAudio == null || rayOrigins == null) return;

        bool hitDetected = false;

        foreach (var rayOrigin in rayOrigins)
        {
            if (rayOrigin == null) continue;

            if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out RaycastHit hit, rayDistance))
            {    
                metalAudio.LateUpdateRayHit(hit.point);
                hitDetected = true;
            }
        }

        if (!hitDetected)
        {
            metalAudio.ResetRayHit();
        }
    }
}

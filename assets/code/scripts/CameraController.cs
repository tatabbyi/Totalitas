using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target to Follow")]
    public Transform target; //player
    [Header("Camera Settings")]
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    void LateUpdate() //moves camera to target smoothly
    {
        if (target == null) return;
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}

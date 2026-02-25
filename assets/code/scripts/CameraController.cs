using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target to Follow")]
    public Transform target; //player
    [Header("Camera Settings")]
    public float smoothTime = 0.125f; //camera moves to desired position speed
    public Vector3 offset = new Vector3(-10f, 10f, -10f); //place camera diagonally above and behind 
    [Header("Rotate Camera")]

    public bool lockRotation = true; //camera fixed if true otherwise rotate to player
    public Vector3 fixedEulerAngles = new Vector3(25f, 45f, 0f); //rotation angles for iso view
    private Vector3 velocity; // for tracking movement speed internally
    void LateUpdate() //moves camera to target smoothly
    {
        if (target == null) return;
        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(
            transform.position, //current pos
            desiredPosition,
            ref velocity,
            smoothTime //time it takes to smooth
        );
        if (lockRotation){
            transform.rotation = Quaternion.Euler(fixedEulerAngles); //set rotation for each frame
        }else{
           transform.LookAt(target);//alternative camera will always look at the player
            
        }
    }
}

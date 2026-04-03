using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyAnimator : MonoBehaviour
{
    private Animator animator;
    private Vector3 lastPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        bool isMoving = (transform.position - lastPosition).magnitude > 0.01f;
        animator.SetBool("isWalking", isMoving);
        lastPosition = transform.position;
        //checks if the distance travelled between frame 1 and frame 2 is more than 0 to detewrmine
        //if its moving. if it is, it triggers the animation

    }
}

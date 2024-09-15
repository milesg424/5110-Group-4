using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float Speed;
    [SerializeField] float JumpHeight;

    [HideInInspector] public int currentFacingDirection = 1;

    Rigidbody rb;
    Animator animator;
    Interactable lastInteracting;

    int currentWalkingDirection;

    private static PlayerController mInstance;
    public static PlayerController Instance { get { return mInstance; } }

    private void Awake()
    {
        if (mInstance != null && mInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            mInstance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        bool y = Input.GetKeyDown(KeyCode.Space);

        //animator.SetInteger("Walk", (int)x);
        //currentWalkingDirection = (int)x == 0 ? currentWalkingDirection : (int)x;
        //float temp = (currentFacingDirection) * 90 * currentDirection == -180 || 
        //    (currentFacingDirection) * 90 * currentDirection == 360 ?
        //    0 : (currentFacingDirection) * 90 * currentDirection == -360 ? 
        //    180 : (currentFacingDirection) * 90 * currentDirection;
        //transform.rotation = Quaternion.Euler(new Vector3(0, temp, 0));
        if (y)
        {
            rb.velocity = new Vector3(rb.velocity.x, JumpHeight, rb.velocity.z);
        }

        switch (currentFacingDirection)
        {
            case 1:
                rb.velocity = new Vector3(Speed * x, rb.velocity.y, 0);
                break;
            case 4:
                rb.velocity = new Vector3(0, rb.velocity.y, Speed * x);
                break;
            default:
                break;
        }

        CheckInteractable();
    }

    void CheckInteractable()
    {
        RaycastHit hit;
        Vector3 direction = Vector3.zero;
        switch (currentFacingDirection)
        {
            case 1:
                direction = Vector3.forward;
                break;
            case 4:
                direction = Vector3.left;
                break;
            default:
                break;
        }

        if (Physics.Raycast(transform.position, direction, out hit, 1000, LayerMask.GetMask("Interactable") | LayerMask.GetMask("Wall")))
        {
            Interactable interactable = hit.transform.GetComponent<Interactable>();
            if (interactable != null)
            {
                lastInteracting = interactable;
                lastInteracting.SetOutlineThickness(0.015f);
                if (Input.GetButtonDown("Interact"))
                {
                    interactable.Interact();
                }
            }
            else if (lastInteracting != null)
            {
                lastInteracting.SetOutlineThickness(0);
                lastInteracting = null;
            }

        }
        else if (lastInteracting != null)
        {
            lastInteracting.SetOutlineThickness(0);
            lastInteracting = null;
        }

    }

    void Dash()
    {
        if (currentFacingDirection == 2 || currentFacingDirection == 4)
        {

        }
    }
}

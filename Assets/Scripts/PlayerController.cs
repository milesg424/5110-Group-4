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

    int currentDirection;

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
        animator.SetInteger("Walk", (int)x);
        currentDirection = (int)x == 0 ? currentDirection : (int)x;

        float temp = (currentFacingDirection) * 90 * currentDirection == -180 || 
            (currentFacingDirection) * 90 * currentDirection == 360 ?
            0 : (currentFacingDirection) * 90 * currentDirection == -360 ? 
            180 : (currentFacingDirection) * 90 * currentDirection;
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
            case 2:
                rb.velocity = new Vector3(0, rb.velocity.y, -Speed * x);
                break;
            case 3:
                rb.velocity = new Vector3(-Speed * x, rb.velocity.y, 0);
                break;
            case 4:
                rb.velocity = new Vector3(0, rb.velocity.y, Speed * x);
                break;
            default:
                break;
        }
    }
}

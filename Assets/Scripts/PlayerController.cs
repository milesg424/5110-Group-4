using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Basic Stats---------")]
    [SerializeField] float Speed;
    [SerializeField] float JumpHeight;
    [SerializeField] float dashCD;

    [Header("Crash Wall Stats---------")]
    [SerializeField] float jumpAnimationTimer;
    [SerializeField] float jumpAnimationSpeed;
    [SerializeField] float numberOfRotation;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDistance;
    [SerializeField] float horizontalKnockBackForce;
    [SerializeField] float verticalKnockBackForce;


    [HideInInspector] public int currentFacingDirection = 1;

    Rigidbody rb;
    Animator animator;
    Interactable lastInteracting;

    bool canMove = true;
    bool isDashHitSomething;
    bool isDashing;
    float x;
    float mDashCD;
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
        if (canMove)
        {
            x = Input.GetAxisRaw("Horizontal");
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
            CrashWall();
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

        if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), direction, out hit, 1000, LayerMask.GetMask("Interactable") | LayerMask.GetMask("Wall")))
        {
            Interactable interactable = hit.transform.GetComponent<Interactable>();
            if (interactable != null)
            {
                if (lastInteracting != null && lastInteracting != this)
                {
                    lastInteracting.SetOutlineThickness(0);
                }
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

    void CrashWall()
    {
        if (currentFacingDirection == 4 && Input.GetButtonDown("CrashWall") && mDashCD <= 0)
        {
            if (x > 0)
            {
                mDashCD = dashCD;
                canMove = false;
                StartCoroutine(ICrashWall(1));
            }
            else if (x < 0)
            {
                mDashCD = dashCD;
                canMove = false;
                StartCoroutine(ICrashWall(-1));
            }
        }
        if (mDashCD > 0)
        {
            mDashCD = mDashCD - Time.deltaTime < 0 ? 0 : mDashCD - Time.deltaTime;
        }
    }

    IEnumerator ICrashWall(int direction)
    {
        //Jump and Rotate
        float timer = jumpAnimationTimer;
        float rot = numberOfRotation * 360 + 90 * direction;
        while (timer > 0)
        {
            timer = timer - Time.deltaTime < 0 ? 0 : timer - Time.deltaTime;
            rb.velocity = new Vector3(0, jumpAnimationSpeed * timer, jumpAnimationSpeed * timer * -direction);
            transform.rotation = Quaternion.Euler(new Vector3((jumpAnimationTimer - timer) / jumpAnimationTimer * rot, 0, 0));
            yield return new WaitForEndOfFrame();
        }
        transform.rotation = Quaternion.Euler(new Vector3(rot, 0, 0));

        //Dash
        rb.useGravity = false;
        yield return new WaitForSeconds(0.05f);
        isDashing = true;
        rb.velocity = new Vector3(0, 0, dashSpeed * direction);
        Vector3 curPos = transform.position;
        Vector3 targetPos = new Vector3(curPos.x, curPos.y, dashDistance * direction + curPos.z);
        if (direction > 0)
        {
            while (true)
            {
                if (transform.position.z - targetPos.z > 0)
                {
                    transform.position = targetPos;
                    break;
                }
                if (isDashHitSomething)
                {
                    isDashHitSomething = false;
                    rb.AddForce(new Vector3(0, verticalKnockBackForce, horizontalKnockBackForce * -direction), ForceMode.Impulse);
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
        }
        else if (direction < 0)
        {
            while (true)
            {
                if (transform.position.z - targetPos.z < 0)
                {
                    transform.position = targetPos;
                    break;
                }
                if (isDashHitSomething)
                {
                    isDashHitSomething = false;
                    rb.AddForce(new Vector3(0, verticalKnockBackForce, horizontalKnockBackForce * -direction), ForceMode.Impulse);
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
        }
        isDashing = false;

        //Reset Rotation and Regrant player control and reset momentum;
        rb.useGravity = true;
        //rb.velocity = Vector3.zero;
        canMove = true;
        while (Mathf.Abs(transform.rotation.eulerAngles.x) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * 20);
            yield return new WaitForEndOfFrame();

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isDashing && collision.gameObject.CompareTag("Breakable"))
        {
            //StartCoroutine(IHitStop());
            BreakableObject bo = collision.gameObject.GetComponent<BreakableObject>();
            bo.InstantiateParticle(transform.position + new Vector3(0, 0, 1) * x, Quaternion.identity);
            bo.Break();
            isDashHitSomething = true;
            
        }
        else if (isDashing && collision.gameObject.CompareTag("Wall"))
        {
            isDashHitSomething = true;
        }
    }

    IEnumerator IHitStop()
    {
        Time.timeScale = 0.02f;
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public int currentFacingDirection = 1;
    [HideInInspector] public bool isThirdPerson;

    public Action OnSwitchThirdPerson;
    public Action OnSwitchLockCamera;

    Rigidbody rb;
    Animator animator;
    Interactable lastInteracting;
    CameraController cam;
    GSettings settings;

    [HideInInspector] public bool canMove = true;
    bool isDashHitSomething;
    bool isDashing;
    float x;
    float z;
    float mDashCD;

    Coroutine moveTimerCoroutine;

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
        settings = GameManager.Instance.settings;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        cam = FindObjectOfType<CameraController>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            x = Input.GetAxisRaw("Horizontal");
            z = Input.GetAxisRaw("Vertical");
            bool jump = Input.GetKeyDown(KeyCode.Space);
            if (jump)
            {
                rb.velocity = new Vector3(rb.velocity.x, settings.playerJumpHeight, rb.velocity.z);
            }

            if (Input.GetKeyDown(KeyCode.X) && cam.canSwith3D)
            {
                if (!isThirdPerson)
                {
                    OnSwitchThirdPerson?.Invoke();
                }
                else
                {
                    OnSwitchLockCamera?.Invoke();
                    StartCoroutine(IResetRotation());
                }
                isThirdPerson = !isThirdPerson;
            }
            CrashWall();
        }
        CheckInteractable();

    }

    IEnumerator IResetRotation()
    {
        yield return new WaitForFixedUpdate();
        transform.rotation = Quaternion.identity;
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            if (isThirdPerson)
            {
                if (x != 0 || z != 0)
                {
                    transform.localRotation = Quaternion.Euler(0f, cam.transform.localRotation.eulerAngles.y, 0f);
                }
                rb.velocity = transform.forward * settings.playerSpeed * z + transform.right * settings.playerSpeed * x + new Vector3(0, rb.velocity.y, 0);
            }
            else
            {
                switch (currentFacingDirection)
                {
                    case 1:
                        rb.velocity = new Vector3(settings.playerSpeed * x, rb.velocity.y, 0);
                        break;
                    case 4:
                        rb.velocity = new Vector3(0, rb.velocity.y, settings.playerSpeed * x);
                        break;
                    default:
                        break;
                }
            }
        }

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
        Interactable interactable = null;

        if (isThirdPerson)
        {
            if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), transform.forward, out hit, 1000, LayerMask.GetMask("Interactable") | LayerMask.GetMask("Wall")))
            {
                interactable = hit.transform.GetComponent<Interactable>();
            }
        }
        else if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), direction, out hit, 1000, LayerMask.GetMask("Interactable") | LayerMask.GetMask("Wall")))
        {
            interactable = hit.transform.GetComponent<Interactable>();
        }

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

    void CrashWall()
    {
        if (Input.GetButtonDown("CrashWall"))
        {
            if (!isThirdPerson && currentFacingDirection == 4  && mDashCD <= 0)
            {
                StartCoroutine(ICrashWall());
            }
        }
        
        if (mDashCD > 0)
        {
            mDashCD = mDashCD - Time.deltaTime < 0 ? 0 : mDashCD - Time.deltaTime;
        }
    }

    IEnumerator ICrashWall()
    {
        int direction = x > 0 ? 1 : -1;
        mDashCD = settings.dashCD;
        canMove = false;
        //Jump and Rotate
        float timer = settings.jumpAnimationTimer;
        float rot = settings.numberOfRotation * 360 + 90 * direction;
        while (timer > 0)
        {
            timer = timer - Time.deltaTime < 0 ? 0 : timer - Time.deltaTime;
            rb.velocity = new Vector3(0, settings.jumpAnimationSpeed * timer, settings.jumpAnimationSpeed * timer * -direction);
            transform.rotation = Quaternion.Euler(new Vector3((settings.jumpAnimationTimer - timer) / settings.jumpAnimationTimer * rot, 0, 0));
            yield return new WaitForEndOfFrame();
        }
        transform.rotation = Quaternion.Euler(new Vector3(rot, 0, 0));

        //Dash
        rb.useGravity = false;
        yield return new WaitForSeconds(0.05f);
        isDashing = true;
        rb.velocity = new Vector3(0, 0, settings.dashSpeed * direction);
        Vector3 curPos = transform.position;
        Vector3 targetPos = new Vector3(curPos.x, curPos.y, settings.dashDistance * direction + curPos.z);
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
                    rb.AddForce(new Vector3(0, settings.verticalKnockBackForce, settings.horizontalKnockBackForce * -direction), ForceMode.Impulse);
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
                    rb.AddForce(new Vector3(0, settings.verticalKnockBackForce, settings.horizontalKnockBackForce * -direction), ForceMode.Impulse);
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

    public void SetPlayerCanMove(float timer)
    {
        if (moveTimerCoroutine != null)
        {
            StopCoroutine(moveTimerCoroutine);
        }
        moveTimerCoroutine = StartCoroutine(ISetPlayerCanMove(timer));
    }
    IEnumerator ISetPlayerCanMove(float timer)
    {
        canMove = false;
        yield return new WaitForSeconds(timer);
        canMove = true;
    }
}

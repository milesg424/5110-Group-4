using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
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
    bool isPreDashing;
    bool useGravity;
    float x;
    float z;
    float mDashCD;

    Coroutine moveTimerCoroutine;

    Vector3 externalForce;
    Vector3 movementForce;
    Vector3 constantForcee;
    Vector3 gravityForce;

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
        useGravity = true;
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
                externalForce = new Vector3(externalForce.x, settings.playerJumpHeight, externalForce.z);
                gravityForce = Vector3.zero;
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
                movementForce = transform.forward * settings.playerSpeed * z + transform.right * settings.playerSpeed * x;
            }
            else
            {
                switch (currentFacingDirection)
                {
                    case 1:
                        movementForce = new Vector3(settings.playerSpeed * x, 0, 0);
                        break;
                    case 4:
                        movementForce = new Vector3(0, 0, settings.playerSpeed * -x);
                        break;
                    default:
                        break;
                }
            }
        }
        else
        {
            movementForce = Vector3.zero;
        }

        if (!SameSign(movementForce.x, externalForce.x))
        {
            if (externalForce.x < 0)
            {
                externalForce = new Vector3(Mathf.Clamp(externalForce.x + Time.deltaTime * 5, externalForce.x, 0), externalForce.y, externalForce.z);
            }
            else
            {
                externalForce = new Vector3(Mathf.Clamp(externalForce.x - Time.deltaTime * 5, 0, externalForce.x), externalForce.y, externalForce.z);
            }
            movementForce = new Vector3(0, movementForce.y, movementForce.z);
        }
        if (!SameSign(movementForce.z, externalForce.z))
        {
            if (externalForce.z < 0)
            {
                externalForce = new Vector3(externalForce.x, externalForce.y, Mathf.Clamp(externalForce.z + Time.deltaTime * 5, externalForce.z, 0));
            }
            else
            {
                externalForce = new Vector3(externalForce.x, externalForce.y, Mathf.Clamp(externalForce.z - Time.deltaTime * 5, 0, externalForce.z));
            }

            movementForce = new Vector3(movementForce.x, movementForce.y, 0);
        }
        rb.velocity = movementForce + externalForce + constantForcee;
        if (useGravity)
        {
            if (externalForce.y > 0)
            {
                externalForce = new Vector3(externalForce.x, externalForce.y - Time.deltaTime * 15, externalForce.z);
                gravityForce = Vector3.zero;
            }
            else
            {
                gravityForce = Vector3.Lerp(gravityForce, Vector3.down * 30, Time.fixedDeltaTime * 2);
                rb.velocity += gravityForce;

            }
        }
        else
        {
            gravityForce = Vector3.zero;
        }
        externalForce = Vector3.Lerp(externalForce, Vector3.zero, Time.fixedDeltaTime * 10);
        if (externalForce.magnitude <= 0.1f)
        {
            externalForce = Vector3.zero;
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
                direction = Vector3.right;
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
        isPreDashing = true;
        externalForce = Vector3.zero;
        int direction = x > 0 ? -1 : 1;
        mDashCD = settings.dashCD;
        canMove = false;
        //Jump and Rotate
        useGravity = false;
        float timer = settings.jumpAnimationTimer;
        float rot = settings.numberOfRotation * 360 + 90 * direction;
        while (timer > 0)
        {
            timer = timer - Time.deltaTime < 0 ? 0 : timer - Time.deltaTime;
            constantForcee = new Vector3(0, settings.jumpAnimationSpeed * timer, settings.jumpAnimationSpeed * timer * -direction);
            transform.rotation = Quaternion.Euler(new Vector3((settings.jumpAnimationTimer - timer) / settings.jumpAnimationTimer * rot, 0, 0));
            yield return new WaitForEndOfFrame();
        }
        transform.rotation = Quaternion.Euler(new Vector3(rot, 0, 0));

        //Dash
        yield return new WaitForSeconds(0.05f);
        isDashing = true;
        //rb.velocity = new Vector3(0, 0, settings.dashSpeed * direction);
        constantForcee = new Vector3(0, 0, settings.dashSpeed * direction);
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
                    externalForce = new Vector3(0, settings.verticalKnockBackForce, settings.horizontalKnockBackForce * -direction);
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
                    externalForce = new Vector3(0, settings.verticalKnockBackForce, settings.horizontalKnockBackForce * -direction);
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
        }
        constantForcee = Vector3.zero;
        isDashing = false;
        isPreDashing = false;

        //Reset Rotation and Regrant player control and reset momentum;
        useGravity = true;
        canMove = true;
        while (Mathf.Abs(transform.rotation.eulerAngles.x) > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * 20);
            yield return new WaitForEndOfFrame();

        }
        transform.rotation = Quaternion.identity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isDashing)
        {
            if (isDashing && collision.gameObject.CompareTag("Breakable"))
            {
                //StartCoroutine(IHitStop());
                BreakableObject bo = collision.gameObject.GetComponent<BreakableObject>();
                bo.InstantiateParticle(transform.position + new Vector3(0, 0, 1) * -x, Quaternion.identity);
                bo.Break();
                isDashHitSomething = true;

            }
            else if (isDashing && collision.gameObject.CompareTag("Wall"))
            {
                isDashHitSomething = true;
            }
        }
        else if(!isPreDashing)
        {
            if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Breakable"))
            {
                if (collision.contacts[0].normal.y == 0)
                {
                    WalkIntoWalls();
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            gravityForce = Vector3.zero;
        }
        else if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Breakable"))
        {
            if (collision.contacts[0].normal.y > 0)
            {
                gravityForce = Vector3.zero;
            }
        }
    }

    IEnumerator IHitStop()
    {
        Time.timeScale = 0.02f;
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1;
    }

    void WalkIntoWalls()
    {
        if (!isThirdPerson)
        {
            if (currentFacingDirection == 1)
            {
                externalForce = new Vector3(settings.horzontalForce * -x, externalForce.y + settings.verticalForce, externalForce.z);
            }
            else
            {
                externalForce = new Vector3(externalForce.x, externalForce.y + settings.verticalForce, settings.horzontalForce * x);
            }
        }

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

    bool SameSign(int x, int y)
    {
        if (x < 0 && y > 0)
        {
            return false;
        }
        else if (x > 0 && y < 0)
        {
            return false;
        }
        return true;
    }
    bool SameSign(float x, float y)
    {
        if (x < 0 && y > 0)
        {
            return false;
        }
        else if (x > 0 && y < 0)
        {
            return false;
        }
        return true;
    }
}

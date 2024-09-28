using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class LightSource : Interactable
{
    [SerializeField] float lightIntensityBeforeInteract;
    [SerializeField] float targetLightIntensity;
    [SerializeField] Color color;
    [SerializeField] Light lightSource;
    [SerializeField] Transform targetPos;

    [HideInInspector] public bool isUseMaxDistance = true;

    public List<Transform> relayPoints;

    Material mt;
    Rigidbody rb;
    AudioSource audioSource;
    GSettings settings;
    Coroutine floatingCoroutine;

    bool bIsInteracted;
    bool isFinishedInteraction;
    bool isHiding;
    bool isHided;
    float currentIntensity;
    int currentRelayPoint = 0;
    public int moveDirection;
    public bool doHide;

    float lastAmount;

    // Start is called before the first frame update
    protected override void Start()
    {
        settings = GameManager.Instance.settings;
        mt = transform.Find("EmissionSphere").GetComponent<MeshRenderer>().material;
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = settings.lightSourceClip;
        SetOverallIntensity(lightIntensityBeforeInteract);
        currentIntensity = lightIntensityBeforeInteract;
        //floatingCoroutine = StartCoroutine(IFloating());
        doHide = true;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    private void LateUpdate()
    {
        if (!isHided)
        {
            Vector3 posWithoutModified = Vector3.zero;
            float curFloatAmount = Mathf.Sin(Time.time * 1.2f) * 0.2f;
            transform.position = new Vector3(transform.position.x, transform.position.y + curFloatAmount - lastAmount, transform.position.z);
            lastAmount = curFloatAmount;
        }
        //if (rb.velocity.magnitude < 0.1f)
        //{
        //    audioSource.volume = Mathf.Lerp(audioSource.volume, 0, Time.deltaTime * 5);
        //    if (audioSource.volume < 0.05f)
        //    {
        //        audioSource.volume = 0;
        //    }
        //}
        //else
        //{
        //    if (!audioSource.isPlaying)
        //    {
        //        audioSource.UnPause();
        //    }
        //    audioSource.volume = Mathf.Lerp(audioSource.volume, 1, Time.deltaTime * 2);
        //}

        audioSource.volume = Mathf.Lerp(audioSource.volume, rb.velocity.magnitude / settings.lightSourceMoveSpeed, Time.deltaTime * 5);
    }
    public override void SetOutlineThickness(float thickness)
    {
        if (!bIsInteracted)
        {
            mt.SetInt("_Interact", thickness > 0 ? 1 : 0);
        }
    }
    public override void Interact()
    {
        if (!bIsInteracted)
        {
            audioSource.Play();
            OnInteract?.Invoke();
            bIsInteracted = true;
            mt.SetInt("_Interact", 0);
            StartCoroutine(IInteract(1));
            StartCoroutine(ISmoothLightUp(targetLightIntensity));
        }
    }

    public void InteractNoWaitTime()
    {
        if (!bIsInteracted)
        {
            audioSource.Play();
            OnInteract?.Invoke();
            bIsInteracted = true;
            mt.SetInt("_Interact", 0);
            StartCoroutine(IInteract(0));
            StartCoroutine(ISmoothLightUp(targetLightIntensity));
        }
    }

    void SetOverallIntensity(float intensity)
    {
        SetMateralIntensity(intensity);
        SetLightIntensity(intensity * 2);
        SetLightRange(intensity / 3);
    }

    //IEnumerator IFloating()
    //{
    //    float temp = 0;
    //    Vector3 posWithoutModified = Vector3.zero;
    //    float y = transform.position.y;
    //    while (true)
    //    {
    //        temp += Time.deltaTime;
    //        transform.position = new Vector3(transform.position.x, y + Mathf.Sin(temp * 1.2f) * 0.2f, transform.position.z);
    //        yield return new WaitForEndOfFrame();
    //    }
    //}

    void SetMaterialColor(Color color)
    {
        mt.SetColor("_Color", color);
    }

    void SetMateralIntensity(float intensity)
    {
        mt.SetColor("_Color", color * intensity);
    }

    void SetLightIntensity(float intensity)
    {
        lightSource.intensity = intensity;
    }

    void SetLightRange(float range)
    {
        lightSource.range = range;
    }

    IEnumerator IInteract(float waitTime)
    {

        yield return new WaitForSeconds(waitTime);
        if (moveDirection == 0)
        {
            while (targetPos.position.x - transform.position.x > 0.1f)
            {
                if (relayPoints != null && relayPoints.Count > 0 && currentRelayPoint < relayPoints.Count)
                {
                    if (Mathf.Abs(transform.position.x - relayPoints[currentRelayPoint].position.x) < 5)
                    {
                        if (!isHiding)
                        {
                            isHiding = true;
                            //StopCoroutine(floatingCoroutine);
                            StartCoroutine(IDisapear());
                        }

                        if (Vector3.Distance(new Vector3(relayPoints[currentRelayPoint].position.x, relayPoints[currentRelayPoint].position.y, 0), new Vector3(transform.position.x, transform.position.y, 0)) < 0.1f)
                        {
                            rb.velocity = Vector3.zero;
                            yield return new WaitForEndOfFrame();
                            continue;
                        }
                        else
                        {
                            Vector3 dir = new Vector3(relayPoints[currentRelayPoint].position.x, relayPoints[currentRelayPoint].position.y, 0) - new Vector3(transform.position.x, transform.position.y, 0);
                            dir = dir.normalized;
                            rb.velocity = Vector3.Lerp(rb.velocity, dir * settings.lightSourceMoveSpeed, Time.deltaTime * 2);
                            yield return new WaitForEndOfFrame();
                            continue;
                        }
                    }
                }
                if (isUseMaxDistance && transform.position.x - PlayerController.Instance.transform.position.x > settings.maxDistanceBetweenPlayer)
                {
                    rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * 2);
                }
                else
                {
                    Vector3 dir = new Vector3(targetPos.position.x, targetPos.position.y, 0) - new Vector3(transform.position.x, transform.position.y, 0);
                    dir = dir.normalized;
                    rb.velocity = Vector3.Lerp(rb.velocity, dir * settings.lightSourceMoveSpeed, Time.deltaTime * 2);
                }
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (targetPos.position.z - transform.position.z < -0.1f)
            {
                if (relayPoints != null && relayPoints.Count > 0 && currentRelayPoint < relayPoints.Count)
                {
                    if (Mathf.Abs(transform.position.z - relayPoints[currentRelayPoint].position.z) < 5)
                    {
                        if (!isHiding && doHide)
                        {
                            isHiding = true;
                            //StopCoroutine(floatingCoroutine);
                            StartCoroutine(IDisapear());
                        }

                        if (Vector3.Distance(new Vector3(0, relayPoints[currentRelayPoint].position.y, relayPoints[currentRelayPoint].position.z), new Vector3(0, transform.position.y, transform.position.z)) < 1f)
                        {
                            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * 5);
                            //rb.velocity = Vector3.zero;
                            yield return new WaitForEndOfFrame();
                            continue;
                        }
                        else
                        {
                            Vector3 dir = new Vector3(0, relayPoints[currentRelayPoint].position.y, relayPoints[currentRelayPoint].position.z) - new Vector3(0, transform.position.y, transform.position.z);
                            dir = dir.normalized;
                            rb.velocity = Vector3.Lerp(rb.velocity, dir * settings.lightSourceMoveSpeed, Time.deltaTime * 2);
                            yield return new WaitForEndOfFrame();
                            continue;
                        }
                    }
                }
                if (isUseMaxDistance && transform.position.z - PlayerController.Instance.transform.position.z < -settings.maxDistanceBetweenPlayer)
                {
                    rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * 2);
                }
                else
                {
                    Vector3 dir = new Vector3(0, targetPos.position.y, targetPos.position.z) - new Vector3(0, transform.position.y, transform.position.z);
                    dir = dir.normalized;
                    rb.velocity = Vector3.Lerp(rb.velocity, dir * settings.lightSourceMoveSpeed, Time.deltaTime * 2);
                }
                yield return new WaitForEndOfFrame();
            }
        }


        while (rb.velocity.magnitude > 0.1f)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * 2);
            yield return new WaitForEndOfFrame();

        }
        rb.velocity = Vector3.zero;
    }

    IEnumerator ISmoothLightUp(float target)
    {
        float temp = currentIntensity;
        while (Mathf.Abs(temp - target) > 0.1f)
        {
            temp = Mathf.Lerp(temp, target, Time.deltaTime * 1f);
            currentIntensity = temp;
            SetOverallIntensity(temp);
            yield return new WaitForEndOfFrame();
        }
        SetOverallIntensity(target);
        currentIntensity = target;

    }

    IEnumerator IDisapear()
    {
        float size = 1;
        while (size > 0.02f)
        {
            size = Mathf.Lerp(size, 0, Time.deltaTime);
            transform.localScale = Vector3.one * size;
            yield return new WaitForEndOfFrame();
        }
        transform.localScale *= 0;
        isHided = true;
    }

    IEnumerator IShow()
    {
        isHided = false;

        while (transform.localScale.x < 0.95f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * 1.5f);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.5f);
        currentRelayPoint++;
        isHiding = false;
    }

    public void GoToNextRelayPoint()
    {
        StartCoroutine(IShow());
    }
}

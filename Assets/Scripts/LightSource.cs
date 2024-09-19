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

    [SerializeField] List<Transform> relayPoints;

    [HideInInspector] public bool isUseMaxDistance = true;

    Material mt;
    Rigidbody rb;
    GSettings settings;
    Coroutine floatingCoroutine;

    bool bIsInteracted;
    bool isFinishedInteraction;
    bool isHiding;
    bool isHided;
    float currentIntensity;
    int currentRelayPoint = 0;

    float lastAmount;

    // Start is called before the first frame update
    protected override void Start()
    {
        settings = GameManager.Instance.settings;
        mt = transform.Find("EmissionSphere").GetComponent<MeshRenderer>().material;
        rb = GetComponent<Rigidbody>();
        SetOverallIntensity(lightIntensityBeforeInteract);
        currentIntensity = lightIntensityBeforeInteract;
        //floatingCoroutine = StartCoroutine(IFloating());

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.V) && isHided)
        {
            StartCoroutine(IShow());
        }
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
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(0.5f);
        currentRelayPoint++;
        isHiding = false;
    }
}

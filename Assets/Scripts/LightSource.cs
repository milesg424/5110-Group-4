using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LightSource : Interactable
{
    [SerializeField] float lightIntensityBeforeInteract;
    [SerializeField] float targetLightIntensity;
    [SerializeField] float moveSpeed;
    [SerializeField] float maxDistanceBetweenPlayer;
    [SerializeField] Color color;
    [SerializeField] Light lightSource;
    [SerializeField] Transform targetPos;

    [HideInInspector] public bool isUseMaxDistance = true;

    Material mt;
    Rigidbody rb;

    bool bIsInteracted;
    bool isFinishedInteraction;
    float currentIntensity;

    // Start is called before the first frame update
    protected override void Start()
    {
        mt = transform.Find("EmissionSphere").GetComponent<MeshRenderer>().material;
        rb = GetComponent<Rigidbody>();
        SetOverallIntensity(lightIntensityBeforeInteract);
        currentIntensity = lightIntensityBeforeInteract;
        StartCoroutine(IFloating());

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
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
            StartCoroutine(IInteract());
            StartCoroutine(ISmoothLightUp(targetLightIntensity));
        }
    }

    void SetOverallIntensity(float intensity)
    {
        SetMateralIntensity(intensity);
        SetLightIntensity(intensity * 2);
        SetLightRange(intensity / 3);
    }

    IEnumerator IFloating()
    {
        float temp = 0;
        Vector3 posWithoutModified = Vector3.zero;
        float y = transform.position.y;
        while (true)
        {
            temp += Time.deltaTime;
            transform.position = new Vector3(transform.position.x, y + Mathf.Sin(temp * 1.2f) * 0.2f, transform.position.z);
            yield return new WaitForEndOfFrame();
        }
    }

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

    IEnumerator IInteract()
    {
        yield return new WaitForSeconds(1);
        while (targetPos.position.x - transform.position.x > 0.1f)
        {

            if (isUseMaxDistance && transform.position.x - PlayerController.Instance.transform.position.x > maxDistanceBetweenPlayer)
            {
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * 2);
            }
            else
            {
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.right * moveSpeed, Time.deltaTime * 2);
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
}

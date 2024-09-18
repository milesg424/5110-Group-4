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
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        //if (transform.position.x - PlayerController.Instance.transform.position.x > maxDistanceBetweenPlayer)
        //{

        //}
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
        while (targetPos.position.z - transform.position.z > 0.1f)
        {

            if (transform.position.z - PlayerController.Instance.transform.position.z > maxDistanceBetweenPlayer)
            {
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * 2);
            }
            else
            {
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.forward * moveSpeed, Time.deltaTime * 2);
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

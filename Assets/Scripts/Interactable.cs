using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    Material outLine;
    public Action OnInteract;
    public Action OnEnter;

    protected bool isInteracting;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        if (GetComponent<MeshRenderer>() != null && GetComponent<MeshRenderer>().materials.Length >= 2)
        {
            outLine = GetComponent<MeshRenderer>().materials[1];
        }
        SetOutlineThickness(0);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (Input.GetButtonDown("Interact") && isInteracting)
        {
            Interact();
        }
    }

    public virtual void Interact()
    {
        Debug.Log("Interacting");
    }

    public virtual void SetOutlineThickness(float thickness)
    {
        if (outLine != null)
        {
            outLine.SetFloat("_OutlineThickness", thickness);
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetOutlineThickness(0.015f);
            OnEnter?.Invoke();
            isInteracting = true;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetOutlineThickness(0);
            isInteracting = false;
        }
    }
}

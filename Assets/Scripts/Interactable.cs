using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    Material outLine;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        outLine = GetComponent<MeshRenderer>().materials[1];
        SetOutlineThickness(0);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
    }

    public virtual void Interact()
    {
        Debug.Log("Interacting");
    }

    public void SetOutlineThickness(float thickness)
    {
        outLine.SetFloat("_OutlineThickness", thickness);
    }
}

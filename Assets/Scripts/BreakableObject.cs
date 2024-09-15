using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] VisualEffect breakEffect;

    public void InstantiateParticle(Vector3 pos, Quaternion rot)
    {
        breakEffect.gameObject.SetActive(true);
        breakEffect.transform.SetParent(null);
        breakEffect.transform.position = pos;
        breakEffect.transform.rotation = rot;
        Destroy(breakEffect.gameObject, 1);
    }

    public void Break()
    {
        Destroy(gameObject);
    }
}

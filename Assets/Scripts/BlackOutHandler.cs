using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackOutHandler : MonoBehaviour
{
    [SerializeField] Blit rf;
    [SerializeField] Material blackMat;
    [HideInInspector] public Material material;


    // Start is called before the first frame update
    void Start()
    {
        material = new Material(blackMat);
        rf.settings.blitMaterial = material;
    }

    public void SetAlpha(float alpha)
    {
        material.SetFloat("_Alpha", alpha);
    }

    public float GetAlpha()
    {
        return material.GetFloat("_Alpha");
    }



    public void SetPosition(int index, Vector3 pos)
    {
        string temp = "_Pos" + index.ToString();
        material.SetVector(temp, pos);
    }

    public Vector3 GetPosition(int index)
    {
        string temp = "_Pos" + index.ToString();
        return material.GetVector(temp);
    }



    public void SetRange(int index, float range)
    {
        string temp = "_Size" + index.ToString();
        material.SetFloat(temp, range);
    }

    public float GetRange(int index)
    {
        string temp = "_Size" + index.ToString();
        return material.GetFloat(temp);
    }

    public void FirstLevelInit()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BreakableObject : MonoBehaviour
{
    [SerializeField] VisualEffect breakEffect;
    public int cubesPerAxis = 10;
    public float delay = 1f;
    public float force = 300f;
    public float radius = 2f;
    public Vector3 cubeScale;
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
        GetComponent<Renderer>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
        for (int i = 0; i < cubesPerAxis; i++)
        {
            for (int j = 0; j < cubesPerAxis; j++)
            {
                for (int k = 0; k < cubesPerAxis; k++)
                {
                    CreatCube(new Vector3(i, j, k));
                }
            }
        }
    }

  
    void CreatCube(Vector3 position)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Renderer rd = cube.GetComponent<Renderer>();
        rd.material = GetComponent<Renderer>().material;
        cube.transform.localScale = cubeScale;
        Vector3 firstCube = transform.position - cubeScale / 2 + cube.transform.localScale / 2;
        cube.transform.position = firstCube + Vector3.Scale(position, cube.transform.localScale);
        Rigidbody rb = cube.AddComponent<Rigidbody>();
        rb.AddExplosionForce(force, transform.position, radius);

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Break();

        }
    }
}

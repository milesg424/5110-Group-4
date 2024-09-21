using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class SeeThroughHandler : MonoBehaviour
{
    BoxCollider col;
    CameraController cc;
    List<Material> obstacleMats;
    List<Material> outMats;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider>();
        cc = FindObjectOfType<CameraController>();
        obstacleMats = new List<Material>();
        outMats = new List<Material>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = cc.transform.position;
        col.center = new Vector3(0, 0, Vector3.Distance(PlayerController.Instance.transform.position, cc.transform.position) / 2);
        col.size = new Vector3(0.1f, 0.1f, Vector3.Distance(PlayerController.Instance.transform.position, cc.transform.position));
        Quaternion rot = Quaternion.LookRotation(PlayerController.Instance.transform.position - transform.position);
        transform.rotation = rot;

        foreach (var item in obstacleMats)
        {
            Color origin = item.GetColor("_BaseColor");
            float alpha = origin.a;
            alpha = Mathf.Lerp(alpha, 0.2f, Time.deltaTime);
            item.SetColor("_BaseColor", new Color(origin.r, origin.g, origin.b, alpha));
        }

        for (int i = outMats.Count - 1; i >= 0; i--)
        {
            Color origin = outMats[i].GetColor("_BaseColor");
            float alpha = origin.a;

            if (alpha > 0.95f)
            {
                alpha = 1;
                outMats[i].SetColor("_BaseColor", new Color(origin.r, origin.g, origin.b, 1));
                outMats.Remove(outMats[i]);
                continue;
            }

            alpha = Mathf.Lerp(alpha, 1, Time.deltaTime * 2);
            outMats[i].SetColor("_BaseColor", new Color(origin.r, origin.g, origin.b, alpha));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("Breakable"))
        {
            if (!obstacleMats.Contains(other.GetComponent<MeshRenderer>().material))
            {
                obstacleMats.Add(other.GetComponent<MeshRenderer>().material);
                if (outMats.Contains(other.GetComponent<MeshRenderer>().material))
                {
                    outMats.Remove(other.GetComponent<MeshRenderer>().material);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("Breakable"))
        {
            if (obstacleMats.Contains(other.GetComponent<MeshRenderer>().material))
            {
                obstacleMats.Remove(other.GetComponent<MeshRenderer>().material);
                outMats.Add(other.GetComponent<MeshRenderer>().material);
            }
        }
    }
}

using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class SeeThroughHandler : MonoBehaviour
{
    BoxCollider col;
    CameraController cc;

    List<MeshRenderer> obstacleMats;
    List<MeshRenderer> outMats;

    float nearPlane;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider>();
        cc = FindObjectOfType<CameraController>();
        obstacleMats = new List<MeshRenderer>();
        outMats = new List<MeshRenderer>();

        nearPlane = cc.GetComponent<CinemachineVirtualCamera>().m_Lens.NearClipPlane;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerController.Instance.isThirdPerson)
        {
            Vector3 temp = PlayerController.Instance.transform.position + new Vector3(0, 0, -nearPlane);
            col.center = new Vector3(0, 0, nearPlane);
            col.size = new Vector3(0.1f, 0.1f, Vector3.Distance(PlayerController.Instance.transform.position, temp) * 2);
            transform.position = PlayerController.Instance.transform.position + new Vector3(0, 1, 0);


            //Vector3 screenCenter = Camera.main.ScreenToWorldPoint(Camera.main.WorldToScreenPoint(PlayerController.Instance.transform.position));
            //transform.position = cc.transform.position;
            //col.center = new Vector3(0, 0, Vector3.Distance(PlayerController.Instance.transform.position, screenCenter) / 2);
            //col.size = new Vector3(0.1f, 0.1f, Vector3.Distance(PlayerController.Instance.transform.position, cc.transform.position));
            Quaternion rot = Quaternion.Euler(new Vector3(0, cc.transform.rotation.eulerAngles.y, 0));
            transform.rotation = rot;
        }


        foreach (var item in obstacleMats)
        {
            float alpha = item.materials[0].GetFloat("_Alpha");
            alpha = Mathf.Lerp(alpha, 0.2f, Time.deltaTime);
            item.materials[0].SetFloat("_Alpha", alpha);
            //item.materials[1].SetFloat("_Alpha", alpha);
        }

        for (int i = outMats.Count - 1; i >= 0; i--)
        {
            float alpha = outMats[i].materials[0].GetFloat("_Alpha");

            if (alpha > 0.95f)
            {
                outMats[i].materials[0].SetFloat("_Alpha", 1);
                //outMats[i].materials[1].SetFloat("_Alpha", 1);

                outMats.Remove(outMats[i]);
                continue;
            }

            alpha = Mathf.Lerp(alpha, 1, Time.deltaTime * 2);
            outMats[i].materials[0].SetFloat("_Alpha", alpha);
            //outMats[i].materials[1].SetFloat("_Alpha", alpha);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("Breakable"))
        {
            if (!obstacleMats.Contains(other.GetComponent<MeshRenderer>()))
            {
                obstacleMats.Add(other.GetComponent<MeshRenderer>());
                if (outMats.Contains(other.GetComponent<MeshRenderer>()))
                {
                    outMats.Remove(other.GetComponent<MeshRenderer>());
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wall") || other.CompareTag("Breakable"))
        {
            if (obstacleMats.Contains(other.GetComponent<MeshRenderer>()))
            {
                obstacleMats.Remove(other.GetComponent<MeshRenderer>());
                outMats.Add(other.GetComponent<MeshRenderer>());
            }
        }
    }
}

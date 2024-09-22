using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDistanceCheck : MonoBehaviour
{
    PlayerController pc;
    CameraController cc;
    Material mt;
    MeshRenderer renderer;
    Color origin;
    float extendX;
    float extendZ;

    static bool isFirstCollider;
    // Start is called before the first frame update

    private void Awake()
    {
        WallDistanceCheck.isFirstCollider = true;
    }
    void Start()
    {
        pc = PlayerController.Instance;
        cc = FindObjectOfType<CameraController>();
        renderer = GetComponent<MeshRenderer>();
        mt = renderer.material;
        origin = mt.GetColor("_Color");
        extendX = renderer.bounds.extents.x;
        extendZ = renderer.bounds.extents.z;

    }

    // Update is called once per frame
    void Update()
    {
        if (pc.currentFacingDirection == 1)
          {
            float actualSize = transform.position.z - extendZ;
            float actualCam = (cc.transform.position.z + cc.vCam.m_Lens.NearClipPlane) / 2;
            float multiplier = map(actualSize, actualCam, actualCam + 20, 0.05f, 1);
            multiplier = 1 - multiplier;
            mt.SetColor("_Color", origin * multiplier);
        }
        if (pc.currentFacingDirection == 4)
        {
            float actualSize = transform.position.x - extendX / 2;
            float actualCam = cc.transform.position.x;
            float multiplier = map(actualSize, actualCam, actualCam + 20, 0.05f, 1);
            multiplier = 1 - multiplier;
            mt.SetColor("_Color", origin * multiplier);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (isFirstCollider)
            {
                WallDistanceCheck.isFirstCollider = false;
                UIManager.Instance.PopQE(3);
            }
        }
    }

    float map(float s, float a1, float a2, float b1, float b2)
    {
        s = Mathf.Clamp(s, a1, a2);
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}

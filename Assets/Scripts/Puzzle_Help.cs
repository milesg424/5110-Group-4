using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle_Help : MonoBehaviour
{
    Transform H;
    Transform E;
    Transform L;
    Transform P;

    Transform H_;
    Transform E_;
    Transform L_;
    Transform P_;

    Transform p1;
    Transform p2;
    Transform p3;
    Transform p4;

    float timer = 0.8f;
    float timer1;
    float timer2;
    float timer3;
    float timer4;

    bool _1Solved;
    bool _2Solved;
    bool _3Solved;
    bool _4Solved;

    Transform shadow;
    // Start is called before the first frame update
    void Start()
    {
        H = transform.Find("H");
        E = transform.Find("E");
        L = transform.Find("L");
        P = transform.Find("P");

        H_ = transform.Find("H_");
        E_ = transform.Find("E_");
        L_ = transform.Find("L_");
        P_ = transform.Find("P_");

        p1 = transform.Find("Point1");
        p2 = transform.Find("Point2");
        p3 = transform.Find("Point3");
        p4 = transform.Find("Point4");

        timer1 = timer;
        timer2 = timer;
        timer3 = timer;
        timer4 = timer;

        shadow = GameObject.FindWithTag("FakeShadow").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_1Solved)
        {
            CheckPlayerOnPosition(1);
        }
        if (!_2Solved)
        {
            CheckPlayerOnPosition(2);
        }
        if (!_3Solved)
        {
            CheckPlayerOnPosition(3);
        }
        if (!_4Solved)
        {
            CheckPlayerOnPosition(4);
        }
    }

    void CheckPlayerOnPosition(int index)
    {
        switch (index)
        {
            case 1:
                if (Vector3.Distance(new Vector3(shadow.position.x, shadow.position.y, 0), new Vector3(p1.transform.position.x, p1.transform.position.y, 0)) < 0.2f)
                {
                    timer1 -= Time.deltaTime;
                    if (timer1 <= 0)
                    {
                        _1Solved = true;
                        H.gameObject.SetActive(true);
                    }
                }
                else
                {
                    timer1 = timer;
                }
                break;
            case 2:
                if (Vector3.Distance(new Vector3(shadow.position.x, shadow.position.y, 0), new Vector3(p2.transform.position.x, p2.transform.position.y, 0)) < 0.2f)
                {
                    timer2 -= Time.deltaTime;
                    if (timer2 <= 0)
                    {
                        _2Solved = true;
                        E.gameObject.SetActive(true);
                    }
                }
                else
                {
                    timer2 = timer;
                }
                break;
            case 3:
                if (Vector3.Distance(new Vector3(shadow.position.x, shadow.position.y, 0), new Vector3(p3.transform.position.x, p3.transform.position.y, 0)) < 0.2f)
                {
                    timer3 -= Time.deltaTime;
                    if (timer3 <= 0)
                    {
                        _3Solved = true;
                        L.gameObject.SetActive(true);
                    }
                }
                else
                {
                    timer3 = timer;
                }
                break;
            case 4:
                if (Vector3.Distance(new Vector3(shadow.position.x, shadow.position.y, 0), new Vector3(p4.transform.position.x, p4.transform.position.y, 0)) < 0.2f)
                {
                    timer4 -= Time.deltaTime;
                    if (timer4 <= 0)
                    {
                        _4Solved = true;
                        P.gameObject.SetActive(true);
                    }
                }
                else
                {
                    timer4 = timer;
                }
                break;
        }
    }
}

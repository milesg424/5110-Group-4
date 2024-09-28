using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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

    float timer;
    float timer1;
    float timer2;
    float timer3;
    float timer4;

    bool _1Solved;
    bool _2Solved;
    bool _3Solved;
    bool _4Solved;

    bool puzzleComplete;

    public Action OnComplete;
    public Action OnOneCharacterComplete;
    GSettings settings;

    Transform shadow;
    // Start is called before the first frame update
    void Start()
    {
        settings = GameManager.Instance.settings;

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

        timer = settings.help_Timer;
        timer1 = timer;
        timer2 = timer;
        timer3 = timer;
        timer4 = timer;

        shadow = GameObject.FindWithTag("FakeShadow").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (!puzzleComplete)
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

            if (_1Solved && _2Solved && _3Solved && _4Solved)
            {
                puzzleComplete = true;
                OnComplete?.Invoke();
                StartCoroutine(IPlayerClip());
            }
        }

    }

    void CheckPlayerOnPosition(int index)
    {
        switch (index)
        {
            case 1:
                if (Vector3.Distance(new Vector3(shadow.position.x, shadow.position.y, 0), new Vector3(p1.transform.position.x, p1.transform.position.y, 0)) < settings.help_Offset)
                {
                    timer1 -= Time.deltaTime;
                    if (timer1 <= 0)
                    {
                        _1Solved = true;
                        StartCoroutine(IShow(1));
                        GameManager.Instance.PlaySound(GameManager.Instance.settings.HClip);
                    }
                }
                else
                {
                    timer1 = timer;
                }
                break;
            case 2:
                if (Vector3.Distance(new Vector3(shadow.position.x, shadow.position.y, 0), new Vector3(p2.transform.position.x, p2.transform.position.y, 0)) < settings.help_Offset)
                {
                    timer2 -= Time.deltaTime;
                    if (timer2 <= 0)
                    {
                        _2Solved = true;
                        StartCoroutine(IShow(2));
                        GameManager.Instance.PlaySound(GameManager.Instance.settings.EClip);
                    }
                }
                else
                {
                    timer2 = timer;
                }
                break;
            case 3:
                if (Vector3.Distance(new Vector3(shadow.position.x, shadow.position.y, 0), new Vector3(p3.transform.position.x, p3.transform.position.y, 0)) < settings.help_Offset)
                {
                    timer3 -= Time.deltaTime;
                    if (timer3 <= 0)
                    {
                        _3Solved = true;
                        StartCoroutine(IShow(3));
                        GameManager.Instance.PlaySound(GameManager.Instance.settings.LClip);
                    }
                }
                else
                {
                    timer3 = timer;
                }
                break;
            case 4:
                if (Vector3.Distance(new Vector3(shadow.position.x, shadow.position.y, 0), new Vector3(p4.transform.position.x, p4.transform.position.y, 0)) < settings.help_Offset)
                {
                    timer4 -= Time.deltaTime;
                    if (timer4 <= 0)
                    {
                        _4Solved = true;
                        StartCoroutine(IShow(4));
                        GameManager.Instance.PlaySound(GameManager.Instance.settings.PClip);
                    }
                }
                else
                {
                    timer4 = timer;
                }
                break;
        }
    }

    IEnumerator IShow(int index)
    {
        OnOneCharacterComplete?.Invoke();
        float strength = 1;
        Material mat = null;
        switch (index)
        {
            case 1:
                mat = H.GetComponent<SpriteRenderer>().material;
                H.gameObject.SetActive(true);
                break;
            case 2:
                mat = E.GetComponent<SpriteRenderer>().material;
                E.gameObject.SetActive(true);
                break;
            case 3:
                mat = L.GetComponent<SpriteRenderer>().material;
                L.gameObject.SetActive(true);
                break;
            case 4:
                mat = P.GetComponent<SpriteRenderer>().material;
                P.gameObject.SetActive(true);
                break;
        }
        StartCoroutine(IHide(index));

        mat.SetFloat("_DissolveStrength", strength);
        while (strength > 0.05f)
        {
            strength = Mathf.Lerp(strength, 0, Time.deltaTime * 2);
            mat.SetFloat("_DissolveStrength", strength);
            yield return new WaitForEndOfFrame();
        }
        mat.SetFloat("_DissolveStrength", 0);
    }

    IEnumerator IHide(int index)
    {
        SpriteRenderer sr = null;
        switch (index)
        {
            case 1:
                sr = H_.GetComponent<SpriteRenderer>();
                break;
            case 2:
                sr = E_.GetComponent<SpriteRenderer>();
                break;
            case 3:
                sr = L_.GetComponent<SpriteRenderer>();
                break;
            case 4:
                sr = P_.GetComponent<SpriteRenderer>();
                break;
        }

        Color col = sr.color;
        float alpha = col.a;
        while (alpha > 0.05f)
        {
            alpha = Mathf.Lerp(alpha, 0, Time.deltaTime * 2);
            sr.color = new Color(col.r, col.g, col.b, alpha);
            yield return new WaitForEndOfFrame();
        }
        sr.color = new Color(col.r, col.g, col.b, 0);
    }

    IEnumerator IPlayerClip()
    {
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.PlaySound(GameManager.Instance.settings.puzzleSolveClip);
    }
}

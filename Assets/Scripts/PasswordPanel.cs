using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class PasswordPanel : Interactable
{
    GameObject correctEffect;
    PasswordKeyBoard panel;
    bool canInteract;
    bool isFirstInteract;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        correctEffect = transform.Find("Cone").gameObject;
        panel = FindObjectOfType<PasswordKeyBoard>(true);
        panel.OnCorrect += () => { SetCanInteract(false); correctEffect.SetActive(true); };
        panel.OnCorrect += () => { StartCoroutine(IPopF(1f)); };
        canInteract = true;
        isFirstInteract = true;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void Interact()
    {
        if (canInteract)
        {
            panel.gameObject.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("Level4");
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (canInteract)
            {
                SetOutlineThickness(0.015f);
            }
            OnEnter?.Invoke();
            isInteracting = true;

            if (isFirstInteract)
            {
                UIManager.Instance.PopF(1.5f);
                isFirstInteract = false;
            }
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (canInteract)
            {
                SetOutlineThickness(0);
            }
            OnEnter?.Invoke();
            isInteracting = false;
        }
    }

    void SetCanInteract(bool b)
    {
        canInteract = b;
    }

    IEnumerator IPopF(float timer)
    {
        yield return new WaitForSeconds(timer);
        UIManager.Instance.PopF(1.5f);
    }
}

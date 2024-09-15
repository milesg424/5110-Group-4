using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PasswordPanel : Interactable
{
    PasswordKeyBoard panel;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        panel = FindObjectOfType<PasswordKeyBoard>(true);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public override void Interact()
    {
        panel.gameObject.SetActive(true);
    }


}

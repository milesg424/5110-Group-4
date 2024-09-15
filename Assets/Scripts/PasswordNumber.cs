using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PasswordNumber : MonoBehaviour
{
    List<Image> images;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Init()
    {
        images = new List<Image>();
        Transform root = transform.Find("Root");
        for (int i = 0; i < 7; i++)
        {
            images.Add(root.GetChild(i).GetComponent<Image>());
        }
    }

    public void ChangeNnumber(int num, Color fgColor, Color bgColor)
    {
        switch (num)
        {
            case 0:
                images[0].color = fgColor;
                images[1].color = fgColor;
                images[2].color = fgColor;
                images[3].color = fgColor;
                images[4].color = fgColor;
                images[6].color = fgColor;
                images[5].color = bgColor;
                break;
            case 1:
                images[3].color = fgColor;
                images[4].color = fgColor;
                images[0].color = bgColor;
                images[1].color = bgColor;
                images[2].color = bgColor;
                images[5].color = bgColor;
                images[6].color = bgColor;
                break;
            case 2:
                images[0].color = fgColor;
                images[2].color = fgColor;
                images[3].color = fgColor;
                images[5].color = fgColor;
                images[6].color = fgColor;
                images[1].color = bgColor;
                images[4].color = bgColor;
                break;
            case 3:
                images[0].color = fgColor;
                images[3].color = fgColor;
                images[4].color = fgColor;
                images[5].color = fgColor;
                images[6].color = fgColor;
                images[1].color = bgColor;
                images[2].color = bgColor;
                break;
            case 4:
                images[1].color = fgColor;
                images[3].color = fgColor;
                images[4].color = fgColor;
                images[5].color = fgColor;
                images[0].color = bgColor;
                images[2].color = bgColor;
                images[6].color = bgColor;
                break;
            case 5:
                images[0].color = fgColor;
                images[1].color = fgColor;
                images[4].color = fgColor;
                images[5].color = fgColor;
                images[6].color = fgColor;
                images[2].color = bgColor;
                images[3].color = bgColor;
                break;
            case 6:
                images[0].color = fgColor;
                images[1].color = fgColor;
                images[2].color = fgColor;
                images[4].color = fgColor;
                images[5].color = fgColor;
                images[6].color = fgColor;
                images[3].color = bgColor;
                break;
            case 7:
                images[0].color = fgColor;
                images[3].color = fgColor;
                images[4].color = fgColor;
                images[1].color = bgColor;
                images[2].color = bgColor;
                images[5].color = bgColor;
                images[6].color = bgColor;
                break;
            case 8:
                images[0].color = fgColor;
                images[1].color = fgColor;
                images[2].color = fgColor;
                images[3].color = fgColor;
                images[4].color = fgColor;
                images[5].color = fgColor;
                images[6].color = fgColor;
                break;
            case 9:
                images[0].color = fgColor;
                images[1].color = fgColor;
                images[3].color = fgColor;
                images[4].color = fgColor;
                images[5].color = fgColor;
                images[6].color = fgColor;
                images[2].color = bgColor;
                break;
            default:
                images[5].color = fgColor;
                images[0].color = bgColor;
                images[1].color = bgColor;
                images[2].color = bgColor;
                images[3].color = bgColor;
                images[4].color = bgColor;
                images[6].color = bgColor;
                break;
        }
    }

    public void ChangeColor(Color color)
    {
        for (int i = 0; i < 7; i++)
        {
            images[i].color = color;
        }
    }

    public void Clear(Color color)
    {
        for (int i = 0; i < 7; i++)
        {
            images[i].color = color;
        }
    }
}

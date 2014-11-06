using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stone : MonoBehaviour
{
    public SpriteRenderer normal;
    public SpriteRenderer normalsign;
    public SpriteRenderer back;
    public  int number = 1;
    public  int type = 1;
    public Sprite[] signs;


    public void    turnit(bool val)
    {
        if (!val)
        {
            back.enabled = true;
            normal.enabled = false;
            normalsign.enabled = false;
        }
        else
        {
            back.enabled = true;
            normal.enabled = true;
            normalsign.enabled = true;
        }
    }

    public void imaging(Sprite cursprite)
    {
        normal.sprite = cursprite;
        normalsign.sprite = signs[type];

        if (OkeyEngine.jokernumber != 0)
        {
            if (type == 4)
            {
                number = OkeyEngine.jokernumber;
                type = OkeyEngine.jokertype;
            }
            else if (number == OkeyEngine.jokernumber && type == OkeyEngine.jokertype)
            {
                number = 0;
                type = 4;
            }
        }


    }

    public void hideit()
    {
        normal.enabled = false;
        normalsign.enabled = false;
        back.enabled = false;
    }

    void placeitforward(Stone tempcard, int layer)
    {
        tempcard.renderer.sortingOrder = layer;
    }

    public void layering(bool show)
    {
        if (show)
        {
            renderer.sortingOrder = 60;
            normal.renderer.sortingOrder = 61;
            normalsign.renderer.sortingOrder = 61;
        }
        else
        {
            int parentplace = 0;
            if (transform.parent.tag == "places")
            {
                OkeyPlace parentpl = transform.parent.GetComponent<OkeyPlace>();
                parentplace = parentpl.place;
                renderer.sortingOrder = 5 + parentplace;
                normal.renderer.sortingOrder = 6 + parentplace;
                normalsign.renderer.sortingOrder = 6 + parentplace;
            }
            else if (transform.parent.tag == "sides")
            {
                transform.parent.SendMessage("layerit", this);
            }

        }
    }

}

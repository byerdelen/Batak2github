using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OkeySides : MonoBehaviour
{

    // Use this for initialization
    public int sidenum;
    public List<Stone> cards;
    public OkeyEngine engine;



    public void putstone(Stone tempcard)
    {
        cards.Add(tempcard);
        tempcard.transform.parent.SendMessage("remove", tempcard);
        tempcard.transform.parent = transform;
        tempcard.renderer.enabled = true;
        tempcard.normal.enabled = true;
        tempcard.normalsign.enabled = true;
        layerit(tempcard);

        iTween.MoveTo(tempcard.gameObject, iTween.Hash("x", 0.0, "y", 0.0, "z", 0.0, "islocal", true,  "easeType",  "easeOutQuad", "time", 0.30f));
    }

    public void layerit(Stone tempcard)
    {
        tempcard.renderer.sortingOrder = cards.Count * 2;
        tempcard.normal.sortingOrder = cards.Count * 2 + 1;
        tempcard.normalsign.sortingOrder = cards.Count * 2 + 1;
    }


    public void remove(Stone tempcard)
    {
        cards.Remove(tempcard);
    }



}

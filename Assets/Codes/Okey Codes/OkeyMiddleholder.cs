using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OkeyMiddleholder : MonoBehaviour
{

    // Use this for initialization
    public List<Stone> cards;
    public OkeyEngine engine;

    public void addinfo(Stone tempcard)
    {
        cards.Add(tempcard);
        tempcard.transform.parent = transform;
        tempcard.layering(false);
        if (cards.Count == 1)
            tempcard.turnit(false);
        else
            tempcard.hideit();
    }

    public void remove(Stone tempcard)
    {
        cards.Remove(tempcard);
    }




}

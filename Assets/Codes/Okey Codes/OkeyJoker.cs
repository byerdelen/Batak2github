using UnityEngine;
using System.Collections;

public class OkeyJoker : MonoBehaviour
{

    public Stone card;
    public OkeyEngine engine;

    void addinfo(Stone tempcard)
    {
        card = tempcard;
        tempcard.transform.parent = transform;
        iTween.MoveTo(card.gameObject, iTween.Hash("x", 0, "y", 0, "z", 0, "islocal", true,  "easeType",  "easeOutQuad", "time", 0.3f));
        engine.joker = tempcard;
        tempcard.renderer.sortingOrder = 0;
        tempcard.normal.sortingOrder = 1;
        tempcard.normalsign.sortingOrder = 1;
    }

}

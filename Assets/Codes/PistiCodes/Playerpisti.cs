using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Playerpisti : MonoBehaviour
{

    public    bool canplay = false;
    public List<Card> cards;
    public Enginepisti engine;
    public Card tempcard;
    Card oldcard;
    bool phone = false;
    Vector2 posch;
    public int handtotake = 0;

    void Start()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
            phone = true;
    }

    void addinfo(Card tempcard)
    {
        cards.Add(tempcard);
        tempcard.rend.sprite = tempcard.normal;
        tempcard.transform.parent = transform;
        placethem();
    }


    void placethem()
    {
        for (int i = 0; i < cards.Count; ++i)
        {

            cards[i].rend.renderer.sortingOrder = i;
            iTween.MoveTo(cards[i].gameObject, iTween.Hash("x", -2.3f * (cards.Count / 2.0f - 0.5f) + i * 2.3f, "y", -Mathf.Abs(i - (cards.Count / 2.0f) + 0.5f) * 0.3f, "z", -i * 0.2f, "islocal", true,  "easeType",  "easeOutQuad", "time", 0.3f));
            iTween.RotateTo(cards[i].gameObject, iTween.Hash("x", 0, "y", 0, "z", ((cards.Count / 2.0f - 0.5f) * 4.0f - i * 4.0f), "islocal", true,  "easeType",  "easeOutQuad", "time", 0.1f));

        }
    }


    void play()
    {
        if (cards.Count == 0)
        {
            StartCoroutine(engine.dealhand());
            return;
        }
        canplay = true;
    }




    void Update ()
    {
        if (!canplay)
            return;
        buttondown();
    }

    Vector2 mousepos()
    {
        if (phone)
            return   Input.GetTouch(0).position;
        else
            return Input.mousePosition;
    }



    void buttondown()
    {

        if (Input.GetMouseButton(0))
        {

            Ray ray = Camera.main.ScreenPointToRay(mousepos());
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 1000);

            if (hit)
            {
                if (hit.transform.parent != transform)
                    return;
                tempcard =  hit.transform.GetComponent<Card>();

                if (oldcard != tempcard)
                {

                    tempcard.rend.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
                    if (oldcard)
                        oldcard.rend.transform.localScale = new Vector3(1, 1, 1);
                    oldcard = tempcard;
                }

            }
            else
            {
                tempcard = null;
                if (oldcard)
                    oldcard.rend.transform.localScale = new Vector3(1, 1, 1);
                oldcard = null;
            }
        }
        if (Input.GetMouseButtonUp(0) && tempcard)
        {
            //check the card
            if (tempcard.transform.parent != transform)
                return;
            tempcard.rend.transform.localScale = new Vector3(1, 1, 1);
            checkthecard(tempcard);

            tempcard = null;
            oldcard = null;
        }
    }

    void    checkthecard(Card tempcard)
    {

        cards.Remove(tempcard);
        placethem();
        if (Cardstatic.checkpowercardopened(tempcard.type, engine.powercardtype))
            engine.powercardopened = true;
        tempcard.rend.renderer.sortingOrder = 20;
        StartCoroutine(engine.middle.addcard(tempcard));
        canplay = false;
    }



}
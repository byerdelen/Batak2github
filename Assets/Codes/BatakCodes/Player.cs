using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{

    public    bool canplay = false;
    public List<Card> cards;
    public Engine engine;
    public Card tempcard;
    public Text ihale;
    Card oldcard;
    bool phone = false;
    Vector2 posch;
    public int handtotake = 0;
    public GameObject uyari;
    public Text uyaritext;
    public void hand(int val)
    {
        handtotake = val;
        ihale.text = "" + val;
        engine.aims[0] = val;
        if (val > 9)
            engine.pointlabels[0].fontSize = 25;
        engine.pointlabels[0].text = "0/" + val ;
    }

    void Start()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
            phone = true;
    }

    void addinfo(Card tempcard)
    {
        cards.Add(tempcard);
        tempcard.transform.parent = transform;
        sortcards();
    }

    void sortcards()
    {
        cards.Sort(delegate(Card t2, Card t1)
        {
            return (t2.type * 14 + t2.number).CompareTo(t1.type * 14 + t1.number);
        });
        placethem();
    }

    void placethem()
    {
        for (int i = 0; i < cards.Count; ++i)
        {

            cards[i].rend.renderer.sortingOrder = i;
            iTween.MoveTo(cards[i].gameObject, iTween.Hash("x", -1.9f * (cards.Count / 2.0f - 0.5f) + i * 1.9f, "y", -Mathf.Abs(i - (cards.Count / 2.0f) + 0.5f) * 0.3f, "z", -i * 0.1f, "islocal", true,  "easeType",  "easeOutQuad", "time", 0.3f));
            iTween.RotateTo(cards[i].gameObject, iTween.Hash("x", 0, "y", 0, "z", ((cards.Count / 2.0f - 0.5f) * 2.0f - i * 2.0f), "islocal", true,  "easeType",  "easeOutQuad", "time", 0.1f));

        }
    }


    void play()
    {
        if (cards.Count == 0)
        {
            engine.gameended();
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
        if (engine.middle.cardcount() == 0)
        {
            if (tempcard.type == engine.powercardtype && !engine.powercardopened)
            {
                warnit2("Önceden koz kullanılmadan koz atılamaz");
                return;
            }

        }
        else
        {
            if (engine.middle.startcard.type == engine.powercardtype)
            {
                if (Cardstatic.thereisthiscardtype(engine.powercardtype, cards))
                {
                    if (tempcard.type != engine.powercardtype)
                    {
                        warnit2("İlk atılan kart cinsi atılmalıdır");
                        return;
                    }
                    else
                    {
                        if (Cardstatic.thereisbiggerbutnotused(tempcard, Cardstatic.findbiggestfromtypetonumber(engine.middle.startcard.type, engine.middle.cards), cards))
                        {
                            warnit2("Koz atılınca eğer elde daha yüksek koz varsa o kart kullanılmalıdır");
                            return;
                        }
                    }
                }
            }
            else
            {
                if (tempcard.type != engine.middle.startcard.type)
                {
                    if (Cardstatic.thereisthiscardtype(engine.middle.startcard.type, cards))
                    {
                        warnit2("İlk atılan kart cinsi atılmalıdır");
                        return;
                    }
                    else
                    {
                        if (Cardstatic.thereisthiscardtype(engine.powercardtype, cards))
                        {
                            if (Cardstatic.thereisbiggerbutnotused(tempcard, Cardstatic.findbiggestfromtypetonumber(engine.powercardtype, engine.middle.cards), cards))
                            {
                                warnit2("Koz atılınca eğer elde daha yüksek koz varsa o kart kullanılmalıdır");
                                return;
                            }
                            if (tempcard.type != engine.powercardtype)
                            {
                                warnit2("Atılan kart cinsi elde yoksa koz atılmalıdır");
                                return;
                            }

                        }
                    }

                }
            }

        }
        cards.Remove(tempcard);
        placethem();
        if (Cardstatic.checkpowercardopened(tempcard.type, engine.powercardtype))
            engine.powercardopened = true;
        tempcard.rend.renderer.sortingOrder = 20;
        StartCoroutine(engine.middle.addcard(tempcard));
        canplay = false;
    }




    void warnit2(string  tempwarn)
    {
        StopCoroutine("warnit");
        StartCoroutine("warnit", tempwarn);
        if (Sound.sound == 0)
            engine.audio.PlayOneShot(engine.buzz);
    }

    IEnumerator warnit(string  tempwarn)
    {
        float tempcounter = 2.0f;
        uyari.SetActive(true);
        uyaritext.text = tempwarn;
        while (tempcounter > 0)
        {
            yield return new WaitForSeconds(0.1f);
            tempcounter -= 0.1f;
        }
        uyari.SetActive(false);
    }


}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class AIpisti2 : MonoBehaviour
{

    public List<Card> cards;
    public Enginepisti2 engine;
    public int handtotake = 0;
    public int playerturn = 0;


    IEnumerator addinfo(Card tempcard)
    {
        cards.Add(tempcard);
        tempcard.rend.sprite = tempcard.back;
        tempcard.transform.parent = transform;
        iTween.MoveTo(cards[cards.Count - 1].gameObject, iTween.Hash("x", 0, "y", -5.0f, "z", 0, "islocal", true,  "easeType",  "easeOutQuad", "time", 0.3f));
        iTween.RotateTo(cards[cards.Count - 1].gameObject, iTween.Hash("x", 0, "y", 0, "z", 0, "islocal", true,  "easeType",  "easeOutQuad", "time", 0.3f));
        yield return new  WaitForSeconds(0.3f);
        tempcard.rend.sprite = tempcard.normal;
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

    }


    IEnumerator play()
    {
        if (cards.Count == 0)
        {
            StartCoroutine(engine.dealhand());
            yield break;
        }
        int playingcard = 0;
        //If card count is more than zero
        playingcard = checkthebestplay();
        yield return new WaitForSeconds(0.75f);

        Card curcard  = cards[playingcard];
        cards.Remove(curcard);
        curcard.rend.renderer.sortingOrder = 20;
        StartCoroutine(engine.middle.addcard(curcard));

    }

    int checkthebestplay()
    {
        int[] points = new int[cards.Count];
        for (int i = 0; i < cards.Count; ++i)
        {
            for (int s = 0; s < cards.Count; ++s)
            {
                if (i != s && cards[i].number == cards[s].number)
                    points[i] += 1;
            }

            for (int a = 0; a < engine.usedcards.Length; ++a)
            {

                for (int e = 0; e < engine.usedcards[a].woncards.Count; ++e)
                {
                    if (engine.usedcards[a].woncards[e].turntaken != -1 && engine.usedcards[a].woncards[e].turntaken != (playerturn % 2))
                        continue;
                    if (engine.usedcards[a].woncards[e].pisticount > (engine.curcards.Count + 20))
                        continue;
                    if (cards[i].number == engine.usedcards[a].woncards[e].number && cards[i].number != 11)
                        points[i] += 1;
                }
            }
            for (int a = 0; a < engine.middle.cards.Count; ++a)
            {
                if (cards[i].number == engine.middle.cards[a].number && cards[i].number != 11 && engine.middle.cards[a].rend.sprite == engine.middle.cards[a].normal)
                    points[i] += 1;
            }
            if (cards[i].number == 11)
            {
                if (engine.middle.cards.Count > 0)
                    points[i] = 2;
                else
                    points[i] = -10;
            }
            if (engine.middle.cards.Count > 0 && cards[i].number == engine.middle.cards[engine.middle.cards.Count - 1].number)
                points[i] = 10;

        }
        return Cardstatic.findbiggest(points);
    }



}
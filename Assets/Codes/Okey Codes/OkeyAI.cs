using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class OkeyAI : MonoBehaviour
{

    public List<Stone> cards;
    public OkeyEngine engine;
    public int ainum;
    IEnumerator addinfo(Stone tempcard)
    {
        cards.Add(tempcard);

        tempcard.turnit(false);
        tempcard.transform.parent = transform;
        iTween.MoveTo(cards[cards.Count - 1].gameObject, iTween.Hash("x", 0, "y", -5.0f, "z", 0, "islocal", true,  "easeType",  "easeOutQuad", "time", 0.3f));
        yield return new  WaitForSeconds(0.3f);
        tempcard.turnit(true);

    }


    IEnumerator play()
    {

        yield return new WaitForSeconds(0.3f);
        //cards.Remove(curcard);
        //curcard.renderer.sortingOrder = 20;
        int randomplace = Random.Range(0, 5);
        if (cards.Count == 14)
        {
            if (engine.sides[ainum].cards.Count > 0 && (engine.sides[ainum].cards[engine.sides[ainum].cards.Count - 1].type == 4 || randomplace == 0)  )
            {
                Stone tempcard = engine.sides[ainum].cards[engine.sides[ainum].cards.Count - 1];
                tempcard.transform.parent.SendMessage("remove", tempcard);
                StartCoroutine(addinfo(tempcard));
            }
            else
            {
                Stone tempcard = engine.middle.cards[engine.middle.cards.Count - 1];
                tempcard.back.renderer.enabled = true;
                tempcard.transform.parent.SendMessage("remove", tempcard);
                StartCoroutine(addinfo(tempcard));
            }
            yield return new WaitForSeconds(0.6f);
        }



        randomplace = Random.Range(0, cards.Count);
        while (cards[randomplace].type == 4)
            randomplace = Random.Range(0, cards.Count);

        if (engine.turnplus >= engine.endturn)
        {
            iTween.MoveTo(cards[randomplace].gameObject, iTween.Hash("x", engine.middle.transform.position.x, "y", engine.middle.transform.position.y, "z", engine.middle.transform.position.z, "easeType",  "easeOutQuad", "time", 0.3f));
            yield return new WaitForSeconds(0.6f);
            engine.newgame();
            yield break;
        }


        int sidesnum = (ainum + 1);
        if (sidesnum > 3)
            sidesnum = 0;
        engine.sides[sidesnum].SendMessage("putstone", cards[randomplace]);

        yield return new WaitForSeconds(0.6f);
        engine.turnfinished(ainum);

    }

    public void remove(Stone tempcard)
    {
        cards.Remove(tempcard);
    }



}
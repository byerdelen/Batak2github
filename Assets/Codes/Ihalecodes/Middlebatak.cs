using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Middlebatak : MonoBehaviour
{

    public List<Card> cards = new List<Card>();
    [System.Serializable]
    public class arrayData2
    {
        public Vector3 place;
        public Vector3 rotate;
    }
    public List<arrayData2> placements = new List<arrayData2>();
    public Card startcard;
    public Enginebatak engine;
    public AudioClip slide;


    public IEnumerator addcard(Card curcard)
    {
        cards[engine.turn] = curcard;

        int curcardcount = cardcount();

        curcard.transform.parent = transform;

        iTween.MoveTo(curcard.gameObject, iTween.Hash("x", placements[curcardcount - 1].place.x * 2.0f, "y", placements[curcardcount - 1].place.y, "z", placements[curcardcount - 1].place.z, "islocal", true,  "easeType",  "easeOutQuad", "time", 0.30f));
        iTween.RotateTo(curcard.gameObject, iTween.Hash("x", placements[curcardcount - 1].rotate.x, "y", placements[curcardcount - 1].rotate.y, "z", placements[curcardcount - 1].rotate.z, "islocal", true,  "easeType",  "easeOutQuad", "time", 0.30f));
        if (Sound.sound == 0)
            audio.PlayOneShot(slide, 0.4f);
        yield return new WaitForSeconds(0.3f);
        curcard.rend.renderer.sortingOrder = curcardcount;
        if (curcardcount == 1)
        {
            startcard = curcard;
        }
        if (curcardcount == 4)
            yield return StartCoroutine(engine.winner(checkresult()));
        else
            engine.turnfinished(engine.turn);

    }


    public int cardcount()
    {

        int curcount = 0;

        for (int i = 0; i < cards.Count; ++i)
        {
            if (cards[i] != null)
                curcount += 1;
        }

        return curcount;

    }


    int checkresult()
    {
        if (Cardstatic.thereispowercard(cards, engine.powercardtype))
            return Cardstatic.findbiggestfromtypetoplayer(engine.powercardtype, cards);
        else
            return Cardstatic.findbiggestfromtypetoplayer(startcard.type, cards);
    }


    public void clearcards()
    {

        for (int i = 0; i < cards.Count; ++i)
        {
            cards[i] = null;
        }
        startcard = null;
    }









}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Middlepisti2 : MonoBehaviour
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
    public Enginepisti2 engine;
    public AudioClip slide;


    public IEnumerator addcard(Card curcard)
    {
        cards.Add(curcard);

        curcard.transform.parent = transform;

        if (cards.Count > 1)
        {
            iTween.MoveTo(cards[cards.Count - 2].gameObject, iTween.Hash("x", 0, "y", 0, "z", 0, "islocal", true,  "easeType",  "easeOutQuad", "time", 0.30f));
            iTween.RotateTo(cards[cards.Count - 2].gameObject, iTween.Hash("x", 0, "y", 0, "z", 0, "islocal", true,  "easeType",  "easeOutQuad", "time", 0.30f));
        }
        iTween.MoveTo(curcard.gameObject, iTween.Hash("x", 1.1, "y", -0.29, "z", 0, "islocal", true,  "easeType",  "easeOutQuad", "time", 0.30f));
        iTween.RotateTo(curcard.gameObject, iTween.Hash("x", 0, "y", 0, "z", 340, "islocal", true,  "easeType",  "easeOutQuad", "time", 0.30f));
        if (Sound.sound == 0)
            engine.audio.PlayOneShot(slide, 0.4f);
        yield return new WaitForSeconds(0.3f);
        curcard.rend.renderer.sortingOrder = cards.Count - 30;

        yield return StartCoroutine(checkresult());
        engine.turnfinished(engine.turn);
    }

    public IEnumerator checkresult()
    {
        if (cards.Count < 2)
            yield break;
        if (cards[cards.Count - 1].number == cards[cards.Count - 2].number || cards[cards.Count - 1].number == 11)
            yield return StartCoroutine(engine.winner());
    }

    public IEnumerator addground(Card curcard)
    {
        cards.Add(curcard);
        curcard.transform.parent = transform;
        iTween.MoveTo(curcard.gameObject, iTween.Hash("x", placements[cards.Count - 1].place.x, "y", placements[cards.Count - 1].place.y, "z", placements[cards.Count - 1].place.z, "islocal", true,  "easeType",  "easeOutQuad", "time", 0.30f));
        iTween.RotateTo(curcard.gameObject, iTween.Hash("x", placements[cards.Count - 1].rotate.x, "y", placements[cards.Count - 1].rotate.y, "z", placements[cards.Count - 1].rotate.z, "islocal", true,  "easeType",  "easeOutQuad", "time", 0.30f));
        curcard.rend.renderer.sortingOrder = cards.Count - 30;
        yield return new WaitForSeconds(0.3f);
    }














}
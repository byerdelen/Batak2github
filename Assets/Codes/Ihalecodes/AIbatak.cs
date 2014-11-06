using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class AIbatak : MonoBehaviour
{

    public List<Card> cards;
    public Enginebatak engine;
    public int handtotake = 0;
    public int playerturn = 0;
    public int[] bestpossible;
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
            engine.gameended();
            yield break;
        }


        int playingcard = 0;
        //If card count is more than zero

        if (engine.middle.cardcount() > 0)
        {
            //Check if I have the card type
            if (Cardstatic.checkifihavethecardtype(engine.middle.startcard.type, cards))
            {
                //indextrue,numberfalse
                //Check if  my biggest of that type is bigger than the biggest on the table

                if (engine.middle.startcard.type != engine.powercardtype && Cardstatic.thereispowercard(engine.middle.cards, engine.powercardtype))
                {
                    playingcard = Cardstatic.findsmallestofthetype(engine.middle.startcard.type, true, cards);

                }

                else if (engine.middle.cardcount() == 3 && Cardstatic.findbiggestofthetype(engine.middle.startcard.type, false, cards) > Cardstatic.findbiggestfromtypetonumber(engine.middle.startcard.type, engine.middle.cards))
                {

                    playingcard = Cardstatic.findbiggerofthetype(engine.middle.startcard.type, Cardstatic.findbiggestfromtypetonumber(engine.middle.startcard.type, engine.middle.cards), cards);
                }
                else if (Cardstatic.findbiggestofthetype(engine.middle.startcard.type, false, cards) > Cardstatic.findbiggestfromtypetonumber(engine.middle.startcard.type, engine.middle.cards) && Cardstatic.allbigsaregone(engine.middle.startcard.type, Cardstatic.findbiggestofthetype(engine.middle.startcard.type, false, cards), engine.usedcards))
                {

                    //If I have bigger, find the smallest of that type that is bigger than the biggest on the table

                    playingcard = Cardstatic.findbiggestofthetype(engine.middle.startcard.type, true, cards);

                }
                else
                    //If I dont have bigger , find the smallest to waste
                {
                    playingcard = Cardstatic.findsmallestofthetype(engine.middle.startcard.type, true, cards);
                }
            }

            else  //If I dont have the card type
            {

                //print("Check if I have the power card");
                if (Cardstatic.checkifihavethecardtype(engine.powercardtype, cards))
                {

                    //Check if  my biggest of that type is bigger than the biggest on the table
                    if (Cardstatic.findbiggestofthetype(engine.powercardtype, false, cards) > Cardstatic.findbiggestfromtypetonumber(engine.powercardtype, engine.middle.cards))

                    {
                        //If I have bigger, find the smallest of that type that is bigger than the biggest on the table
                        playingcard = Cardstatic.findbiggerofthetype(engine.powercardtype, Cardstatic.findbiggestfromtypetonumber(engine.powercardtype, engine.middle.cards), cards);

                    }
                    else
                        //If I dont have bigger , find the smallest to waste
                        playingcard = Cardstatic.findsmallestofthetype(engine.powercardtype, true, cards);

                    engine.powercardopened = true;
                }
                else//If I have the played card and the power card
                {
                    //print("If I have the played card and the power card");
                    playingcard = Cardstatic.findleastimportantcard(engine.powercardopened, engine.powercardtype, cards);
                }


            }
        }
        else
        {
            //print("If card count is  zero");

            playingcard = Cardstatic.findbestcard(engine.powercardopened, 0, engine.powercardtype, cards);
            int counter = 0;
            while (!Cardstatic.allbigsaregone(cards[playingcard].type, cards[playingcard].number, engine.usedcards) && counter < 3)
            {
                counter += 1;
                playingcard = Cardstatic.findbestcard(engine.powercardopened, counter, engine.powercardtype, cards);
            }
            if (!Cardstatic.allbigsaregone(cards[playingcard].type, cards[playingcard].number, engine.usedcards) && counter == 3)
                playingcard = Cardstatic.findleastimportantcard(engine.powercardopened, engine.powercardtype, cards);
            if (!engine.powercardopened && cards[playingcard].type == engine.powercardtype)
                playingcard = Cardstatic.findleastimportantcard(engine.powercardopened, engine.powercardtype, cards);
        }

        yield return new WaitForSeconds(0.5f);

        Card curcard  = cards[playingcard];
        cards.Remove(curcard);
        curcard.rend.renderer.sortingOrder = 20;
        if (Cardstatic.checkpowercardopened(curcard.type, engine.powercardtype))
            engine.powercardopened = true;
        StartCoroutine(engine.middle.addcard(curcard));

    }


}
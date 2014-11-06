using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Cardstatic
{

    public static bool thereisthiscardtype(int curtype, List<Card> cards)
    {

        for (int i = 0; i < cards.Count; ++i)
        {
            if (cards[i].type == curtype)
                return true;
        }
        return false;
    }

    public static bool thereisbiggerbutnotused(Card curcard, int biggernum, List<Card> cards)
    {
        int biggestnumber = 0;
        for (int i = 0; i < cards.Count; ++i)
        {
            if (cards[i].type != curcard.type)
                continue;
            if (cards[i].number > biggestnumber)
                biggestnumber = cards[i].number;
        }

        if (curcard.number < biggernum && biggestnumber > biggernum)
            return true;
        else
            return false;
    }

    public static bool checkpowercardopened(int curtype, int powercardtype)
    {
        if (curtype == powercardtype)
        {
            return true;
        }
        else
            return false;
    }

    public static int findtherank(int[] points, int[] prevpoints)
    {
        int rank = 4;
        for (int i = 1; i < 4; ++i)
        {
            if ((points[0] + prevpoints[0]) >= (points[i] + prevpoints[i]))
                rank -= 1;
        }
        return rank;
    }

    public static int findtherank358(int[] points, int[] prevpoints)
    {
        int rank = 3;
        for (int i = 1; i < 3; ++i)
        {
            if ((points[0] + prevpoints[0]) >= (points[i] + prevpoints[i]))
                rank -= 1;
        }
        return rank;
    }

    public static   int findbiggestfromtypetonumber(int curtype, List<Card> cards)
    {

        int biggestnumber = 0;

        for (int i = 0; i < cards.Count; ++i)
        {
            if (cards[i] && curtype == cards[i].type && cards[i].number > biggestnumber)
                biggestnumber = cards[i].number;
        }
        return biggestnumber;

    }

    public static   int findbiggestfromtypetoplayer(int curcard, List<Card> cards)
    {

        int biggestplayer = curcard;
        int biggestnumber = 0;

        for (int i = 0; i < cards.Count; ++i)
        {
            if (curcard == cards[i].type && cards[i].number > biggestnumber)
            {
                biggestnumber = cards[i].number;
                biggestplayer = i;
            }
        }

        return biggestplayer;

    }

    public static bool thereispowercard(List<Card> tempcards, int powercardtype )
    {

        for (int i = 0; i < tempcards.Count; ++i)
        {
            if (tempcards[i] && tempcards[i].type == powercardtype)
                return true;
        }
        return false;

    }

    public static bool thereis(int tempnum, int temptype, List<Card> cards)
    {
        for (int i = 0; i < cards.Count; ++i)
        {
            if (cards[i].number == tempnum && cards[i].type == temptype)
                return true;

        }
        return false;
    }

    public static int therearecards(int temptype, List<Card> cards)
    {
        int tempnum = 0;
        for (int i = 0; i < cards.Count; ++i)
        {
            if (cards[i].type == temptype)
                tempnum += 1;

        }
        return tempnum;
    }


    public static bool allbigsaregone(int curtype, int curbiggest, List<Card> cards)
    {
        List<int> numberstocheck = new List<int>();

        for (int i = curbiggest + 1; i < 15; ++i)
        {
            numberstocheck.Add(i);
        }

        for (int i = 0; i < cards.Count; ++i)
        {
            if (cards[i].type != curtype)
                continue;
            for (int a = 0; a < numberstocheck.Count; ++a)
            {
                if (cards[i].number == numberstocheck[a])
                {
                    numberstocheck.RemoveAt(a);
                    a = 100;
                }
            }
        }
        if (numberstocheck.Count == 0)
            return true;
        else
            return false;
    }



    public static bool checkifihavethecardtype(int curtype, List<Card> cards)
    {
        for (int i = 0; i < cards.Count; ++i)
        {
            if (cards[i].type == curtype)
                return true;
        }
        return false;
    }

    public static int findleastimportantcard(bool curopened, int powercardtype, List<Card> cards)
    {

        int smallestnumber = 15;
        int cardindex = 0;
        for (int i = 0; i < cards.Count; ++i)
        {
            if (!curopened && powercardtype == cards[i].type)
                continue;
            if (cards[i].number < smallestnumber)
            {
                smallestnumber = cards[i].number;
                cardindex = i;
            }
        }

        return cardindex;

    }

    public static int findbestcard(bool curopened, int curtype, int powercardtype, List<Card> cards)
    {

        int biggestnumber = 0;
        int cardindex = 0;
        for (int i = 0; i < cards.Count; ++i)
        {
            if (!curopened && powercardtype == cards[i].type)
            {
                continue;
            }
            if (cards[i].type != curtype)
                continue;
            if (cards[i].number > biggestnumber)
            {
                biggestnumber = cards[i].number;
                cardindex = i;
            }
        }

        return cardindex;

    }
    /*
        public static Card[] findmostsuitablecard2(List<Card> cards, List<Card> excludedcards)
        {

            int biggestnumber = 14;
            int cardindex = 0;
            Card biggestcard = null;
            for (int i = 0; i < 4; ++i)
            {

                int tempnum = findbiggestofthetypewithexcluded(i, cards, excludedcards);
                if (cards[tempnum].number < biggestnumber)
                {
                    biggestnumber = cards[tempnum].number;
                    biggestcard = cards[tempnum];
                    cardindex = i;
                }
            }

            return new Card[] {cards[findsmallestofthetype(cardindex, true, cards)], biggestcard};

        }

        public static List<Card> findmostsuitablecard(List<Card> cards)
        {
            List<Vector2> tempcards = new List<Vector2>();
            int[] order = new int[] {0, 1, 2, 3};

            for (int a = 14; a > 0; --a)
            {

                order = reshuffle(order);
                for (int i = 0; i < 4; ++i)
                {

                    for (int e = 0; e < cards.Count; ++e)
                    {
                        if ((cards[e].type == order[i] && cards[e].number == a))
                        {
                            continue;
                        }
                        tempcards.Add(new Vector2(order[i], a));
                        if (tempcards.Count == 13)
                        {
                            tempcards = findmostsuitablecard2(tempcards);
                            List<Card> tempcards2 = null;
                            for (int s = 0; s < tempcards.Count; ++s)
                            {
                                for (int l = 0; l < cards.Count; ++l)
                                {
                                    if ((cards[s].type == (int)tempcards[s].x && cards[s].number == (int)tempcards[s].y))
                                        tempcards2.Add(cards[s]);
                                }
                            }
                            return tempcards2;

                        }

                    }

                }

            }
            return null;
        }


        public static List<Vector2> findmostsuitablecard2(List<Vector2> cards)
        {
            List<Vector2> tempcards = new List<Vector2>();

            for (int a = 0; a < 15; ++a)
            {

                for (int i = 0; i < 4; ++i)
                {

                    for (int e = 0; e < cards.Count; ++e)
                    {
                        if (((int)cards[e].x == i))
                        {
                            tempcards.Add(new Vector2(i, a));
                            if (tempcards.Count == 13)
                                return tempcards;
                            break;
                        }
                    }

                }

            }
            return null;
        }

        static int[] reshuffle(int[] texts)
        {

            for (int t = 0; t < texts.Length; t++ )
            {
                int tmp = texts[t];
                int r = Random.Range(t, texts.Length);
                texts[t] = texts[r];
                texts[r] = tmp;
            }
            return texts;
        }

    */

    public static int findbiggestofthetypewithexcluded(int curtype, List<Card> cards, List<Card> excludedcards)
    {

        int biggestnumber = 0;
        int cardindex = 0;
        bool cancontinue = true;
        for (int i = 0; i < cards.Count; ++i)
        {

            if (curtype != cards[i].type)
                continue;
            for (int a = 0; a < excludedcards.Count; ++a)
            {
                if (excludedcards[a] == cards[i])
                {
                    cancontinue = false;
                    break;
                }
            }
            if (!cancontinue)
            {
                cancontinue = true;
                continue;
            }
            if (cards[i].number > biggestnumber)
            {
                biggestnumber = cards[i].number;
                cardindex = i;
            }
        }
        return cardindex;
    }

    public static int findbiggestofthetype(int curtype, bool index, List<Card> cards)
    {

        int biggestnumber = 0;
        int cardindex = 0;
        for (int i = 0; i < cards.Count; ++i)
        {

            if (curtype != cards[i].type)
                continue;

            if (cards[i].number > biggestnumber)
            {
                biggestnumber = cards[i].number;
                cardindex = i;
            }


        }
        if (index)
            return cardindex;
        else
            return biggestnumber;
    }


    public static int findbiggerofthetype(int curtype, int curstart, List<Card> cards)
    {

        int biggestnumber = 15;
        int cardindex = 0;
        for (int i = 0; i < cards.Count; ++i)
        {

            if (curtype != cards[i].type)
                continue;

            if (cards[i].number < curstart)
                continue;

            if (cards[i].number < biggestnumber)
            {
                biggestnumber = cards[i].number;
                cardindex = i;
            }


        }

        return cardindex;
    }

    public static Card findsmallest(List<Card> cards)
    {

        int biggestnumber = 15;
        Card tempcard = new Card();
        for (int i = 0; i < cards.Count; ++i)
        {


            if (cards[i].number < biggestnumber)
            {
                biggestnumber = cards[i].number;
                tempcard = cards[i];
            }
        }
        return tempcard;

    }

    public static int findsmallestofthetype(int curtype, bool index, List<Card> cards)
    {

        int biggestnumber = 15;
        int cardindex = 0;
        for (int i = 0; i < cards.Count; ++i)
        {

            if (curtype != cards[i].type)
                continue;

            if (cards[i].number < biggestnumber)
            {
                biggestnumber = cards[i].number;
                cardindex = i;
            }
        }
        if (index)
            return cardindex;
        else
            return biggestnumber;
    }

    public static int calculatehandsbatak(int powercardtype, List<Card> cards)
    {
        int temphand = 0;
        for (int i = 0; i < cards.Count; ++i)
        {
            if (cards[i].number == 14)
            {
                temphand += 1;
                if (Cardstatic.thereis(13, cards[i].type, cards))
                    temphand += 1;
            }
        }
        int tempcards = 0;
        for (int i = 0; i < 4; ++i)
        {
            tempcards = Cardstatic.therearecards(i, cards);
            if (powercardtype == i)
            {
                if (tempcards > 3)
                    temphand += (tempcards - 3);
            }
            else if (tempcards == 0)
                temphand += 1;
        }
        temphand += 1;
        return temphand;
    }

    public static int calculatehandsihale(int powercardtype, List<Card> cards)
    {
        int temphand = 0;
        for (int i = 0; i < cards.Count; ++i)
        {
            if (cards[i].number == 14)
            {
                temphand += 1;
                if (Cardstatic.thereis(13, cards[i].type, cards))
                    temphand += 1;
            }
        }
        int tempcards = 0;
        for (int i = 0; i < 4; ++i)
        {
            tempcards = Cardstatic.therearecards(i, cards);
            if (powercardtype == i)
            {
                if (tempcards > 3)
                    temphand += (tempcards - 3);
            }
            else if (tempcards == 0)
                temphand += 1;
        }
        return temphand;
    }

    public static int[] calculatebesttypeandhand(List<Card> cards)
    {
        int biggestnum = 0;
        int besttype = 0;
        for (int i = 0; i < 4; ++i)
        {
            int tempnum =        Cardstatic.calculatehandsihale(i, cards);
            if (tempnum > biggestnum)
            {
                biggestnum = tempnum;
                besttype = i;
            }

        }
        return new int[] {besttype, biggestnum};
    }

    public static int bestplayer(int[] aims)
    {
        int biggerone = 0;
        int player = 0;
        for (int i = 0; i < 4; ++i)
        {
            if (aims[i] > biggerone)
            {
                biggerone = aims[i];
                player = i;
            }
        }
        return player;
    }

    public static int findbiggest(int[] points)
    {

        int biggestnumber = 0;
        int cardindex = 0;
        for (int i = 0; i < points.Length; ++i)
        {

            if (points[i] > biggestnumber)
            {
                biggestnumber = points[i];
                cardindex = i;
            }
        }
        return cardindex;
    }

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class OkeyEngine : MonoBehaviour
{

    public GameObject cardobj;
    public GameObject[] players;
    public OkeyJoker jokerholder;
    public OkeySides[] sides;
    public OkeyMiddleholder middle;
    public Stone joker;
    public static int jokertype;
    public static int jokernumber;
    public int turn = 0;
    public int turnplus = 0;
    public int endturn;

    [System.Serializable]
    public class arrayData
    {
        public Sprite image;
        public int number;
        public int type;
    }
    public List<arrayData> cards = new List<arrayData>();

    public AudioClip slide;

    //   public UILabel[] pointlabels;
    //   public UILabel[] scorelabels;
    //public GameObject endmenu;
    //    public void startit()


    [Range(0, 100)]
    public int[] percentage;
    public Vector2[] winrounds;


    void Awake ()
    {
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        StartCoroutine(starting());
    }

    int endturnwincalc()
    {
        int randint = Random.Range(0, 101);
        int sum = 0;
        int tempnum = 0;

        for (int i = 0; i < percentage.Length; ++i)
        {
            sum += percentage[i];

            if (randint < sum)
            {
                tempnum = i;
                break;
            }

        }

        int winround = Random.Range((int)winrounds[tempnum].x, (int)winrounds[tempnum].y + 1);

        return winround;
    }

    IEnumerator starting()
    {

        endturn = endturnwincalc();

        while (cards.Count > 0)
        {
            int rand = Random.Range(0, cards.Count);
            while (cards.Count == 106 && rand > 103)
                rand = Random.Range(0, cards.Count);
            GameObject tempobj = Instantiate(cardobj, middle.transform.position, middle.transform.rotation) as GameObject;
            Stone tempcard = tempobj.GetComponent<Stone>();

            tempcard.number = cards[rand].number;
            tempcard.type = cards[rand].type;
            tempcard.imaging(cards[rand].image);
            cards.RemoveAt(rand);

            if (cards.Count < 57)
            {
                audio.PlayOneShot(slide, 0.2f);
                players[turn].SendMessage("addinfo", tempcard);
                yield return new WaitForSeconds(0.05f);
                turn += 1;
                if (turn == players.Length)
                    turn = 0;
            }
            else if (cards.Count == 105)
            {
                jokerholder.SendMessage("addinfo", tempcard);
                int tempjoker = 0;
                tempjoker = joker.number;
                tempjoker += 1;
                if (tempjoker == 14)
                    tempjoker = 1;
                jokernumber = tempjoker;
                jokertype = joker.type;
            }
            else
                middle.addinfo(tempcard);
        }

        turn -= 1;
        if (turn == -1)
            turn = players.Length - 1;
        players[0].SendMessage("canmove");
        players[turn].SendMessage("play");
    }

    public IEnumerator winner(int curwinner)
    {
        yield return new WaitForSeconds(0.5f);


        //points[curwinner] += 1;
        //        pointlabels[curwinner].text = "" + points[curwinner];
        turnfinished(curwinner - 1);
    }

    public void turnfinished(int curturn)
    {
        turn = curturn;
        turn += 1;
        turnplus += 1;
        if (turn == players.Length)
        {
            turn = 0;
        }

        StartCoroutine("finishturn");
    }

    IEnumerator finishturn()
    {
        if (middle.cards.Count == 0)
            yield return StartCoroutine("takecardstomiddle");
        players[turn].SendMessage("play");
        yield return null;
    }

    public IEnumerator takecardstomiddle()
    {
        while (sides[0].cards.Count > 0 || sides[1].cards.Count > 0 || sides[2].cards.Count > 0 || sides[3].cards.Count > 0)
        {
            int siderand = Random.Range(0, 4);
            while (sides[siderand].cards.Count == 0)
                siderand = Random.Range(0, 4);
            int cardrand = Random.Range(0, sides[siderand].cards.Count);
            sides[siderand].cards[cardrand].transform.parent = middle.transform;
            iTween.MoveTo(sides[siderand].cards[cardrand].gameObject, iTween.Hash("x", 0, "y", 0, "z", 0, "islocal", true,  "easeType",  "easeOutQuad", "time", 0.05f));
            yield return new WaitForSeconds(0.05f);
            middle.addinfo(sides[siderand].cards[cardrand]);


            sides[siderand].remove(sides[siderand].cards[cardrand]);
        }

        yield return null;
    }

    public void exit()
    {
        Application.LoadLevel(0);
    }

    public void newgame()
    {
        Application.LoadLevel(0);
    }

    public void gameended()
    {
        //endmenu.SetActive(true);
        //       for (int i = 0; i < points.Length; ++i)
        //          scorelabels[i].text = "" + points[i];

    }

}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class Enginebatak : MonoBehaviour
{

    public GameObject cardobj;
    public GameObject[] players;
    public AIbatak[] coms;
    public int[] aims;
    public int[] aimtypes;
    public int[] prevpoints;
    public int[] points;
    public int hand;
    public int turn = 0;
    public int powercardtype = 0;
    public Middlebatak middle;
    public bool powercardopened = false;
    public Text[] scores;
    public Text[] pointlabels;
    public GameObject[] symbols;
    public GameObject menu;
    public Text finishedstate;
    public Text handstate;
    [System.Serializable]
    public class arrayData
    {
        public Sprite image;
        public int number;
        public int type;
    }
    public List<arrayData> cards = new List<arrayData>();

    public List<Card> usedcards;
    public AudioClip slide;
    public AudioClip buzz;
    public AudioClip win;
    public AudioClip lose;
    public GameObject startmenu;
    public GameObject endmenu;
    public int[] gamedata;
    public GameObject turner;
    public Vector3[] turnerplacements;

    public int roundwinner = -1;
    public bool ihaleon = false;
    public bool playervoted = false;
    public Text[] ihale;
    public GameObject ihaletext;
    public GameObject[] ihalebuttons;
    public GameObject showsymbols;
    public int totalhands = 1;
    public string[] symbolnames;
    public Text showpowername;

    public void Start()
    {
        string[] tempdata = PlayerPrefs.GetString("ihaledata").Split(","[0]);

        if (tempdata.Length > 1)
        {
            for (int i = 0; i < tempdata.Length; ++i)
            {
                gamedata[i] = int.Parse(tempdata[i]);
            }
            hand = gamedata[0] + 1;
            turn = gamedata[0] % 4;
            prevpoints[0] = gamedata[1];
            prevpoints[1] = gamedata[2];
            prevpoints[2] = gamedata[3];
            prevpoints[3] = gamedata[4];
            totalhands = gamedata[5];
            StartCoroutine(starting());
        }
        else
        {
            startmenu.SetActive(true);
        }

    }

    public void hizlioyun()
    {
        turn = Random.Range(0, players.Length);
        StartCoroutine(starting());
    }

    public void besel(int val)
    {
        PlayerPrefs.SetString("ihaledata", "1,0,0,0,0," + val);
        totalhands = val;
        hand = 1;
        StartCoroutine(starting());
    }

    public void startit()
    {
        turner.SetActive(true);
        placeturner();
        players[turn].SendMessage("play");
    }


    IEnumerator starting()
    {

        while (cards.Count > 0)
        {
            int rand = Random.Range(0, cards.Count);
            GameObject tempobj = Instantiate(cardobj, transform.position, transform.rotation) as GameObject;
            Card tempcard = tempobj.GetComponent<Card>();
            tempcard.number = cards[rand].number;
            tempcard.type = cards[rand].type;
            tempcard.rend.sprite = cards[rand].image;
            tempcard.normal = cards[rand].image;
            players[turn].SendMessage("addinfo", tempcard);
            cards.RemoveAt(rand);
            if (Sound.sound == 0)
                audio.PlayOneShot(slide, 0.5f);
            yield return new WaitForSeconds(0.1f);
            turn += 1;
            if (turn == players.Length)
                turn = 0;
        }

        menu.SetActive(true);
        StartCoroutine("chooser");

    }

    public IEnumerator chooser()
    {

        int[] bestpossible;
        if (turn != 0)
        {
            bestpossible = Cardstatic.calculatebesttypeandhand(coms[turn].cards);
            roundchanger(turn, 4, bestpossible[0]);
        }
        else
        {
            roundchanger(0, 4, 0);
        }
        turn += 1;
        if (turn == players.Length)
            turn = 0;

        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < 4; ++i)
        {

            if (!ihaleon && i == 3 && turn != 0)
                continue;
            if (turn == 0)
            {
                ihaletext.SetActive(true);
                while (!playervoted)
                {
                    yield return new WaitForSeconds(0.1f);
                }
                ihaletext.SetActive(false);
            }
            else
            {
                bestpossible = Cardstatic.calculatebesttypeandhand(coms[turn].cards);
                roundchanger(turn, bestpossible[1], bestpossible[0]);
            }
            turn += 1;
            if (turn == players.Length)
                turn = 0;
            if (Sound.sound == 0)
                audio.PlayOneShot(slide, 0.5f);
            yield return new WaitForSeconds(1.0f);
        }


        if (roundwinner != 0)
        {
            turn = roundwinner;
            powercardtype = aimtypes[turn];
            symbols[powercardtype].SetActive(true);
            menu.SetActive(false);
            showpowername.text = "Koz  " + symbolnames[powercardtype];
            showpowername.gameObject.SetActive(true);
            yield return new WaitForSeconds(2.0f);
            showpowername.gameObject.SetActive(false);
            if (aims[roundwinner] > 9)
                pointlabels[roundwinner].fontSize = 25;
            pointlabels[roundwinner].text = "" + points[roundwinner] + "/" + aims[roundwinner];
            startit();
        }
        else
            showsymbols.SetActive(true);
    }

    public void roundchanger(int tempplayer, int tempnumber, int temptype)
    {

        aims[tempplayer] = tempnumber;
        aimtypes[tempplayer] = temptype;
        if (roundwinner == -1 || aims[tempplayer] > aims[roundwinner] || (tempplayer == roundwinner && tempplayer == 0))
        {
            if (tempplayer == roundwinner && tempplayer == 0 && tempnumber == 0)
                aims[tempplayer] = 4;
            if (roundwinner != -1)
                ihaleon = true;
            roundwinner = tempplayer;
        }
        else
        {
            ihale[tempplayer].text = "İhaleye Girmedi";
            return;
        }

        ihale[tempplayer].text = "" + aims[tempplayer];
        cancelbuttons(aims[tempplayer] - 4);

    }

    public void noplay()
    {
        roundchanger(0, 0, 0);
        playervoted = true;
    }

    public void powercardshower(int temptype)
    {
        powercardtype = temptype;
        turn = 0;
        menu.SetActive(false);
        symbols[powercardtype].SetActive(true);
        if (aims[roundwinner] > 9)
            pointlabels[roundwinner].fontSize = 25;
        pointlabels[roundwinner].text = "" + points[roundwinner] + "/" + aims[roundwinner];
        startit();
    }

    void cancelbuttons(int canceled)
    {
        for (int i = 0; i < canceled; ++i)
        {
            ihalebuttons[i].SetActive(false);
        }
    }



    void placeturner()
    {
        turner.transform.localPosition = turnerplacements[turn];
        turner.transform.localEulerAngles = new Vector3(0, 0, 90 * turn);
    }

    public IEnumerator winner(int curwinner)
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < middle.cards.Count; ++i)
        {
            middle.cards[i].transform.parent = transform;
            usedcards.Add(middle.cards[i]);
            middle.cards[i].rend.renderer.sortingOrder -= 10;
            iTween.MoveTo(middle.cards[i].gameObject, iTween.Hash("x", players[curwinner].transform.position.x * 2, "y", players[curwinner].transform.position.y * 2, "z", -10, "easeType",  "easeOutQuad", "time", 0.50f));
        }

        middle.clearcards();
        points[curwinner] += 1;

        if (curwinner == roundwinner)
        {
            if (points[curwinner] > 9)
                pointlabels[curwinner].fontSize = 25;
            pointlabels[curwinner].text = "" + points[curwinner] + "/" + aims[curwinner] ;
        }
        else
            pointlabels[curwinner].text = "" + points[curwinner];
        turnfinished(curwinner - 1);
    }

    public void turnfinished(int curturn)
    {
        turn = curturn;
        turn += 1;
        if (turn == players.Length)
        {
            turn = 0;
        }
        placeturner();
        turner.transform.localEulerAngles = new Vector3(0, 0, 90 * turn);
        players[turn].SendMessage("play");
    }



    public void exit()
    {

        Application.LoadLevel(0);
    }

    public void newgame()
    {
        Application.LoadLevel(2);
    }

    public void gameended()
    {

        for (int i = 0; i < points.Length; ++i)
        {
            if (i == roundwinner)
            {
                if (aims[i] > points[i] || aims[i] <= points[i] - 3)
                    points[i] = -aims[i] * 10;
                else if (aims[i] <= points[i])
                    points[i] = aims[i] * 10 + points[i] - aims[i];
            }
            if (Mathf.Sign(points[i]) == -1)
                scores[i].color = new Color(188f / 255f, 0, 0);
            else if (points[i] > 20)
                scores[i].color = new Color(0, 188f / 255f, 0);
            scores[i].text = "" + (points[i] + prevpoints[i]);
        }
        if (hand != totalhands)
        {
            string[] tempdata = PlayerPrefs.GetString("ihaledata").Split(","[0]);
            if (tempdata.Length > 1)
            {
                PlayerPrefs.SetString("ihaledata", "" + hand + "," + (points[0] + prevpoints[0]) + "," + (points[1] + prevpoints[1]) + "," + (points[2] + prevpoints[2]) + "," + (points[3] + prevpoints[3]) + "," + totalhands);
                handstatewrite();
            }
        }
        else
        {
            int temprank = Cardstatic.findtherank(points, prevpoints);
            PlayerPrefs.DeleteKey("ihaledata");
            finishedstate.text = "" + temprank + ". OLDUNUZ";
            if (temprank > 2)
            {
                if (Sound.sound == 0)
                    audio.PlayOneShot(lose);
            }
            else
            {
                if (Sound.sound == 0)
                    audio.PlayOneShot(win);
            }
            handstatewrite();
            finishedstate.gameObject.SetActive(true);
        }

        endmenu.SetActive(true);



    }

    void handstatewrite()
    {
        handstate.gameObject.SetActive(true);
        handstate.text = "" + hand + ". EL BİTTİ";
    }

    public void changesound()
    {
        if (Sound.sound == 0)
        {
            PlayerPrefs.SetInt("sound", 1);
            Sound.sound = 1;
        }
        else
        {
            audio.Play();
            Sound.sound = 0;
            PlayerPrefs.SetInt("sound", 0);
        }
    }

    public void playsound()
    {
        if (Sound.sound == 0)
            audio.Play();
    }


}

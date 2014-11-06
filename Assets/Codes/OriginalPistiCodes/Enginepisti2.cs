using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class Enginepisti2 : MonoBehaviour
{

    public GameObject cardobj;
    public GameObject[] players;
    public int[] prevpoints;
    public int[] points;
    public int hand;
    public int turn = 0;
    public int powercardtype = 0;
    public Middlepisti2 middle;
    public bool powercardopened = false;
    public Text[] scoresnow;
    public Text[] scores;
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

    public List<Card> curcards = new List<Card>();

    [System.Serializable]
    public class arrayData2
    {
        public List<Card> woncards = new List<Card>();
    }
    public arrayData2[] usedcards;

    public AudioClip slide;
    public AudioClip buzz;
    public AudioClip win;
    public AudioClip lose;
    public GameObject startmenu;
    public GameObject endmenu;
    public int[] gamedata;
    public GameObject turner;
    public Vector3[] turnerplacements;
    public int totalhands = 1;

    [System.Serializable]
    public class arrayData3
    {
        public int pisti;
        public int sinek2;
        public int karo10;
        public int vale;
        public int ace;
        public int most;
        public int sinek2points;
        public int karo10points;
        public int valepoints;
        public int acepoints;
        public int mostpoints;
    }
    public arrayData3[] results;
    bool firstround;
    int lastwinner;

    public void Start()
    {
        string[] tempdata = PlayerPrefs.GetString("pistidata").Split(","[0]);

        if (tempdata.Length > 1)
        {
            for (int i = 0; i < tempdata.Length; ++i)
            {
                gamedata[i] = int.Parse(tempdata[i]);
            }
            hand = gamedata[0] + 1;
            turn = gamedata[0] % 2;
            prevpoints[0] = gamedata[1];
            prevpoints[1] = gamedata[2];
            totalhands = gamedata[3];
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
        PlayerPrefs.SetString("pistidata", "0,0,0,0,0," + val);
        totalhands = val;
        hand = 1;
        StartCoroutine(starting());
    }

    public void startit()
    {
        players[turn].SendMessage("play");
    }


    IEnumerator starting()
    {
        Card tempcard = null;

        while (cards.Count > 0)
        {
            int rand = Random.Range(0, cards.Count);
            GameObject tempobj = Instantiate(cardobj, (new Vector3(-4.5f, 2.5f, 0)), transform.rotation) as GameObject;
            tempobj.transform.parent = transform;
            tempcard = tempobj.GetComponent<Card>();

            tempcard.number = cards[rand].number;
            tempcard.type = cards[rand].type;
            tempcard.normal = cards[rand].image;
            curcards.Add(tempcard);
            if (curcards.Count > 1)
                tempcard.rend.enabled = false;
            cards.RemoveAt(rand);
            tempcard.rend.sortingOrder = curcards.Count - 1;
        }
        while (curcards.Count > 48)
        {
            tempcard = curcards[curcards.Count - 1];
            tempcard.rend.enabled = true;
            StartCoroutine(middle.addground(tempcard));
            if (middle.cards.Count == 4)
            {

                tempcard.rend.sprite = tempcard.normal;
                bool openit = false;
                for (int e = 3; e > -1; --e)
                {
                    if (e == 3)
                        openit = true;
                    if (middle.cards[e].number == 11 && openit)
                    {
                        yield return new WaitForSeconds(0.5f);
                        if (e == 0 && middle.cards[e - 1].rend.sprite == middle.cards[e - 1].normal)
                        {
                            yield return new WaitForSeconds(1.0f);
                            newgame();
                            break;
                        }
                        middle.cards[e - 1].rend.sprite = middle.cards[e - 1].normal;

                    }
                    else
                        openit = false;
                }
            }
            tempcard.pisticount = 52 - curcards.Count;
            curcards.Remove(tempcard);
            if (Sound.sound == 0)
                audio.PlayOneShot(slide, 0.5f);
            yield return new WaitForSeconds(0.3f);
        }
        yield return StartCoroutine("dealhand");

        turner.SetActive(true);
        placeturner();

    }

    public IEnumerator dealhand()
    {
        int counter = players.Length * 4;
        if (curcards.Count == 0)
        {
            turn = lastwinner;
            yield return StartCoroutine(winner());
            yield return new WaitForSeconds(0.5f);
            gameended();
            yield break;
        }
        while (counter != 0)
        {
            Card tempcard = curcards[curcards.Count - 1];
            tempcard.rend.enabled = true;
            players[turn].SendMessage("addinfo", tempcard);
            tempcard.pisticount = 52 - curcards.Count;
            curcards.Remove(tempcard);
            if (Sound.sound == 0)
                audio.PlayOneShot(slide, 0.5f);
            turn += 1;
            if (turn == players.Length)
                turn = 0;
            counter -= 1;
            yield return new WaitForSeconds(0.1f);
        }
        startit();
    }

    void placeturner()
    {
        turner.transform.localPosition = turnerplacements[turn];
        turner.transform.localEulerAngles = new Vector3(0, 0, 180 * turn);
    }

    public IEnumerator winner()
    {
        yield return new WaitForSeconds(0.5f);
        int placement = 0;
        lastwinner = turn;

        if (turn == 0)
        {
            placement = -15;
        }
        if (turn == 0)
        {
            if (!firstround)
            {

                for (int i = 0; i < 4; ++i)
                {
                    middle.cards[i].turntaken = turn % 2;
                    middle.cards[i].rend.sprite = middle.cards[i].normal;
                    iTween.RotateTo(middle.cards[i].gameObject, iTween.Hash("x", 0, "y", 0, "z", 0, "easeType",  "easeOutQuad",  "time", 0.25f));
                    iTween.MoveTo(middle.cards[i].gameObject, iTween.Hash("x", -2.25f + i * 1.5f, "y", 5, "z", 0, "easeType",  "easeOutQuad", "islocal", true, "time", 0.25f));
                }
                yield return new WaitForSeconds(2.2f);
            }
        }

        firstround = true;
        if (middle.cards.Count == 2)
        {
            results[turn].pisti += 10;
        }

        for (int i = 0; i < middle.cards.Count; ++i)
        {
            middle.cards[i].transform.parent = players[turn].transform;
            usedcards[turn].woncards.Add(middle.cards[i]);
            middle.cards[i].rend.renderer.sortingOrder -= 10;
            iTween.RotateTo(middle.cards[i].gameObject, iTween.Hash("x", 0, "y", 0, "z", 0, "easeType",  "easeOutQuad",  "time", 0.50f));
            iTween.MoveTo(middle.cards[i].gameObject, iTween.Hash("x", 0, "y", placement, "z", 0, "easeType",  "easeOutQuad", "islocal", true, "time", 0.50f));
        }

        middle.cards.Clear();
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
        turner.transform.localEulerAngles = new Vector3(0, 0, 180 * turn);
        players[turn].SendMessage("play");
    }



    public void exit()
    {

        Application.LoadLevel(0);
    }

    public void newgame()
    {
        Application.LoadLevel(4);
    }

    public void gameended()
    {
        int[] tempmost = new int[usedcards.Length];
        for (int a = 0; a < usedcards.Length; ++a)
        {
            for (int e = 0; e < usedcards[a].woncards.Count; ++e)
            {

                if (usedcards[a].woncards[e].number == 2 && usedcards[a].woncards[e].type == 2)
                {
                    results[a].sinek2 += 1;
                }
                else if (usedcards[a].woncards[e].number == 10 && usedcards[a].woncards[e].type == 0)
                {
                    results[a].karo10 += 1;
                }
                else if (usedcards[a].woncards[e].number == 11)
                {
                    results[a].vale += 1;
                }
                else if (usedcards[a].woncards[e].number == 14)
                {
                    results[a].ace += 1;
                }
                tempmost[a] += 1;
            }

        }
        results[Cardstatic.findbiggest(tempmost)].most = 1;

        for (int a = 0; a < usedcards.Length; ++a)
        {
            results[a].sinek2 *= results[a].sinek2points;
            results[a].karo10 *= results[a].karo10points;
            results[a].vale *= results[a].valepoints;
            results[a].ace *= results[a].acepoints;
            results[a].most *= results[a].mostpoints;
            points[a] = results[a].sinek2 + results[a].karo10 + results[a].vale + results[a].ace + results[a].most + results[a].pisti;
        }
        scoresnow[0].text = "Sonuç : " + (points[0]);
        scoresnow[1].text = "Sonuç : " + (points[1]);
        scores[0].text = "Toplam Puan : " + (prevpoints[0] + points[0]);
        scores[1].text = "Toplam Puan : " + (prevpoints[1] + points[1]);

        if (hand != totalhands)
        {
            string[] tempdata = PlayerPrefs.GetString("pistidata").Split(","[0]);
            if (tempdata.Length > 1)
            {
                PlayerPrefs.SetString("pistidata", "" + hand + "," + (prevpoints[0] + points[0]) + "," + (prevpoints[1] + points[1]) + "," + totalhands);
                handstatewrite();
            }
        }
        else
        {
            int temprank = ((prevpoints[0] + points[0]) > (prevpoints[1] + points[1])) ? 1 : 2;
            PlayerPrefs.DeleteKey("pistidata");
            finishedstate.text = "" + temprank + ". OLDUNUZ";
            if (temprank > 1)
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

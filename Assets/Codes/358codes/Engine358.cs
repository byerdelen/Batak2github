using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class Engine358 : MonoBehaviour
{

    public GameObject cardobj;
    public GameObject[] players;
    public Player358 player;
    public AI358[] coms;
    public int[] aims;
    public int[] prevpoints;
    public int[] points;
    public int[] debts;
    public int hand;
    public int turn = 0;
    public int powercardtype = 0;
    public Middle358 middle;
    public bool powercardopened = false;
    public bool powercardchosen = false;
    public Text[] scores;
    public Text[] scorestotal;
    public Text[] pointlabels;
    public Text[] ihale;
    public GameObject menu;
    public GameObject kozsecin;
    public GameObject[] showsymbols;
    public Text finishedstate;
    public Text handstate;
    public Text kartsecme;
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
    public Vector3[] turnerrotations;
    public int totalhands = 1;
    public string[] symbolnames;
    public Text showpowername;
    public bool nodebt = false;

    public void Start()
    {
        string[] tempdata = PlayerPrefs.GetString("358data").Split(","[0]);

        if (tempdata.Length > 1)
        {
            for (int i = 0; i < tempdata.Length; ++i)
            {
                gamedata[i] = int.Parse(tempdata[i]);
            }
            hand = gamedata[0] + 1;
            turn = hand % 3;
            prevpoints[0] = gamedata[1];
            prevpoints[1] = gamedata[2];
            prevpoints[2] = gamedata[3];
            if (((gamedata[0] ) % 4) != 0)
            {

                debts[0] = gamedata[4];
                debts[1] = gamedata[5];
                debts[2] = gamedata[6];
            }
            else
                nodebt = true;
            totalhands = gamedata[7];
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
        PlayerPrefs.SetString("358data", "1,0,0,0,0,0,0," + val);
        hand = 1;
        turn = 1;
        totalhands = val;
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

            if (middle.buriedcards.Count < 4)
            {
                StartCoroutine(middle.addburiedcard(tempcard));
                if (middle.buriedcards.Count == 4)
                    turn -= 1;
            }
            else
                players[turn].SendMessage("addinfo", tempcard);
            cards.RemoveAt(rand);
            if (Sound.sound == 0)
                audio.PlayOneShot(slide, 0.5f);
            yield return new WaitForSeconds(0.1f);


            if (middle.buriedcards.Count == 4)
            {
                turn += 1;
            }

            if (turn == players.Length)
                turn = 0;

        }

        aims[turn] = 3;
        aims[(turn + 1) % 3] = 8;
        aims[(turn + 2) % 3] = 5;
        for (int i = 0; i < aims.Length; ++i)
        {
            pointlabels[i].text = "0/" + aims[i];

            if (debts[i] > 0)
                ihale[i].text = "İhale : " + aims[i] + "        Borç : +" + debts[i];
            else
                ihale[i].text = "İhale : " + aims[i] + "        Borç : " + debts[i];
        }
        if (nodebt)
        {
            showpowername.text = "Önceki el borçları silindi";
            showpowername.gameObject.SetActive(true);
            yield return new WaitForSeconds(2.0f);
            showpowername.gameObject.SetActive(false);
        }
        menu.SetActive(true);
        while (menu.activeSelf)
            yield return null;
        yield return StartCoroutine("sharedebts");
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(choosepowercard());
        while (!powercardchosen)
            yield return null;
        yield return StartCoroutine("sharetablecards");
        startit();
    }

    public IEnumerator choosepowercard()
    {
        if (aims[0] == 8)
            kozsecin.SetActive(true);
        else
        {
            int[] bestpossible;
            int chooser = aims[1] == 8  ? 1 : 2;

            bestpossible = Cardstatic.calculatebesttypeandhand(coms[chooser].cards);
            turn = chooser;
            powercardtype = bestpossible[0];
            showsymbols[powercardtype].SetActive(true);
            showpowername.text = "Koz  " + symbolnames[powercardtype];
            showpowername.gameObject.SetActive(true);
            yield return new WaitForSeconds(2.0f);
            showpowername.gameObject.SetActive(false);
            powercardchosen = true;
        }
    }

    public void powercardshower(int temptype)
    {
        powercardtype = temptype;
        powercardchosen = true;
    }

    public IEnumerator sharetablecards()
    {
        //print("yes1");
        for (int i = 0; i < aims.Length; ++i)
        {
            if (aims[i] == 8)
            {
                yield return StartCoroutine(exchangetable(i));
                break;
            }

        }
    }

    public IEnumerator sharedebts()
    {

        List<int> order = new List<int>()
        {
            8, 5, 3
        };
        for (int i = 0; i < aims.Length; ++i)
        {
            if (aims[i] == 8)
                order[0] = i;
            else if (aims[i] == 5)
                order[1] = i;
            else if (aims[i] == 3)
                order[2] = i;
        }

        for (int i = 0; i < aims.Length; ++i)
        {

            if (aims[order[i]] == 8)
            {

                if (debts[order[i]] > 0)
                {
                    for (int a = 0; a < aims.Length; ++a)
                    {
                        if (order[i] == a)
                            continue;
                        if (aims[a] == 3)
                        {
                            if (debts[a] < 0 && debts[order[i]] > 0)
                            {
                                yield return StartCoroutine(exchangecards(order[i], a));
                            }
                        }
                        if (aims[a] == 5)
                        {

                            if (debts[a] < 0 && debts[order[i]] > 0)
                            {

                                yield return StartCoroutine(exchangecards(order[i], a));
                            }
                        }
                    }
                }

            }
            else if (aims[order[i]] == 5)
            {

                if (debts[order[i]] > 0)
                {
                    for (int a = 0; a < aims.Length; ++a)
                    {
                        if (order[i] == a)
                            continue;
                        if (aims[a] == 3)
                        {
                            if (debts[a] < 0 && debts[order[i]] > 0)
                            {
                                yield return StartCoroutine(exchangecards(order[i], a));
                            }
                        }
                        if (aims[a] == 8)
                        {
                            if (debts[a] < 0 && debts[order[i]] > 0)
                            {
                                yield return StartCoroutine(exchangecards(order[i], a));
                            }
                        }
                    }
                }

            }
            else if (aims[order[i]] == 3)
            {

                if (debts[order[i]] > 0)
                {
                    for (int a = 0; a < aims.Length; ++a)
                    {
                        if (order[i] == a)
                            continue;
                        if (aims[a] == 5)
                        {
                            if (debts[a] < 0 && debts[order[i]] > 0)
                            {
                                yield return StartCoroutine(exchangecards(order[i], a));
                            }
                        }
                        if (aims[a] == 8)
                        {
                            if (debts[a] < 0 && debts[order[i]] > 0)
                            {
                                yield return StartCoroutine(exchangecards(order[i], a));
                            }
                        }
                    }
                }

            }
        }
    }

    public IEnumerator exchangecards(int plusside, int minusside)
    {
        yield return new WaitForSeconds(1.0f);
        List<Card>  curcards = new List<Card>();
        Card tempcard = new Card();
        int cardcounter = (Mathf.Abs(debts[minusside]) > debts[plusside])  ? debts[plusside] : Mathf.Abs(debts[minusside]);
        while (debts[minusside] < 0 && debts[plusside] > 0)
        {

            if (plusside == 0)
            {
                kartsecme.gameObject.SetActive(true);

                kartsecme.text = "Borcunuzu Almak İçin\n" + cardcounter + " Kart Seçiniz";
                player.choosecardtoexchange(true);
                while (player.exchangecard == null)
                    yield return null;
                tempcard = player.exchangecard;
                player.choosecardtoexchange(false);
                player.exchangecard = null;
            }
            else
                tempcard = Cardstatic.findsmallest(coms[plusside].cards);

            if (minusside == 0)
                player.addinfo(tempcard);
            else
                StartCoroutine(coms[minusside].addinfo(tempcard));
            if (Sound.sound == 0)
                audio.PlayOneShot(slide, 0.5f);
            yield return new  WaitForSeconds(0.3f);

            if (plusside == 0)
            {
                player.cards.Remove(tempcard);
                player.placethem();
            }
            else
                coms[plusside].cards.Remove(tempcard);

            debts[minusside] += 1;
            debts[plusside] -= 1;
            cardcounter -= 1;
            curcards.Add(tempcard);
        }
        kartsecme.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.0f);

        for (int i = curcards.Count - 1; i > -1; --i)
        {

            if (minusside == 0)
                curcards[i] = player.cards[Cardstatic.findbiggestofthetype(curcards[i].type, true, player.cards)];
            else
                curcards[i] = coms[minusside].cards[Cardstatic.findbiggestofthetype(curcards[i].type, true, coms[minusside].cards)];


            if (plusside == 0)
                player.addinfo(curcards[i]);
            else
                StartCoroutine(coms[plusside].addinfo(curcards[i]));
            if (Sound.sound == 0)
                audio.PlayOneShot(slide, 0.5f);
            yield return new  WaitForSeconds(0.3f);
            if (minusside == 0)
            {
                player.cards.Remove(curcards[i]);
                player.placethem();
            }
            else
                coms[minusside].cards.Remove(curcards[i]);
        }
    }

    public IEnumerator exchangetable(int eighter)
    {
        yield return new WaitForSeconds(1.0f);
        List<Card>  curcards = new List<Card>();
        Card tempcard = new Card();
        int cardcounter = 4;
        for (int i = 0; i < 4; ++i)
        {
            curcards.Add(middle.buriedcards[i]);
        }

        for (int i = 0; i < 4; ++i)
        {
            if (eighter == 0)
            {
                player.choosecardtoexchange(true);
                kartsecme.gameObject.SetActive(true);

                kartsecme.text = "Masadan Almak İçin\n" + (4 - i) + " Kart Seçiniz";
                while (player.exchangecard == null)
                    yield return null;
                tempcard = player.exchangecard;
                player.choosecardtoexchange(false);
                player.exchangecard = null;
            }
            else
                tempcard = Cardstatic.findsmallest(coms[eighter].cards);

            StartCoroutine(middle.addburiedcard(tempcard));

            yield return new  WaitForSeconds(0.3f);

            if (eighter == 0)
            {
                player.cards.Remove(tempcard);
                player.placethem();
            }
            else
                coms[eighter].cards.Remove(tempcard);

        }
        kartsecme.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.0f);

        for (int i = curcards.Count - 1; i > -1; --i)
        {

            if (eighter == 0)
                player.addinfo(curcards[i]);
            else
                StartCoroutine(coms[eighter].addinfo(curcards[i]));
            if (Sound.sound == 0)
                audio.PlayOneShot(slide, 0.5f);
            yield return new  WaitForSeconds(0.3f);
            middle.buriedcards.Remove(curcards[i]);
        }
    }


    void placeturner()
    {
        turner.transform.localPosition = turnerplacements[turn];
        turner.transform.localEulerAngles = turnerrotations[turn];
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
        if (points[curwinner] > 9)
            pointlabels[curwinner].fontSize = 25;
        pointlabels[curwinner].text = "" + points[curwinner] + "/" + aims[curwinner] ;
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
        turner.transform.localEulerAngles = turnerrotations[turn];
        players[turn].SendMessage("play");
    }



    public void exit()
    {

        Application.LoadLevel(0);
    }

    public void newgame()
    {
        Application.LoadLevel(3);
    }

    public void gameended()
    {

        for (int i = 0; i < points.Length; ++i)
        {


            points[i] = points[i] - aims[i];
            debts[i] = points[i];
            if (Mathf.Sign(points[i]) == -1)
                scores[i].color = new Color(188f / 255f, 0, 0);
            else
                scores[i].color = new Color(0, 188f / 255f, 0);
            scorestotal[i].text = "Toplam Puan : " + (points[i] + prevpoints[i]);
            scores[i].text = "Sonuç : " + points[i];
        }
        if (hand != totalhands)
        {
            string[] tempdata = PlayerPrefs.GetString("358data").Split(","[0]);
            if (tempdata.Length > 1)
            {
                PlayerPrefs.SetString("358data", "" + hand + "," + (points[0] + prevpoints[0]) + "," + (points[1] + prevpoints[1]) + "," + (points[2] + prevpoints[2]) + "," + (debts[0]) + "," + (debts[1]) + "," + (debts[2]) + "," + totalhands);
                handstatewrite();
            }
        }
        else
        {
            int temprank = Cardstatic.findtherank358(points, prevpoints);
            PlayerPrefs.DeleteKey("358data");
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OkeyPlayer : MonoBehaviour
{

    public    bool canplay = false;
    public List<Stone> cards;
    public List<OkeyPlace> place;
    public OkeyEngine engine;
    public Stone holdedcard;
    public bool taken = false;
    public bool started = false;

    [System.Serializable]
    public class arrayData
    {
        public List<int> rows = new List<int>();
    }
    public List<arrayData> series = new List<arrayData>();



    void addinfo(Stone tempcard)
    {
        cards.Add(tempcard);
        tempcard.transform.parent = transform;
        sortcards();
    }

    void sortcards()
    {
        cards.Sort(delegate(Stone t2, Stone t1)
        {
            return (t2.type * 14 + t2.number).CompareTo(t1.type * 14 + t1.number);
        });
        placethem();
    }

    void placethem()
    {
        for (int i = 0; i < cards.Count; ++i)
        {
            //iTween.MoveTo(cards[i].gameObject, iTween.Hash("x", -1.3f * Mathf.Floor(cards.Count / 2.0f) + i * 1.3f, "y", -Mathf.Abs(i - Mathf.Floor(cards.Count / 2.0f)) * 0.08f, "z", -i * 0.01f, "islocal", true,  "easeType",  "easeOutQuad", "time", 0.3f));
            cards[i].transform.parent = place[i].transform;
            place[i].card = cards[i];
            place[i].card.layering(false);
            iTween.MoveTo(cards[i].gameObject, iTween.Hash("x", place[i].transform.position.x, "y", place[i].transform.position.y, "z", place[i].transform.position.z,  "easeType",  "easeOutQuad", "time", 0.3f));
            //iTween.RotateTo(cards[i].gameObject, iTween.Hash("x", 0, "y", 0, "z", Mathf.Floor(cards.Count / 2.0f) * 2.0f - i * 2.0f, "islocal", true,  "easeType",  "easeOutQuad", "time", 0.1f));

        }
    }


    void play()
    {
        canplay = true;

    }

    void canmove()
    {
        started = true;
    }


    void Update ()
    {
        if (started)
            button();
        restart();
    }

    void restart()
    {
        if (Input.touchCount == 4)
            engine.newgame();
    }


    void button()
    {
        if (!holdedcard)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 1000);
                if (hit)
                {
                    //check the card

                    checkthetaken(hit.transform);
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 1000);
                if (hit)
                {
                    checktheleft(hit.transform, hit.point);
                }
                else
                    returnit();
            }
            else if (Input.GetMouseButton(0))
            {

                holdedcard.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            }
        }

    }

    void returnit()
    {
        moveit(holdedcard.gameObject, holdedcard.transform.parent);
        print("returningback");
    }

    void moveit(GameObject movedpiece, Transform movedplace)
    {
        iTween.MoveTo(movedpiece, iTween.Hash("x", movedplace.position.x, "y", movedplace.position.y, "z", movedplace.position.z,  "easeType",  "easeOutQuad", "time", 0.2f));
        if (holdedcard)
            StartCoroutine("ordering");
        holdedcard = null;
    }

    IEnumerator ordering()
    {
        Stone tempcard = holdedcard;
        yield return new WaitForSeconds(0.2f);
        tempcard.layering(false);
    }


    void checkthetaken(Transform obj)
    {

        if (obj.tag == "places")
        {
            OkeyPlace tempholder = obj.GetComponent<OkeyPlace>();
            if (tempholder.card)
            {
                holdedcard = tempholder.card;
                holdedcard.layering(true);
                print("takenfromboard");
            }
            return;
        }


        if (canplay)
        {
            if (!taken)
            {
                if (obj == engine.sides[0].transform && engine.sides[0].cards.Count > 0)
                {

                    OkeySides tempholder = obj.GetComponent<OkeySides>();
                    holdedcard = tempholder.cards[tempholder.cards.Count - 1];
                    holdedcard.layering(true);

                    print("takenfromside");
                    return;
                }
                else if (obj == engine.middle.transform)
                {

                    OkeyMiddleholder tempholder = obj.GetComponent<OkeyMiddleholder>();
                    holdedcard = tempholder.cards[tempholder.cards.Count - 1];
                    holdedcard.back.renderer.enabled = true;
                    holdedcard.layering(true);

                    print("takenfrommiddle");
                    return;
                }
            }
        }



    }




    void checktheleft(Transform obj, Vector3 point)
    {

        if (obj == holdedcard.transform.parent)
        {
            moveit(holdedcard.gameObject, holdedcard.transform.parent);
            return;
        }

        if (obj.tag == "places")
        {

            if (!checkplacement(obj))
            {
                print("itsfull");
                returnit();
                return;
            }
            else
            {
                if (holdedcard.transform.parent == engine.sides[0].transform || holdedcard.transform.parent == engine.middle.transform)
                    taken = true;
                processplacement(obj, point);
            }

            return;
        }




        if (canplay)
        {

            if (taken)
            {

                if (obj == engine.sides[1].transform)
                {
                    taken = false;
                    engine.sides[1].putstone(holdedcard);
                    holdedcard = null;
                    print("puttingtoside");
                    StartCoroutine("endturn");
                    return;
                }
                else if (obj == engine.middle.transform)
                {
                    if (checkfinishingconditions(holdedcard))
                        engine.newgame();
                    else
                    {
                        returnit();
                    }
                    return;
                }
            }

        }

        returnit();

    }

    bool checkfinishingconditions(Stone minuscard)
    {
        series.Clear();
        bool seriestarted = false;
        int[] tempplacements = new int[24];

        for (int i = 0; i < place.Count; ++i)
        {
            if (place[i].card == null || place[i].card == minuscard)
            {
                tempplacements[i] = 0;
            }
            else
                tempplacements[i] = place[i].card.number + 13 * place[i].card.type;
        }

        for (int i = 0; i < tempplacements.Length; ++i)
        {

            if (tempplacements[i] == 0)
            {
                if (seriestarted)
                {
                    seriestarted = false;
                }
            }
            else
            {
                if (!seriestarted)
                {
                    seriestarted = true;
                    series.Add(new arrayData());
                }
                series[series.Count - 1].rows.Add(tempplacements[i]);
            }
            if (i == 11)
            {
                seriestarted = false;
            }
        }

        //checkifthereissmaller
        for (int i = 0; i < series.Count; ++i)
        {
            if (series[i].rows.Count < 3)
            {
                print("fail-seriesarelessthan3");
                return false;
            }
        }


        for (int i = 0; i < series.Count; ++i)
        {

            if (checkifitisacolumn(i))
            {
                print(i + "isavalidcolumn");
                continue;
                /*
                if (checkthecolumn(i))
                {
                    print(i + "isavalidcolumn");
                    continue;
                }
                */
            }
            else if (checkifitisarow(i))
            {
                print(i + "isarow");
                if (checktherow(i))
                {
                    print(i + "isavalidrow");
                    continue;
                }
            }

            return false;

        }


        return true;
    }

    bool checkifitisarow(int i)
    {
        int basenum = 100;
        for (int a = 0; a < series[i].rows.Count; ++a)
        {

            if (basenum == 100)
            {
                if (series[i].rows[a] == 52)
                    continue;
                else
                {
                    basenum = (Mathf.FloorToInt((series[i].rows[a] - 1) / 13.0f));
                    continue;
                }
            }
            if (basenum != (Mathf.FloorToInt((series[i].rows[a] - 1) / 13.0f)))
            {
                if (series[i].rows[a] == 52)
                    continue;
                print(i + "" + a + "fail-it is not a row");
                return false;
            }
        }
        return true;
    }

    //(series[i].rows[a] - 1) % 13 + 1)

    bool checktherow(int i)
    {
        int basenum = 100;
        for (int a = 0; a < series[i].rows.Count; ++a)
        {
            if (basenum == 100)
            {
                if (series[i].rows[a] == 52)
                {
                    if (series[i].rows[a + 1] == 52)
                    {
                        if (((series[i].rows[a + 2] - 1) % 13 + 1) == 1)
                            series[i].rows[a + 1] = series[i].rows[a + 2] + 12;
                        else
                            series[i].rows[a + 1] = series[i].rows[a + 2] - 1;
                        series[i].rows[a] = series[i].rows[a + 1] - 1;

                    }
                    else
                    {
                        if (((series[i].rows[a + 1] - 1) % 13 + 1) == 1)
                            series[i].rows[a] = series[i].rows[a + 1] + 12;
                        else
                            series[i].rows[a] = series[i].rows[a + 1] - 1;
                    }
                }
                basenum = ((series[i].rows[a] - 1) % 13 + 1);
                continue;
            }
            else
            {
                if (series[i].rows[a] == 52)
                {
                    if (((series[i].rows[a - 1] - 1) % 13 + 1) == 13)
                        series[i].rows[a] =  series[i].rows[a - 1] - 12;
                    else
                        series[i].rows[a] =  series[i].rows[a - 1] + 1;
                }
                if (a == (series[i].rows.Count - 1) && ((series[i].rows[a - 1] - 1) % 13 + 1) == 13 && ((series[i].rows[a] - 1) % 13 + 1) == 1)
                    break;
                if (1 != ((series[i].rows[a] - 1) % 13 + 1) - ((series[i].rows[a - 1] - 1) % 13 + 1))
                {
                    if (series[i].rows[a] == 52)
                        continue;
                    print(i + "" + a + "fail-the numbers donot follow on row");
                    return false;
                }
            }
        }
        return true;
    }

    bool checkifitisacolumn(int i)
    {
        int basenum = 100;
        if (series[i].rows.Count > 4)
            return false;
        for (int a = 0; a < series[i].rows.Count; ++a)
        {
            for (int e = a + 1; e < series[i].rows.Count; ++e)
            {
                if (series[i].rows[a] == series[i].rows[e] && series[i].rows[a] != 52)
                {
                    print(i + "" + a + "fail-column numbers are the same");
                    return false;
                }
            }

            if (basenum == 100)
            {
                if (series[i].rows[a] == 52)
                    continue;
                else
                {
                    basenum = ((series[i].rows[a] - 1) % 13 + 1);
                    continue;
                }
            }
            if (basenum != ((series[i].rows[a] - 1) % 13 + 1))
            {
                if (series[i].rows[a] == 52)
                    continue;
                print(i + "" + a + "fail-the numbers do not follow on column");
                return false;
            }
        }
        return true;
    }

    /*
        bool checkifitisacolumn(int i)
        {
            int basenum = 100;
            for (int a = 0; a < series[i].rows.Count; ++a)
            {
                if (basenum == 100)
                {
                    if (series[i].rows[a] == 52)
                        continue;
                    else
                    {
                        basenum = ((series[i].rows[a] - 1) % 13 + 1);
                        continue;
                    }
                }
                if (basenum != ((series[i].rows[a] - 1) % 13 + 1))
                {
                    if (series[i].rows[a] == 52)
                        continue;
                    return false;
                }
            }
            return true;
        }
    */

    void movecards(Stone stones, OkeyPlace newplace)
    {
        if (holdedcard)
            holdedcard.transform.parent.SendMessage("remove", holdedcard);
        if (stones.transform.parent == engine.middle.transform)
            stones.SendMessage("turnit", true);
        stones.transform.parent = newplace.transform;
        moveit(stones.gameObject, newplace.transform);
        newplace.card = stones;
        holdedcard = null;
        print("puttingtoplace");
    }

    bool checkplacement(Transform obj)
    {
        OkeyPlace tempplace = obj.GetComponent<OkeyPlace>();
        if (tempplace.card != null)
        {
            if (tempplace.place > 11)
            {
                for (int i = 12; i < place.Count; ++i)
                {

                    if (place[i].card == null || place[i].card == holdedcard)
                    {
                        return true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < 12; ++i)
                {

                    if (place[i].card == null || place[i].card == holdedcard)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        else
        {
            return true;
        }
    }

    IEnumerator endturn()
    {
        canplay = false;
        yield return new WaitForSeconds(0.3f);
        engine.turnfinished(0);
    }

    void processplacement(Transform obj, Vector3 point)
    {
        OkeyPlace tempplace = obj.GetComponent<OkeyPlace>();
        int direction = -(int) Mathf.Sign(point.x - tempplace.transform.position.x);
        int placement = (tempplace.place);
        bool empty = false;
        int limit = -1;
        if (place[placement].card == null)
        {
            direction = 0;
        }
        if ( direction == -1)
        {
            if (placement > 11)
                limit = 11;
            for (int i = placement; i > limit; --i)
            {
                if (place[i].card == null || place[i].card == holdedcard)
                {
                    empty = true;
                    break;
                }
            }
            if (!empty)
            {
                direction = +1;
                placement += 1;
            }
        }
        else if ( direction == 1)
        {
            if (placement > 11)
                limit = 24;
            else
                limit = 11;
            for (int i = placement; i < limit; ++i)
            {
                if (place[i].card == null || place[i].card == holdedcard)
                {
                    empty = true;
                    break;
                }
            }
            if (!empty)
            {
                direction = -1;
                placement -= 1;
            }
        }
        if (place[placement].card == null || place[placement].card == holdedcard)
        {
            direction = 0;
        }
        if (direction == 0)
        {
            movecards(holdedcard, place[placement]);
            return;
        }
        Stone curcard = holdedcard;
        Stone oldcard;
        while (place[placement].card != null)
        {
            oldcard = place[placement].card;
            movecards(curcard, place[placement]);
            placement += direction;
            curcard = oldcard;
        }
        movecards(curcard, place[placement]);



    }

}
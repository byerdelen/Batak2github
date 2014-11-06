using UnityEngine;
using System.Collections;

public class OkeyPlace : MonoBehaviour
{

    public Stone card;
    public int place;

    void addcard(Stone tempcard)
    {

        card = tempcard;
    }

    public void remove(Stone tempcard)
    {
        card = null;
    }

}

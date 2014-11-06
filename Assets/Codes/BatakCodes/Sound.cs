using UnityEngine;
using System.Collections;

public class Sound : MonoBehaviour
{

    public static int sound = 0;

    void Awake ()
    {
        sound = PlayerPrefs.GetInt("sound");

    }
}

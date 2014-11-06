using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour
{

    void Start()
    {
        Application.targetFrameRate = 60;
        PlayerPrefs.SetString("carddata", "");
        PlayerPrefs.SetString("ihaledata", "");
        PlayerPrefs.SetString("358data", "");
        PlayerPrefs.SetString("pistidata", "");
        PlayerPrefs.SetString("eslipistidata", "");
    }

    public void batak (int level)
    {
        Application.LoadLevel(level);
        audio.Play();
    }
}

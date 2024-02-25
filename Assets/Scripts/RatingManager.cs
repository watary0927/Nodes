using System;
using System.Collections;
using UnityEngine;
using Unity;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class RatingManager : MonoBehaviour
{
    private Text text;

    void Start()
    {
        Debug.Log("Start");
        text = GetComponent<Text>();
        if (!PlayerPrefs.HasKey("Rating")) PlayerPrefs.SetInt("Rating", 1500);
        if (!PlayerPrefs.HasKey("Nowplaying")) PlayerPrefs.SetInt("Nowplaying", 0);
        ShowRating();
        if (!PlayerPrefs.HasKey("Tutorial"))
        {
            Debug.Log("Tutorial");
            PlayerPrefs.SetInt("Tutorial", 1);
                SceneManager.LoadScene("TutorialScene");
        }
        else
        {
            Debug.Log(PlayerPrefs.GetInt("Tutorial"));
        }
    }
    /*public void UpdateRating(bool result, int enemyrating)
    {
        int rating = PlayerPrefs.GetInt("Rating");
        int newrating = rating + 16 * (result ? 1 : -1) + 0.04 * (enemyrating - rating);
        PlayerPrefs.SetInt("Rating", newrating);
    }*/
    void ShowRating()
    {
        int rating = PlayerPrefs.GetInt("Rating");
        if (PlayerPrefs.GetInt("Nowplaying") == 1)
        {
            rating -= 0;
            PlayerPrefs.SetInt("Rating", rating);
            PlayerPrefs.SetInt("Nowplaying", 0);
        }
        text.text = rating.ToString();
    }
}



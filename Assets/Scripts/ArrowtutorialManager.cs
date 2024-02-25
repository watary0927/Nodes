using UnityEngine;
using System;
using UnityEngine.UI;

public class ArrowtutorialManager : MonoBehaviour
{
    public Button Arrow;
    GameObject turnmana;
    TutorialManager script;
    void Start()
    {
        turnmana = GameObject.Find("TurnManager");
        script = turnmana.GetComponent<TutorialManager>();
    }
    public void Onclick()
    {
        script.OnClick(100);
    }
}

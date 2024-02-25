using UnityEngine;
using System;
using UnityEngine.UI;

public class ArrowManager : MonoBehaviour
{
    public Button Arrow;
    GameObject turnmana;
    TurnManager script;
    void Start()
    {
        turnmana = GameObject.Find("TurnManager");
        script=turnmana.GetComponent<TurnManager>();
    }
    public void Onclick()
    {
        script.OnClick(100);
    }
}

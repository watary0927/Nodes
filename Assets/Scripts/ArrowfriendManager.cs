using UnityEngine;
using System;
using UnityEngine.UI;

public class ArrowfriendManager : MonoBehaviour
{
    public Button Arrow;
    GameObject turnmana;
    FriendturnManager script;
    void Start()
    {
        turnmana = GameObject.Find("TurnManager");
        script = turnmana.GetComponent<FriendturnManager>();
    }
    public void Onclick()
    {
        script.OnClick(100);
    }
}

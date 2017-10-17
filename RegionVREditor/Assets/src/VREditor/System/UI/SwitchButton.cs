using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class SwitchButton : MonoBehaviour
{

    public GameObject UI_TimeLine;
    public GameObject UI_NodeConnection;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (UI_TimeLine.activeSelf == true)
        {
            changeTitleAsNodeConnection();
        }
        else if (UI_NodeConnection.activeSelf == true)
        {
            changeTitleAsTimeLine();
        }
    }

    void changeTitleAsNodeConnection()
    {
        this.GetComponentInChildren<Text>().text = "Node Connection";
    }

    void changeTitleAsTimeLine()
    {
        this.GetComponentInChildren<Text>().text = "Time Line";
    }

    public void Toggle()
    {
        if (UI_TimeLine.activeSelf == true)
        {
            UI_TimeLine.SetActive(false);
            UI_NodeConnection.SetActive(true);
        }
        else if (UI_NodeConnection.activeSelf == true)
        {
            UI_NodeConnection.SetActive(false);
            UI_TimeLine.SetActive(true);
        }
    }

    

 
}

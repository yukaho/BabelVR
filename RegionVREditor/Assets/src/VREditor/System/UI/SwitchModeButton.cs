using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwitchModeButton : MonoBehaviour, IPointerClickHandler
{

    public GameObject TimeLine;
    public GameObject NodeConnection;
   
    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Toggle
        if (TimeLine.active)
        {
            this.transform.GetComponentInChildren<Text>().text = "Time Line";
            TimeLine.SetActive(false);
            NodeConnection.SetActive(true);
        }
        else
        {

           
            this.transform.GetComponentInChildren<Text>().text = "NodeConnection";
            TimeLine.SetActive(true);
            NodeConnection.SetActive(false);
        }
 
    }
}

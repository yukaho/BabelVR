using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Canvas : MonoBehaviour,IPointerDownHandler{



    public event EventHandler onClick_Action;



    public void OnPointerDown(PointerEventData eventData)
    {
       

        if (Input.GetMouseButton(0) == true)
        {
           // Debug.Log(this.name + " OnClick().");

            if (onClick_Action != null)
            {

                //Call action
                onClick_Action(this, null);
            }

        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

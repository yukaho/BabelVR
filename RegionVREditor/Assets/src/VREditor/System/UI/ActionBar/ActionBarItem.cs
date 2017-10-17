using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ActionBarItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerCancelAction
{




    //orignal color
    Color c;

    //function pointer --> function
    public event EventHandler onClick_Action;


    //Dropdown menu
    public GameObject dropdown_menu;

    public void OnPointerDown(PointerEventData eventData)
    {
        //Left Click
        if (Input.GetMouseButton(0) == true)
        {
          //  Debug.Log(this.name + " OnClick().");

            if (onClick_Action != null)
            {

                //Call action
                onClick_Action(this, null);
            }


            //restore color
            GetComponent<Image>().color = c;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        c = this.GetComponent<Image>().color;
        this.GetComponent<Image>().color = new Color(0, 1, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        this.GetComponent<Image>().color = c;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnCancel()
    {
        if (dropdown_menu != null)
        {
            dropdown_menu.SetActive(false);
        }
    }
}

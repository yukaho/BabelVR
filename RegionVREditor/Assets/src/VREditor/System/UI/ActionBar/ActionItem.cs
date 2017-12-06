#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ActionItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerCancelAction
{
    //Editor System
    RegionVREditorCore system;

    //orignal color
    Color c;

    //function pointer --> function
    public event EventHandler onClick_Action;

    void Start()
    {
        //reference core system
        system = GameObject.Find("EditorSystem").GetComponent<RegionVREditorCore>();

        //initialize functions
        initializeFunctionPointer();
    }

    void initializeFunctionPointer()
    {


        if (this.name.Equals("Open"))
        {
            this.onClick_Action = system.OnClickOpen;
        }
        else if (this.name.Equals("Save"))
        {
            this.onClick_Action = system.OnClickSave;
        }
    }

    public void OnCancel()
    {

    }


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
}
#endif
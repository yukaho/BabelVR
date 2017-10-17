using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuBtn : MonoBehaviour,IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler {

    public event EventHandler onClick_Action;



    public void OnPointerDown(PointerEventData eventData)
    {
        //Left Click
        if (Input.GetMouseButton(0) == true)
        {
            Debug.Log(this.name + " OnClick().");

            if (onClick_Action != null)
            {

                //Call action
                onClick_Action(this, null);
            }

            //Close Menu
            this.transform.parent.parent.gameObject.SetActive(false);

            //restore color
            float c = 94 / 255f;
            GetComponent<Image>().color = new Color(c, c, c);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Image>().color = new Color(0, 1, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        float c = 94 / 255f;
        GetComponent<Image>().color = new Color(c, c, c);
    }



    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

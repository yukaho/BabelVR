using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LiveViewImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{


    //Main Camera
    public Camera main_ui_cam;

    //view Camera
    public Camera view_cam;

    //Mouse Checking
    bool mouse_entered;
    Vector3 mouse_last_pos;

    // Use this for initialization
    void Start()
    {
        main_ui_cam = GameObject.Find("MainUICamera").GetComponent<Camera>();
        mouse_entered = false;

    }

    // Update is called once per frame
    void Update()
    {
        //check mouse interaction
        update_mouse();


    }

    void update_mouse()
    {
        if (mouse_entered)
        {
            //check left click
            if (Input.GetMouseButton(0))
            {

                //mouse movement offset
                Vector3 draging_angle = Input.mousePosition - mouse_last_pos;
               
                //rotate values
                float y = view_cam.transform.rotation.eulerAngles.y + draging_angle.x;
                float x = view_cam.transform.rotation.eulerAngles.x + -draging_angle.y/2;

                //rotate camera
                view_cam.transform.localRotation = Quaternion.Euler(x, y, 0);


            }
            mouse_last_pos = Input.mousePosition;
        }
        else
        {

        }
    }

    //implementation
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouse_entered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouse_entered = false;
    }
}

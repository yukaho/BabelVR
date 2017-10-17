using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NodeConnectionGraphView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    //Editor Main Core
    RegionVREditorCore EditorSystem;

    //Scrolling Panel
    public RectTransform UI_ScrollPanel;

    //Mouse Tracking
    public bool mouse_down = false;
    public Vector3 last_mouse_L_pos;
    public Vector3 last_mouse_R_pos;


    //render texture
    public RenderTexture view_texture;

    //Find Camera
    public Camera render_cam;

    //PopUp Window
    GameObject popup_menu;

    //Node List
    public List<UI_Node> NodeList;

    public void OnPointerDown(PointerEventData eventData)
    {
        //Left Click
        if (Input.GetMouseButton(0) == true)
        {
            mouse_down = true;
            last_mouse_L_pos = Input.mousePosition;
            

        }
        //Right Click
        if (Input.GetMouseButton(1) == true)
        {
            popUpMenu();
            last_mouse_R_pos = Input.mousePosition;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
        if (Input.GetMouseButton(0) == false)
        {
            mouse_down = false;
        }
    }

    //Create Pop Menu
    void popUpMenu()
    {

        if (popup_menu == null)
        {
            //Initialization
            popup_menu = Resources.Load("Prefab/UI/NodeConnection_Menu") as GameObject;
            popup_menu = Instantiate(popup_menu, new Vector3(0, 0, 0), Quaternion.identity);
            popup_menu.transform.SetParent(EditorSystem.UI_Canvas.transform);
            popup_menu.transform.localScale = new Vector3(3, 3, 3);

            //Set Function
            popup_menu.GetComponentInChildren<MenuBtn>().onClick_Action += EditorSystem.onClickCreateNode;


        }
  

        popup_menu.GetComponent<RectTransform>().position = new Vector3(Input.mousePosition.x+50, Input.mousePosition.y-50, 0);
        popup_menu.SetActive(true);


    }

    // Use this for initialization
    void Start()
    {
        //find editor core
        EditorSystem = GameObject.Find("EditorSystem").GetComponent<RegionVREditorCore>();

        //create new node list
        NodeList = new List<UI_Node>();


        if (view_texture != null)
        {
    
            //set virtual view has same size with window
            EditorSystem.UI_ConnectionVirtualView.GetComponent<RectTransform>().sizeDelta = this.GetComponent<RectTransform>().sizeDelta;

            render_cam = GameObject.Find("UI_NodeConnectionCamera").GetComponent<Camera>();

            //set render texture to camera
            view_texture.width = (int)this.GetComponent<RectTransform>().sizeDelta.x;
            view_texture.height = (int)this.GetComponent<RectTransform>().sizeDelta.y;
            render_cam.targetTexture = view_texture;
            //calculate FoV
            render_cam.fieldOfView = Mathf.Rad2Deg * Mathf.Atan(Mathf.Abs(EditorSystem.UI_ConnectionVirtualView.GetComponent<RectTransform>().sizeDelta.y / 2) / Mathf.Abs(render_cam.transform.position.z)) *2;

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mouse_down == true)
        {



            Vector3 offset = Input.mousePosition - last_mouse_L_pos;

            UI_ScrollPanel.transform.localPosition = UI_ScrollPanel.transform.localPosition + offset;

            last_mouse_L_pos = Input.mousePosition;



        }
    }

    public void selectNode(UI_Node node)
    {


        foreach(UI_Node nn in NodeList)
        {
            if (nn != node)
            {
                nn.flag = UI_Node.NodeUIStatus.NONE;
            }
        }


        EditorSystem.UI_Properites.GetComponent<UI_Properites>().setNode(node);
    }
}



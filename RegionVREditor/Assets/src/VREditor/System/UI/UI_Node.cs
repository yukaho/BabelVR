#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Babel.System.Data;


public class UI_Node : MonoBehaviour
{
    
    //Editor Main Core
    RegionVREditorCore EditorSystem;

    //restore color
    Color restore_color;

    //Hovered color
    Color hover_color;

    //Selected Color
    Color selected_color;

    //Rectangle
    Rect rect;

    //UI flag
    public NodeUIStatus flag;

    //Link with Node Data
    SceneNode NodeData;


    //GUI text displayed on the node
    public Text title;

    public enum NodeUIStatus
    {
        NONE,
        HOVER,
        SELECTED

    }


    public void OnPointerEnter()
    {
        if (flag != NodeUIStatus.SELECTED)
            flag = NodeUIStatus.HOVER;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            this.flag = NodeUIStatus.SELECTED;
            EditorSystem.UI_ConnectionView.GetComponent<NodeConnectionGraphView>().selectNode(this);
        }
    }
    public void OnPointerExit()
    {
        if (flag != NodeUIStatus.SELECTED)
            flag = NodeUIStatus.NONE;
    }

    void Start()
    {
        //find editor core
        EditorSystem = GameObject.Find("EditorSystem").GetComponent<RegionVREditorCore>();

        //set reset color
        restore_color = this.GetComponent<Image>().color;

        //set hover color
        hover_color = new Color(restore_color.r * 0.8f, restore_color.g * 0.8f, restore_color.b * 0.8f, 0.8f);


        //set selected color
        selected_color = new Color(1, 1, 1);

        flag = NodeUIStatus.NONE;


        Vector3[] corners = new Vector3[4];
        this.GetComponent<RectTransform>().GetWorldCorners(corners);

        corners[0].x -= EditorSystem.UI_ConnectionVirtualView.transform.GetChild(0).position.x;
        corners[0].y -= EditorSystem.UI_ConnectionVirtualView.transform.GetChild(0).position.y;
        corners[2].x -= EditorSystem.UI_ConnectionVirtualView.transform.GetChild(0).position.x;
        corners[2].y -= EditorSystem.UI_ConnectionVirtualView.transform.GetChild(0).position.y;
        rect = new Rect(corners[0], corners[2] - corners[0]);
    }


    void Update()
    {

        if (EditorSystem == null)
            return;


        //converted view click pos
        Vector3 converted_view_click_pos = RegionVREditorUtilities.ScreenPointToVirtualViewPoint(
             EditorSystem.UI_ConnectionView,
             EditorSystem.UI_ConnectionVirtualView,
             Input.mousePosition);


        //check mouse if entered
        if (rect.Contains(converted_view_click_pos))
        {
            OnPointerEnter();
        }
        else
        {
            OnPointerExit();
        }


        //update its color
        updateColor();
    }

    public void updateColor()
    {
        switch (flag)
        {
            case NodeUIStatus.NONE:
                this.GetComponent<Image>().color = restore_color;
                break;
            case NodeUIStatus.HOVER:
                this.GetComponent<Image>().color = hover_color;
                break;
            case NodeUIStatus.SELECTED:
                this.GetComponent<Image>().color = selected_color;
                break;
        }
    }


    public void setEditorSystem(RegionVREditorCore EditorSystem)
    {
        this.EditorSystem = EditorSystem;

        if (EditorSystem == null)
        {
            Debug.Log("NULL");
        }
        else
        {
            Debug.Log(this.EditorSystem.name);
        }
    }

    public void setNode(SceneNode node)
    {
        this.NodeData = node;
        this.title.text = node.s_name;
    }

    public SceneNode getNode()
    {
        return NodeData;
    }

}

#endif
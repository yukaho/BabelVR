#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Babel.System.Data;
public class UI_Properites: MonoBehaviour
{
    public Text NodeTitle;
    SceneNode node;


    //Link with content transform
    public RectTransform Content;


    //3 blocks
    RectTransform UI_Properties_Initialize_Block;
    RectTransform UI_Properties_Running_Block;
    RectTransform UI_Properties_End_Block;
    void Start()
    {
        //initialize menu
        //h:w of eact properties block : 449 x 449

        //Load Prefab
        RectTransform UI_Properties_Block = Resources.Load<RectTransform>("Prefab/UI/UI_Properties/UI_StateSetting");

        //Anchored Point
        Vector3 start_post = new Vector3(0, -300, 0);
        Vector3 offset = new Vector3(0, -449, 0);

        //Initialize block
        UI_Properties_Initialize_Block =Instantiate(UI_Properties_Block, Content);
        UI_Properties_Initialize_Block.anchoredPosition3D = start_post;
        UI_Properties_Initialize_Block.localScale = new Vector3(1, 1, 1);
        UI_Properties_Initialize_Block.FindChild("TopRegion").FindChild("Title").GetComponent<Text>().text = "Initialize()";
        UI_Properties_Initialize_Block.GetComponent<UI_StateSetting>().setFlag(UI_StateSetting.StateFlag.Initialize);

        //Running  block
        UI_Properties_Running_Block = Instantiate(UI_Properties_Block, Content);
        UI_Properties_Running_Block.anchoredPosition3D = start_post += offset;
        UI_Properties_Running_Block.localScale = new Vector3(1, 1, 1);
        UI_Properties_Running_Block.FindChild("TopRegion").FindChild("Title").GetComponent<Text>().text = "Running()";
        UI_Properties_Running_Block.GetComponent<UI_StateSetting>().setFlag(UI_StateSetting.StateFlag.Running);


        //End Block
        UI_Properties_End_Block = Instantiate(UI_Properties_Block, Content);
        UI_Properties_End_Block.anchoredPosition3D = start_post += offset;
        UI_Properties_End_Block.localScale = new Vector3(1, 1, 1);
        UI_Properties_End_Block.FindChild("TopRegion").FindChild("Title").GetComponent<Text>().text = "End()";
        UI_Properties_End_Block.GetComponent<UI_StateSetting>().setFlag(UI_StateSetting.StateFlag.End);


    }

    public void setNode(SceneNode node)
    {

        //select Node
        if (!transform.GetChild(0).gameObject.active)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }

        this.node = node;
        NodeTitle.text = node.s_name;
        UI_Properties_Initialize_Block.GetComponent<UI_StateSetting>().setNode(node);
        UI_Properties_Running_Block.GetComponent<UI_StateSetting>().setNode(node);
        UI_Properties_End_Block.GetComponent<UI_StateSetting>().setNode(node);
    }


    public void setNode(UI_Node node)
    {
        this.setNode(node.getNode());
    }

    public SceneNode getNode()
    {
        return node;
    }
}
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using System.IO;
using Babel.System.Data;
using Babel;
public class RegionVREditorCore : MonoBehaviour
{




    //UI
    public GameObject UI_Canvas;
    public GameObject UI_ConnectionView;
    public GameObject UI_ConnectionVirtualView;
    public GameObject UI_ActionBar_Panel;
    public GameObject UI_Properites;

    //Node List
    List<SceneNode> NodeList;


    //Undo_Redo
    List<SystemAction> ActionHistoryList;


    // Use this for initialization
    void Start()
    {




        //set up new node list for storing node
        NodeList = new List<SceneNode>();




        //set up new action record list for storing action record
        ActionHistoryList = new List<SystemAction>();


        //Set up UI Canvas
        UI_Canvas.GetComponent<UI_Canvas>().onClick_Action += RegionVREditorCore_onClick_Action;


        Debug.Log("Region VR Editor Initializated.");

       





    }

    private void RegionVREditorCore_onClick_Action(object sender, EventArgs e)
    {
        //cancel all other menu items
        UI_ActionBar_Panel.GetComponent<ActionBar>().OnCancel();
    }

    // Update is called once per frame
    void Update()
    {

        //test undo
        if (Input.GetKeyDown(KeyCode.K))
        {
            undohistory();
        }

        //test redo
        if (Input.GetKeyDown(KeyCode.L))
        {
            redohistory();
        }


    }


    //undo/redo
    public void undohistory()
    {

    }

    public void redohistory()
    {

    }


    //onClick Action
    public void onClickCreateNode(object sender, EventArgs e)
    {




        //create new action snap shot and add to the history
        SystemAction action_createNode = new SystemAction();
        action_createNode.REDO += redo_createNode;
        action_createNode.UNDO += undo_createNode;
        ActionHistoryList.Add(action_createNode);

        //create node
        createNode(sender, e);





    }

    public void OnClickActionBarFile(object sender, EventArgs e)
    {
        GameObject file_action = UI_ActionBar_Panel.transform.FindChild("Option_File").gameObject;
        RectTransform rTrans = file_action.GetComponent<RectTransform>();
        ActionBarItem item = file_action.GetComponent<ActionBarItem>();

        //cancel all other menu items
        UI_ActionBar_Panel.GetComponent<ActionBar>().OnCancel();

        if (item.dropdown_menu == null)
        {
            GameObject DropDownMenu = Resources.Load("Prefab/UI/ControlMenu/Option_File/File_DropDownMenu") as GameObject;
            DropDownMenu = Instantiate(DropDownMenu, new Vector3(0, 0, 0), Quaternion.identity, file_action.transform);
            DropDownMenu.transform.localScale = new Vector3(1, 1, 1);
            DropDownMenu.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, rTrans.anchoredPosition.y - rTrans.sizeDelta.y, 0);
            item.dropdown_menu = DropDownMenu;
        }
        else
        {
            item.dropdown_menu.SetActive(true);
        }




    }


    public void OnClickOpen(object sender, EventArgs e)
    {
        string path = EditorUtility.OpenFilePanel("Please select your saved template", "", "");
        this.openFile(path);
    }

    public void OnClickSave(object sender, EventArgs e)
    {
        string path = EditorUtility.SaveFilePanel("Please select your directory", "", "", "vrs");
        this.saveFile(path);
    }

    public void OnClickActionBarEdit(object sender, EventArgs e)
    {
        GameObject file_action = UI_ActionBar_Panel.transform.FindChild("Option_Edit").gameObject;
        RectTransform rTrans = file_action.GetComponent<RectTransform>();
        ActionBarItem item = file_action.GetComponent<ActionBarItem>();


        //cancel all other menu items
        UI_ActionBar_Panel.GetComponent<ActionBar>().OnCancel();

        if (item.dropdown_menu == null)
        {
            GameObject DropDownMenu = Resources.Load("Prefab/UI/ControlMenu/Option_Edit/Edit_DropDownMenu") as GameObject;
            DropDownMenu = Instantiate(DropDownMenu, new Vector3(0, 0, 0), Quaternion.identity, file_action.transform);
            DropDownMenu.transform.localScale = new Vector3(1, 1, 1);
            DropDownMenu.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, rTrans.anchoredPosition.y - rTrans.sizeDelta.y, 0);
            item.dropdown_menu = DropDownMenu;
        }
        else
        {
            item.dropdown_menu.SetActive(true);
        }





    }


    //Functions 
    public void createNode(object sender, EventArgs e)
    {
        //converted view click pos
        Vector3 converted_view_click_pos = RegionVREditorUtilities.ScreenPointToVirtualViewPoint(
            UI_ConnectionView,
            UI_ConnectionVirtualView,
            UI_ConnectionView.GetComponent<NodeConnectionGraphView>().last_mouse_R_pos);



        //Create new Node, Feb 28 2017
        GameObject gNode = Resources.Load("Prefab/UI/Node") as GameObject;

        //set system core
        gNode.GetComponent<UI_Node>().setEditorSystem(this);


        gNode = Instantiate(gNode, UI_ConnectionVirtualView.transform.GetChild(0));

        SceneNode data_node = new SceneNode();
        data_node.s_name = "Node_" + NodeList.Count.ToString("D2");
        data_node.setGraphPosition(converted_view_click_pos);

        UI_Node ui_node = gNode.GetComponent<UI_Node>();
        ui_node.setNode(data_node);


        //Debug.Log(Input.mousePosition);
        gNode.transform.localScale = new Vector3(1, 1, 1);

        gNode.GetComponent<RectTransform>().anchoredPosition3D = converted_view_click_pos;

        //Add node to List
        NodeList.Add(data_node);
        UI_ConnectionView.GetComponent<NodeConnectionGraphView>().NodeList.Add(ui_node);
    }



    public void redo_createNode(object sender, EventArgs e)
    {

    }

    public void undo_createNode(object sender, EventArgs e)
    {

    }



    public void saveFile(string path)
    {


        //check whether path is null
        if (path == null || path.Length == 0)
            return;



        //split out the file name
        string[] split = path.Split('/');
        string file_name = split[split.Length - 1];

        //Prepare saving Data
        SystemData ss = new SystemData();
        ss.file_name = file_name.Remove(file_name.Length - 4);
        ss.savedTime = DateTime.Now;


        //Add evey scene node
        foreach (SceneNode n in NodeList)
        {
            ss.scene_nodes.Add(n);
        }


      

        //Testing mock up data
        //for (int i = 0; i < 3; i++)
        //{
        //    ss.scene_nodes.Add(new SceneNode());
        //}


        ////testing
        //for (int i = 0; i < 3; i++)
        //{
        //    ss.scene_nodes[0].initailize_actions.Add(new SceneAction());
        //    ss.scene_nodes[0].running_actions.Add(new SceneAction());
        //    ss.scene_nodes[0].end_actions.Add(new SceneAction());
        //}

        //for (int i = 0; i < 3; i++)
        //{
        //    ss.scene_nodes[1].initailize_actions.Add(new SceneAction(SceneAction.Flag.PlayAudio));
        //    ss.scene_nodes[1].running_actions.Add(new SceneAction(SceneAction.Flag.SetCameraOrientation));
        //    ss.scene_nodes[1].end_actions.Add(new SceneAction(SceneAction.Flag.SwitchSceneFirstSeenRegion));
        //}

        //for (int i = 0; i < 3; i++)
        //{
        //    ss.scene_nodes[2].initailize_actions.Add(new SceneAction());
        //    ss.scene_nodes[2].running_actions.Add(new SceneAction());
        //    ss.scene_nodes[2].end_actions.Add(new SceneAction());
        //}

        //
        //Write
        //

        //Serialize Object
        string derialized_str = JsonConvert.SerializeObject(ss, Formatting.Indented);

        //string derialized_str = ss.deserialize();

        //initialize writer
        using (StreamWriter outputFile = new StreamWriter(path))
        {
            //write out the data
            outputFile.Write(derialized_str);

            //close writer
            outputFile.Close();
        }
    }

    public void openFile(string path)
    {
        //check whether path is null
        if (path == null || path.Length == 0)
            return;


        //Read File
        using (StreamReader readfile = new StreamReader(path))
        {
            string serialized_str = readfile.ReadToEnd();


            //deserialize
            SystemData data = JsonConvert.DeserializeObject<SystemData>(serialized_str);

            //display message
            Debug.Log(data);

            //
            //clear data
            //

            //clear ui
            for (int i = 0; i < UI_ConnectionVirtualView.transform.GetChild(0).childCount; i++)
            {
                if (!UI_ConnectionVirtualView.transform.GetChild(0).GetChild(i).gameObject.name.Equals("Title"))
                    GameObject.Destroy(UI_ConnectionVirtualView.transform.GetChild(0).GetChild(i).gameObject);
            }

            //clear node list
            NodeList.Clear();

            foreach (SceneNode node in data.scene_nodes)
            {
                NodeList.Add(node);

                //Create new UI Node, Feb 28 2017
                GameObject gNode = Resources.Load("Prefab/UI/Node") as GameObject;

                //set system core
                gNode.GetComponent<UI_Node>().setEditorSystem(this);
                gNode = Instantiate(gNode, UI_ConnectionVirtualView.transform.GetChild(0));
                gNode.transform.localScale = new Vector3(1, 1, 1);
                gNode.GetComponent<RectTransform>().anchoredPosition3D = node.getGraphPosition();
                UI_Node ui_node = gNode.GetComponent<UI_Node>();
                ui_node.setNode(node);
                UI_ConnectionView.GetComponent<NodeConnectionGraphView>().NodeList.Add(ui_node);
            }

        }



    }

}

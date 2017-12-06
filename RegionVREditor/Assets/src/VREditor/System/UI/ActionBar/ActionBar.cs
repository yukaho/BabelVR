#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBar : MonoBehaviour,IPointerCancelAction
{
    //Editor System
    RegionVREditorCore system;

    //Menu Items List
    string[] menuitem_name;
    List<GameObject> menuItem;

    // Use this for initialization
    void Start()
    {
        //reference core system
        system = GameObject.Find("EditorSystem").GetComponent<RegionVREditorCore>();

        //initialize menu list
        menuItem = new List<GameObject>();

        //menu item
        menuitem_name = new string[4];
        menuitem_name[0] = "File";
        menuitem_name[1] = "Edit";
        menuitem_name[2] = "Window";
        menuitem_name[3] = "About";

        //initialize menu options
        initializeMenuOption();

    }

    void initializeMenuOption()
    {
        int x = 0;
        int offset = 150;

        //Create menu item
        foreach (string s in menuitem_name)
        {
            //Initialize Menu Button
            GameObject Option = Resources.Load("Prefab/UI/ControlMenu/Option") as GameObject;
            Option = Instantiate(Option, new Vector3(0, 0, 0), Quaternion.identity);
            Option.transform.SetParent(this.transform);
            Option.GetComponent<RectTransform>().anchoredPosition = new Vector3(x, 0, 0);
            Option.transform.localScale = new Vector3(1, 1, 1);
            x += offset;
            Option.name = "Option_" + s;
            Option.GetComponentInChildren<Text>().text = s;
            menuItem.Add(Option);
        }

        //add function to "file tab"
        menuItem[0].GetComponent<ActionBarItem>().onClick_Action += system.OnClickActionBarFile;


        //add function to "edit tab"
        menuItem[1].GetComponent<ActionBarItem>().onClick_Action += system.OnClickActionBarEdit;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnCancel()
    {
       foreach(GameObject menu in menuItem)
        {
            menu.GetComponent<ActionBarItem>().OnCancel();
        }
    }
}
#endif
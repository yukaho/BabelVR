using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Babel.System.Data;
public class UI_StateSetting : MonoBehaviour
{

    //Link with parent
    SceneNode node;

    //ActionBlock
    Vector3 start_postion;
    Vector3 offset;

    //Reference
    RectTransform loaded_ActionBlock;


    //region that places action block
    public RectTransform bottom_region;


    //Target List
    List<SceneAction> target;


    public enum StateFlag
    {
        Initialize,
        Running,
        End
    }

    public StateFlag flag;

    // Use this for initialization
    void Start()
    {
        //Load action block instance
        loaded_ActionBlock = Resources.Load<RectTransform>("Prefab/UI/UI_Properties/UI_ActionBlock");

    

 
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void addActionBlock()
    {
        if (node == null)
            return;

        SceneAction sa = new SceneAction();
        sa.action_flag = SceneAction.Flag.None;

        //add action to node list
       target.Add(sa);


        //refrest the list action
        refreshActionBlocks();
    }

    public void removeActionBlock(Button sender)
    {


        GameObject actionBlock = sender.transform.parent.parent.gameObject;
        //Debug.Log(actionBlock.name);
        //Destroy(actionBlock);

        int index = int.Parse(actionBlock.name);

        target.RemoveAt(index);


        //refrest the list action
        refreshActionBlocks();
    }

    public void refreshActionBlocks()
    {


        //Clear all action
        for (int cc = 0; cc < bottom_region.childCount; cc++)
        {
            Destroy(bottom_region.GetChild(cc).gameObject);
        }


        //position offset
        start_postion = new Vector3(0, 0, 0);
        offset = new Vector3(0, 100, 0);

        int i = 0;
        foreach (SceneAction sa in target)
        {
            //add new action block
            RectTransform actionBlock = Instantiate(loaded_ActionBlock, bottom_region);
            actionBlock.anchoredPosition3D = start_postion - offset * i;
            actionBlock.localScale = new Vector3(1, 1, 1);
            actionBlock.GetComponentInChildren<Button>().onClick.AddListener(delegate () { removeActionBlock(actionBlock.GetComponentInChildren<Button>()); });
          
            actionBlock.GetComponent<UI_ActionBlock>().loadAction(this.flag);
            actionBlock.GetComponent<UI_ActionBlock>().setAction(sa);
            actionBlock.name = "" + i;

           
        

            i++;
        }


    }

    public void setNode(SceneNode node)
    {
        //set new node
        this.node = node;

        switch (flag)
        {
            case StateFlag.Initialize:
                target = node.initailize_actions;
                break;
            case StateFlag.Running:
                target = node.running_actions;
                break;
            case StateFlag.End:
                target = node.end_actions;
                break;

        }

        //set new node refrest.
        refreshActionBlocks();
    }

    public void setFlag(StateFlag flag)
    {
        this.flag = flag;


    }
}

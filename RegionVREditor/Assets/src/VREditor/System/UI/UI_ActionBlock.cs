using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Babel.System.Data;
public class UI_ActionBlock : MonoBehaviour
{

    //options of actions
    public Dropdown dropdown_actions;
    public Dropdown dropdown_parameterA;
    public Dropdown dropdown_parameterB;

    //new Data list
    List<Dropdown.OptionData> data;


    //Link with scene action
    SceneAction sa;

    public void loadAction(UI_StateSetting.StateFlag flag)
    {
        dropdown_actions.onValueChanged.AddListener(delegate { onActionOptionValueChanged(); });

        //new Data list
        data = new List<Dropdown.OptionData>();

        data.Add(new ActionBlockOptionData(SceneAction.Flag.None));


        //clear option
        dropdown_actions.ClearOptions();


        switch (flag)
        {
            case UI_StateSetting.StateFlag.Initialize:

                data.Add(new ActionBlockOptionData(SceneAction.Flag.SetCameraOrientation));
                data.Add(new ActionBlockOptionData(SceneAction.Flag.SetVideo));
                data.Add(new ActionBlockOptionData(SceneAction.Flag.PlayAudio));
                dropdown_actions.AddOptions(data);
                break;
            case UI_StateSetting.StateFlag.Running:

                data.Add(new ActionBlockOptionData(SceneAction.Flag.RegionJumpToScene));
                data.Add(new ActionBlockOptionData(SceneAction.Flag.RegionJumpToShot));
                data.Add(new ActionBlockOptionData(SceneAction.Flag.RegionToAction));
                data.Add(new ActionBlockOptionData(SceneAction.Flag.TimeToAction));
                data.Add(new ActionBlockOptionData(SceneAction.Flag.TimeToScene));
                data.Add(new ActionBlockOptionData(SceneAction.Flag.TimeToShot));
                dropdown_actions.AddOptions(data);
                break;
            case UI_StateSetting.StateFlag.End:

                data.Add(new ActionBlockOptionData(SceneAction.Flag.SwitchSceneDefault));
                data.Add(new ActionBlockOptionData(SceneAction.Flag.SwitchSceneFirstSeenRegion));
                data.Add(new ActionBlockOptionData(SceneAction.Flag.SwitchSceneLastSeenRegion));
                data.Add(new ActionBlockOptionData(SceneAction.Flag.SwitchSceneMaxGazingTime));
                data.Add(new ActionBlockOptionData(SceneAction.Flag.SwitchSceneMinGazingTime));
                data.Add(new ActionBlockOptionData(SceneAction.Flag.SwitchSceneSeenAt));
                data.Add(new ActionBlockOptionData(SceneAction.Flag.SwitchSceneRandom));
                dropdown_actions.AddOptions(data);
                break;
        }
    }

    public void setAction(SceneAction sa)
    {

        this.sa = sa;
        dropdown_actions.value = searchOption(sa.action_flag);
       
    }

    int searchOption(SceneAction.Flag flag)
    {


        for (int i = 0; i < data.Count; i++)
        {
            ActionBlockOptionData abod = (ActionBlockOptionData)data.ElementAt<Dropdown.OptionData>(i);
            if (abod.action_flag == flag)
            {
                return i;
            }
        }
        //not found
        return -1;
    }

    public void onActionOptionValueChanged()
    {
        sa.action_flag = ((ActionBlockOptionData)dropdown_actions.options.ElementAt<Dropdown.OptionData>(dropdown_actions.value)).action_flag;
    }


}


public class ActionBlockOptionData : Dropdown.OptionData
{
    public SceneAction.Flag action_flag;
    public ActionBlockOptionData(SceneAction.Flag action_flag) : base(action_flag.ToString())
    {
        this.action_flag = action_flag;
    }
}


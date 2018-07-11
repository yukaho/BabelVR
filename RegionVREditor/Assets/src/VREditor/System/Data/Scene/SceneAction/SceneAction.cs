using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Babel.System.Data
{
    public class SceneAction
    {
        //Json Saved Data
        [JsonPropertyAttribute]
        [JsonConverter(typeof(StringEnumConverter))]
        public Flag action_flag = Flag.None;

        [JsonPropertyAttribute]
        public object[] parameters_list;

        public SceneAction()
        {
            setParameters(0, new SimpleVector3(3, 3, 3));
        }

        public void setParameters(params object[] in_parameters_list)
        {
            parameters_list = new object[in_parameters_list.Length];

            for (int i = 0; i < in_parameters_list.Length; i++)
            {
                parameters_list[i] = in_parameters_list[i];
            }
        }

        public void execute(VRPlayerCore core)
        {

            //for (int i = 0; i < parameters_list.Length; i++)
            //{
            //    Debug.Log(parameters_list[i]);
            //}
            int scene_index;
            RegionOfInterest Ref_ROI;
            switch (this.action_flag)
            {
                case Flag.None:
                    break;
                case Flag.SetCameraOrientation:
                    break;
                case Flag.SetVideo:
                    int i = Convert.ToInt32(parameters_list[0]);
                    // switch shot in same scene
                    core.SwitchShotNode(i);
                    break;
                case Flag.SetScene:
                    scene_index = Convert.ToInt32(parameters_list[0]);
                    // switch scene
                    core.SwitchSceneNode(scene_index);
                    break;
                case Flag.PlayAudio:
                    //
                    break;
                case Flag.SwitchSceneDefault:
                    Debug.Log("Switch Scene - Default");
                    scene_index = Convert.ToInt32(parameters_list[0]);
                    core.SwitchSceneNode(scene_index);
                    break;
                case Flag.SwitchSceneMaxGazingTime:
                    Debug.Log("Switch Scene - Max Gazing");
                    //get maximum gazing time ROI and switch to corresponding node
                    Ref_ROI = core.current_node.currentShotNode.Scene_ROIList[0];
                    core.triggerROI(Ref_ROI);
                    break;
                case Flag.SwitchSceneMinGazingTime:
                    Debug.Log("Switch Scene - Min Gazing");
                    //get mininal gazing time ROI and switch to corresponding node
                    Ref_ROI = core.current_node.currentShotNode.Scene_ROIList[core.current_node.currentShotNode.Scene_ROIList.Count - 1];
                    core.triggerROI(Ref_ROI);
                    break;
                case Flag.SwitchSceneLastSeenRegion:
                    Debug.Log("Switch Scene - Last Seen ROI");
                    //Get last seen ROI and switch to corresponding node
                    Ref_ROI = core.LastSeenROI.GetComponent<RegionOfInterestObject>().roi;
                    core.triggerROI(Ref_ROI);
                    break;
                case Flag.SwitchSceneFirstSeenRegion:
                    Debug.Log("Switch Scene - First Seen ROI");
                    //Get last seen ROI and switch to corresponding node
                    Ref_ROI = core.FirstSeenROI.GetComponent<RegionOfInterestObject>().roi;
                    core.triggerROI(Ref_ROI);
                    break;
                case Flag.SwitchSceneSeenAt:
                    Debug.Log("Switch Scene - Seen At");

                    GazingLog log_0 = new GazingLog();
                    GazingLog log_1 = new GazingLog();
                    int target_frame = Convert.ToInt32(parameters_list[0]);


                    for (int g = 0; g < core.GazingLog_List.Count; g++)
                    {
                        if (core.GazingLog_List[g].timecode <= target_frame)
                        {
                            log_0 = core.GazingLog_List[g];
                            if (g + 1 < core.GazingLog_List.Count)
                            {
                                log_1 = core.GazingLog_List[g + 1];
                            }
                        }
                    }




                    if (log_0.flag == GazingAction.ENTERED)
                    {
                        

                        if (log_0.roi != null)
                        {
                            Ref_ROI = log_0.roi;
                            core.triggerROI(Ref_ROI);
                        }
                     

                    }



                    break;
                case Flag.SwitchSceneRandom:
                    int ran_index = UnityEngine.Random.Range(0, core.SceneNodeList.Count);
                    SceneNode ref_node = core.SceneNodeList[ran_index];

                    //get another node if same
                    while (core.current_node == ref_node)
                    {
                        ran_index = UnityEngine.Random.Range(0, core.SceneNodeList.Count);
                        ref_node = core.SceneNodeList[ran_index];
                    }
                    //pick scene node randomly 
                    Debug.Log("Switch Scene - Random");
                    core.SwitchSceneNode(ran_index);
                    break;

                case Flag.TimeToScene:

                    break;
                case Flag.TimeToAction:

                    break;

            }
        }

        public SceneAction(SceneAction.Flag f, params object[] obj)
        {
            this.action_flag = f;

            parameters_list = new object[obj.Length];

            for (int i = 0; i < obj.Length; i++)
            {
                parameters_list[i] = obj[i];
            }
        }

        public SceneAction(SceneAction.Flag f)
        {
            this.action_flag = f;


            switch (this.action_flag)
            {
                case Flag.None:
                    break;
                case Flag.SetCameraOrientation:
                    parameters_list = new object[3];
                    break;
                case Flag.SetVideo:
                    parameters_list = new object[1];
                    break;
                case Flag.PlayAudio:
                    parameters_list = new object[1];
                    break;
                case Flag.SwitchSceneDefault:
                    parameters_list = new object[1];
                    break;
                case Flag.SwitchSceneMaxGazingTime:
                    parameters_list = new object[1];
                    break;
                case Flag.SwitchSceneMinGazingTime:
                    parameters_list = new object[1];
                    break;
                case Flag.SwitchSceneLastSeenRegion:
                    parameters_list = new object[1];
                    break;
                case Flag.SwitchSceneFirstSeenRegion:
                    parameters_list = new object[1];
                    break;
                case Flag.SwitchSceneSeenAt:
                    parameters_list = new object[1];
                    break;
                case Flag.SwitchSceneRandom:
                    parameters_list = new object[1];
                    break;
                case Flag.RegionJumpToScene:
                    parameters_list = new object[1];
                    break;
                case Flag.RegionToAction:
                    parameters_list = new object[1];
                    break;
                case Flag.TimeToScene:
                    parameters_list = new object[1];
                    break;
                case Flag.TimeToAction:
                    parameters_list = new object[1];
                    break;
            }
        }

        public override string ToString()
        {
            return "Action Flag:" + action_flag;
        }
        public enum Flag : int
        {
            None = 0,
            SetCameraOrientation = 1,
            SetVideo = 2,
            PlayAudio = 3,
            SwitchSceneDefault = 4,
            SwitchSceneMaxGazingTime = 5,
            SwitchSceneMinGazingTime = 6,
            SwitchSceneLastSeenRegion = 7,
            SwitchSceneFirstSeenRegion = 8,
            SwitchSceneSeenAt = 9,
            SwitchSceneRandom = 10,
            RegionJumpToScene = 11,
            RegionJumpToShot = 12,
            RegionToAction = 13,
            TimeToScene = 14,
            TimeToAction = 15,
            TimeToShot = 16,
            SetScene = 17

        }


    }

}
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
            setParameters(0, new SimpleVector3(3,3,3));
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
                    core.setVideo(i);
                    break;
                case Flag.SetScene:
                    scene_index = Convert.ToInt32(parameters_list[0]);
                    core.switchSceneNode(scene_index);
                    break;
                case Flag.PlayAudio:

                    break;
                case Flag.SwitchSceneDefault:
                    scene_index = Convert.ToInt32(parameters_list[0]);
                    core.switchSceneNode(scene_index);
                    break;
                case Flag.SwitchSceneMaxGazingTime:

                    //get maximum gazing time ROI and switch to corresponding node
                    Ref_ROI = core.current_node.currentShotNode.Passive_ROIList[0];
                    core.triggerROI(Ref_ROI);
                    break;
                case Flag.SwitchSceneMinGazingTime:

                    //get mininal gazing time ROI and switch to corresponding node
                    Ref_ROI = core.current_node.currentShotNode.Passive_ROIList[core.current_node.currentShotNode.Passive_ROIList.Count - 1];
                    core.triggerROI(Ref_ROI);
                    break;
                case Flag.SwitchSceneLastSeenRegion:

                    //Get last seen ROI and switch to corresponding node
                    Ref_ROI = core.LastSeenROI.GetComponent<RegionOfInterestMesh>().roi;
                    core.triggerROI(Ref_ROI);
                    break;
                case Flag.SwitchSceneFirstSeenRegion:

                    break;
                case Flag.SwitchSceneSeenAt:

                    break;
                case Flag.SwitchSceneRandom:

                    break;

                case Flag.TimeToScene:

                    break;
                case Flag.TimeToAction:

                    break;

            }
        }

        public SceneAction(SceneAction.Flag f,params object[] obj)
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
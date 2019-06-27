using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using RenderHeads.Media.AVProVideo;

namespace Babel.System.Data
{
    public class SceneNode
    {

        public static int factory_id = 0;

        //properties
        [JsonPropertyAttribute]
        public int id;   //scene object id

        [JsonPropertyAttribute]
        public string s_name;   //scene name

        [JsonPropertyAttribute]
        public List<SceneAction> initailize_actions; //actions of initialization stage

        [JsonPropertyAttribute]
        public List<SceneAction> running_actions; //actions of running stage

        [JsonPropertyAttribute]
        public List<SceneAction> end_actions; //actions of end stage

        [JsonPropertyAttribute]
        public float graph_pos_x; //graph position x

        [JsonPropertyAttribute]
        public float graph_pos_y; //graph position y

        [JsonPropertyAttribute]
        public List<ShotNode> shot_list; //video's paths list



        //
        //Run time properites, JSON igonre objects
        //
        //stage
        [JsonPropertyIgnore]
        public enum NodeStatus { initialization, running, end, idle };

        [JsonPropertyIgnore]
        public NodeStatus update_status;

        //AVPro Media Player list
        [JsonPropertyIgnore]
        List<MediaPlayer> loaded_mov_list;

        //current shotNode
        [JsonPropertyIgnore]
        public ShotNode currentShotNode;

        //current playing frame of video
        [JsonPropertyIgnore]
        [SerializeField]
        public int current_frame;
        public float current_timeMs;

        //totatl frame of video
        [JsonPropertyIgnore]
        [SerializeField]
        public int total_frames;


        /// <summary>
        /// Loading Attributes
        /// </summary>
        /// 
        [JsonPropertyIgnore]
        public int totalFiles_count;

        [JsonPropertyIgnore]
        public int currentLoadedFiles_count;

        [JsonPropertyIgnore]
        public bool isLoaded;

        [JsonPropertyIgnore]
        public GameObject scene_obj;


        public SceneNode()
        {

            //register
            id = factory_id;
            factory_id++;

            //name
            s_name = "Scene Node ID: " + id;

            //initialize
            initailize_actions = new List<SceneAction>();
            running_actions = new List<SceneAction>();
            end_actions = new List<SceneAction>();
            shot_list = new List<ShotNode>();

            //set current update status
            update_status = NodeStatus.initialization;

            //default set current shot to first element
            if (shot_list.Count != 0)
            {
                currentShotNode = shot_list[0];
            }

            ///
            /// Loading
            ///
            //set Loaded to false
            isLoaded = false;
            current_timeMs = 0;

        }

        public int getTotalFilesCount()
        {
            totalFiles_count = 0;
            foreach (ShotNode ShotNode in shot_list)
            {
                totalFiles_count += ShotNode.getFilesCount();
            }

            return totalFiles_count;
        }

        public void OnLoadEvent()
        {
            currentLoadedFiles_count++;
        }


        public override string ToString()
        {
            string str = s_name + "\n";


            str += "Initialization Stage:\n";
            foreach (SceneAction sa in initailize_actions)
            {
                str += ">" + sa + "\n";
            }

            str += "Running Stage:\n";
            foreach (SceneAction sa in running_actions)
            {
                str += ">" + sa + "\n";
            }

            str += "End Stage:\n";
            foreach (SceneAction sa in end_actions)
            {
                str += ">" + sa + "\n";
            }

            return str;

        }

        public void reset()
        {
            initailize_actions.Clear();
            running_actions.Clear();
            end_actions.Clear();
        }

        public bool addSceneAction()
        {
            return false;
        }

        public void setGraphPosition(Vector3 post)
        {
            graph_pos_x = post.x;
            graph_pos_y = post.y;
        }

        public Vector3 getGraphPosition()
        {
            return new Vector3(graph_pos_x, graph_pos_y, 0);
        }


        public void update(VRPlayerCore core)
        {
            //if node wasn't loaded with its content, return
            if (!isLoaded)
            {
                Debug.Log("File:" + currentLoadedFiles_count + "/" + getTotalFilesCount());
                return;
            }

            switch (update_status)
            {
                case NodeStatus.initialization:

                    //Load
                    //Load(core);

                    //load all the movie
                    //loadVRMovie(core);

                    Debug.Log("Executing Actions in Running Stage...");

                    //execute initial setting
                    executeInitializationActions(core);

                    //switch status
                    update_status = NodeStatus.running;

                    break;
                case NodeStatus.running:

                    //execute running actions(include conditional checking)
                    executeRunningActions(core);

                    //update timecode info
                    current_timeMs = core.GetCurrentApplyToMesh().Player.Control.GetCurrentTimeMs();
                    float current_time = core.GetCurrentApplyToMesh().Player.Control.GetCurrentTimeMs() / 1000;
                    float total_duration = core.GetCurrentApplyToMesh().Player.Info.GetDurationMs() / 1000;
                    float fps = core.GetCurrentApplyToMesh().Player.Info.GetVideoFrameRate();
                    current_frame = (int)(fps * current_time);
                    total_frames = (int)(fps * total_duration);

                    break;
                case NodeStatus.end:

                    //execute end actions
                    executeEndActions(core);

                    //switch status to idle
                    update_status = NodeStatus.idle;

                    break;
            }

            //** wait for implement
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                this.update_status = NodeStatus.end;
            }
        }



        public void clearROI()
        {

            RegionOfInterest.active_roi_count = 0;
            RegionOfInterest.passive_roi_count = 0;

            Transform group = GameObject.Find("RegionOfInterestGroup").transform;

            //clear all roi in game
            Transform active_ROIGroup = group.FindChild("ActiveROIGroup");
            for (int i = 0; i < active_ROIGroup.childCount; i++)
            {
                GameObject.Destroy(active_ROIGroup.GetChild(i).gameObject);
            }

            //clear all roi in game
            Transform passive_ROIGroup = group.FindChild("PassiveROIGroup");
            for (int i = 0; i < passive_ROIGroup.childCount; i++)
            {
                GameObject.Destroy(passive_ROIGroup.GetChild(i).gameObject);
            }
        }


        public void clearAudioObject()
        {
            Transform group = GameObject.Find("AudioObjectGroup").transform;

            for (int i = 0; i < group.childCount; i++)
            {
                GameObject.Destroy(group.GetChild(i).gameObject);
            }
        }

        public void Load(VRPlayerCore core)
        {
            //Create Scene Data Object
            scene_obj = new GameObject();
            scene_obj.transform.parent = core.gameObject.transform;
            scene_obj.name = "Scene<" + this.s_name + ">";
          //  scene_obj.SetActive(false);


            //Create Shot Data Object from Prefab, Load Data
            foreach (ShotNode shotnode in shot_list)
            {
                GameObject shot_data = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/ShotData"), scene_obj.transform);
                shot_data.name = "Shot<" + shotnode.shot_id + ">";
                shot_data.transform.parent = scene_obj.transform;
                shotnode.OnShotNodeContentLoaded += Shotnode_OnShotNodeContentLoaded;
                shotnode.shotdata_obj = shot_data.transform;
                shotnode.Load(core, shot_data.transform);
            }

            //Content Loaded
            isLoaded = true;
        }


        private void Shotnode_OnShotNodeContentLoaded(object sender, EventArgs e)
        {
            currentLoadedFiles_count++;
        }

  


        public void unloadContent()
        {


            foreach (MediaPlayer mp in loaded_mov_list)
            {
                mp.CloseVideo();
                GameObject.Destroy(mp.gameObject);
            }

        }

        public void executeInitializationActions(VRPlayerCore core)
        {

            foreach (SceneAction sa in initailize_actions)
            {
                sa.execute(core);
            }

            Debug.Log("Executing Actions in Initialization Stage...Done");
        }

        public void executeRunningActions(VRPlayerCore core)
        {
            foreach (SceneAction sa in running_actions)
            {
                sa.execute(core);
            }

        }

        public void executeEndActions(VRPlayerCore core)
        {

            //foreach (SceneAction sa in end_actions)
            //{
                //sa.execute(core);
            //}

            end_actions[core.endActionIndex].execute(core);

            Debug.Log("Executing Actions in End Stage...Done");
        }

        public void SetActive(bool b)
        {
            scene_obj.SetActive(b);
        }
    }
}

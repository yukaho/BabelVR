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

        //totatl frame of video
        [JsonPropertyIgnore]
        [SerializeField]
        public int total_frames;


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
            ///TEST Case , Please comment below during player run-time.
            ///

            //ShotNode[] ns = new ShotNode[2];
            //ns[0] = new ShotNode();
            //ns[1] = new ShotNode();

            //ns[0].movie_dir = "dir_path";

            //RegionOfInterest[] roi = new RegionOfInterest[4];

            //SimpleVector3 transform = new SimpleVector3(0, 0, 5);
            //int length = 1;

            ////create default mesh
            //roi[1] = new RegionOfInterest();
            //roi[1].flag = RegionOfInterestFlag.Active;
            //roi[1].vertics = new SimpleVector3[4];
            //roi[1].vertics[0] = new SimpleVector3(-length, length, 0);
            //roi[1].vertics[1] = new SimpleVector3(length, length, 0);
            //roi[1].vertics[2] = new SimpleVector3(-length, -length, 0);
            //roi[1].vertics[3] = new SimpleVector3(length, -length, 0);
            //roi[1].video_index = 1;
            //ns[0].Active_ROIList.Add(roi[1]);

            //roi[2] = new RegionOfInterest();
            //roi[2].flag = RegionOfInterestFlag.Active;
            //roi[2].vertics = new SimpleVector3[4];
            //roi[2].vertics[0] = new SimpleVector3(-length, length, 0);
            //roi[2].vertics[1] = new SimpleVector3(length, length, 0);
            //roi[2].vertics[2] = new SimpleVector3(-length, -length, 0);
            //roi[2].vertics[3] = new SimpleVector3(length, -length, 0);
            //roi[1].video_index = 2;
            //ns[0].Passive_ROIList.Add(roi[1]);


            //ns[1].movie_dir = "dir_path";

            ////create default mesh
            //roi[1] = new RegionOfInterest();
            //roi[1].flag = RegionOfInterestFlag.Active;
            //roi[1].vertics = new SimpleVector3[4];
            //roi[1].vertics[0] = new SimpleVector3(-length, length, 0);
            //roi[1].vertics[1] = new SimpleVector3(length, length, 0);
            //roi[1].vertics[2] = new SimpleVector3(-length, -length, 0);
            //roi[1].vertics[3] = new SimpleVector3(length, -length, 0);
            //roi[1].video_index = 1;
            //ns[1].Active_ROIList.Add(roi[1]);

            //roi[2] = new RegionOfInterest();
            //roi[2].flag = RegionOfInterestFlag.Passive;
            //roi[2].vertics = new SimpleVector3[4];
            //roi[2].vertics[0] = new SimpleVector3(-length, length, 0);
            //roi[2].vertics[1] = new SimpleVector3(length, length, 0);
            //roi[2].vertics[2] = new SimpleVector3(-length, -length, 0);
            //roi[2].vertics[3] = new SimpleVector3(length, -length, 0);
            //roi[1].video_index = 2;
            //ns[1].Passive_ROIList.Add(roi[1]);


            ////add to shot list
            //shot_list.Add(ns[0]);
            //shot_list.Add(ns[1]);

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
            switch (update_status)
            {
                case NodeStatus.initialization:

                    //load all the movie
                    loadVRMovie(core);

                    //execute initial setting
                    executeInitializationActions(core);

                    //switch status
                    update_status = NodeStatus.running;
                    Debug.Log("Executing Actions in Running Stage...");


                    break;
                case NodeStatus.running:

                    //execute running actions(include conditional checking)
                    executeRunningActions(core);

                    //update timecode info
                    float current_time = core.mesh_content.Player.Control.GetCurrentTimeMs()/1000;
                    float total_duration = core.mesh_content.Player.Info.GetDurationMs() / 1000;
                    float fps = core.mesh_content.Player.Info.GetVideoFrameRate();
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

        public void loadROI()
        {
            //clear All ROI in game if exists
            clearROI();

            //load ROI
            currentShotNode.loadROI();
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

        public void loadVRMovie(VRPlayerCore core)
        {
            //new Movie lists
            loaded_mov_list = new List<MediaPlayer>();

            GameObject videoList = GameObject.Find("VRVideoList");

            //load all movie to AV Pro object
            foreach (ShotNode node in shot_list)
            {

                //Instantiate From Prefab, AVPro Media Player Prefab
                GameObject obj = Resources.Load<GameObject>("Prefab/AVPro Video Media Player");
                obj = GameObject.Instantiate(obj, videoList.transform);

                MediaPlayer mp = obj.GetComponent<MediaPlayer>();
                mp.m_VideoLocation = new MediaPlayer.FileLocation();
                mp.m_VideoPath = node.movie_dir;
                mp.m_StereoPacking = StereoPacking.TopBottom;
                mp.m_Loop = false;


                loaded_mov_list.Add(mp);
                mp.Events.AddListener(core.OnVideoEvent);
                mp.Events.AddListener(node.OnVideoEvent);

                bool found = mp.OpenVideoFromFile(mp.m_VideoLocation, mp.m_VideoPath, false);

                if (!found)
                {
                    Debug.Log("File not found!");
                    obj.name = "Video<Not Found>";
                }
                else
                {
                    string[] split = mp.m_VideoPath.Split('/');
                    obj.name = "Video<" + split[split.Length - 1] + ">";
                }


            }

            Debug.Log("Loading Movies...Done");
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
            foreach (SceneAction sa in end_actions)
            {
                sa.execute(core);
            }

            Debug.Log("Executing Actions in End Stage...Done");
        }
    }
}

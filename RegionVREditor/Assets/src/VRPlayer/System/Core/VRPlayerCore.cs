﻿//
//Last Update: 10 Sept 2017
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Babel.System.Data;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using RenderHeads.Media.AVProVideo;
using System;
using System.Threading;

public class VRPlayerCore : MonoBehaviour
{


    //Node List
    List<SceneNode> NodeList;
    public SceneNode current_node;

    [Header("ROI Settings")]
    //Region Of Interest Group (gameobj)
    public GameObject ROI_group;



    [TextArea]
    public string ROIGroupInformation = "All roi will be sorted in descending order by score in roi group";

    //Camera Base Transform
    public Transform headset_base;

    //CameraTransform
    public Transform headset_cam;

    //locator for collision experiment
    public GameObject target_anchor;

    [Header("Current Scene Node")]
    //Display Information Debug Only
    public string SceneNode;
    public string SceneNodeStatus;

    [Header("Information Collector")]
    //Collecting Info
    public GameObject LastSeenROI;

    //
    //AVPro Video Player
    //
    [Header("AVPro Player")]
    //set Video
    public Transform VideoList;
    public ApplyToMesh mesh_content;



    void Start()
    {

        //create new scene node
        NodeList = new List<SceneNode>();

        Debug.Log("VR Player Ready.");
   
        //open file
        string path = EditorUtility.OpenFilePanel("Please select your saved template", Application.dataPath+ "/../../Data/saved_data/", "");
        openFile(path);



    }

    // Update is called once per frame
    void Update()
    {

        //update current scene node
        if (current_node != null)
        {
            current_node.update(this);
            SceneNodeStatus = current_node.update_status.ToString();
            SceneNode = current_node.s_name;
        }


    }

    void FixedUpdate()
    {
        //get forward vector of headset
        Vector3 fwd = headset_cam.TransformDirection(Vector3.forward);
        RaycastHit hitInfo;
        Ray ray = new Ray(headset_cam.transform.position, fwd);

        if (Physics.Raycast(ray, out hitInfo))
        {
            //print("There is something in front of the object!");


            //draw debug collision ray
            Debug.DrawLine(ray.origin, hitInfo.point, Color.yellow);
            target_anchor.SetActive(true);
            target_anchor.transform.position = hitInfo.point;
            // Debug.Log(hitInfo.collider.GetComponent<RegionOfInterestMesh>().ToString());



            switch (hitInfo.collider.GetComponent<RegionOfInterestMesh>().roi.flag)
            {
                case RegionOfInterestFlag.Active:
                    //  hitInfo.collider.GetComponent<RegionOfInterestMesh>().roi.triggerAction();
                    triggerROI(hitInfo.collider.GetComponent<RegionOfInterestMesh>().roi);
                    break;
                case RegionOfInterestFlag.Passive:
                    LastSeenROI = hitInfo.collider.gameObject;
                    hitInfo.collider.GetComponent<RegionOfInterestMesh>().roi.addScore(1);
                    break;
            }



            //sort roi list
            SortROI();



        }
        else
        {
            LastSeenROI = null;
            target_anchor.SetActive(false);
        }

    }


    public void SortROI()
    {
        //Sort only passive ROIs in current shot node
        current_node.currentShotNode.Passive_ROIList.Sort((x, y) => -1 * x.score.CompareTo(y.score));
        foreach (RegionOfInterest roi in current_node.currentShotNode.Passive_ROIList)
        {
            Debug.Log("##"+roi.mesh_object);
            roi.mesh_object.transform.parent = null;
            roi.mesh_object.transform.parent = ROI_group.transform.FindChild("PassiveROIGroup");


        }

    }


    //open file
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
            NodeList.Clear();


            foreach (SceneNode sn in data.scene_nodes)
            {
                NodeList.Add(sn);
            }


            //foreach (RegionOfInterest roi in data.roi_list)
            //{
            //    switch (roi.flag)
            //    {
            //        case RegionOfInterestFlag.Active:
            //            Active_ROIList.Add(roi);
            //            break;
            //        case RegionOfInterestFlag.Passive:
            //            Passive_ROIList.Add(roi);
            //            break;
            //    }

            //    roi.createMesh();

            //}


            //** wait for implement, load first scene node as default node
            switchSceneNode(0);

        }
    }

    //Video Controller
    public void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
    {
        switch (et)
        {
            case MediaPlayerEvent.EventType.ReadyToPlay:
                //auto play
                mp.Control.Play();
                break;
            case MediaPlayerEvent.EventType.FirstFrameReady:
                Debug.Log("VR Video:"+mp.name+" First frame ready");
                break;
            case MediaPlayerEvent.EventType.FinishedPlaying:
                current_node.update_status = Babel.System.Data.SceneNode.NodeStatus.end;
                break;
        }
    
    }

    //set video
    public void setVideo(int videolist_index)
    {
        Debug.Log("Set #VIDEO_" + videolist_index);

        //set player 
        mesh_content.Player = VideoList.GetChild(videolist_index).GetComponent<MediaPlayer>();
    

        //set current shot node in current scene node
        current_node.currentShotNode = current_node.shot_list[videolist_index];

        //reload ROI from shot node
        current_node.loadROI();

        //set camera angle
        Debug.Log("Set Initial Angle:"+ current_node.currentShotNode.camera_orientation.getUnityVector3());
        headset_base.rotation = current_node.currentShotNode.camera_orientation.getUnityRotationQuaternion();




    }

    public void switchSceneNode(int index)
    {
        Debug.Log("Switch To Scene Node" + index);
        mesh_content.Player = null;

        //unload all contents include videos,ROIs, if current node exists
        if (current_node != null)
        {
            current_node.unloadContent();
        }

        current_node = NodeList[index];


    }


    //trigger ROI
    public void triggerROI(RegionOfInterest roi)
    {

        //reference link
        int content_type;
        int content_index;

        //get content info
        roi.getContentInfo(out content_type, out content_index);

        Debug.Log("Trigger ROI:" + roi + "ct:" + content_type + "ci:" + content_index);

        switch (content_type)
        {
            case 0:  //scene
                switchSceneNode(content_index);
                break;
            case 1:  //video
                setVideo(content_index);
                break;
            case 2: //audio
                //wait for implementation
                break;
        }
    }

}

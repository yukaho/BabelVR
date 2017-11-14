//
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
    public List<SceneNode> SceneNodeList;
    public SceneNode current_node;

    [Header("ROI")]
    //Region Of Interest Group (gameobj)
    public GameObject ROI_group;


    //Camera Base Transform
    public Transform headset_base;

    //CameraTransform
    public Transform headset_cam;

    //locator for collision experiment
    public GameObject target_anchor;


    //Display Information Debug Only
    [HideInInspector]
    public string SceneNode;

    [HideInInspector]
    public string SceneNodeStatus;

    [Header("Information Collector")]
    //Collecting Info
    public GameObject LastSeenROI;
    public GameObject FirstSeenROI;
    public GameObject CurrentGazingROI;
    public List<GazingLog> GazingLog_List;

    //
    //AVPro Video Player
    //
    [Header("AVPro Player")]
    //set Video
    public Transform VideoList;
    public ApplyToMesh mesh_content;



    /// <summary>
    /// Inspector Data 
    /// </summary>


    [SerializeField]
    private string currentNode_info = "Empty";

    [SerializeField]
    private string currentNode_info_lastSeen_roi = "Empty";

    [SerializeField]
    private string currentNode_info_firstSeen_roi = "Empty";

    [SerializeField]
    private string currentNode_info_roi_score_list = "Empty";

    [SerializeField]
    private int currentNode_currentframes = 0;

    [SerializeField]
    private string currentNode_endAction = "Empty";

    [SerializeField]
    private string currentNode_info_roi_log = "Empty";

    void Start()
    {



        //create new scene node
        SceneNodeList = new List<SceneNode>();

        //creqte new log next
        GazingLog_List = new List<GazingLog>();

        Debug.Log("VR Player Ready.");

        //open file
        string path = EditorUtility.OpenFilePanel("Please select your saved template", Application.dataPath + "/../../Data/saved_data/", "");
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

            //#########
            //update node info on inspector
            //#########


            //update node status
            currentNode_info = SceneNode + " - " + SceneNodeStatus;

            //update relationship between show and scene node
            for (int s = 0; s < current_node.shot_list.Count; s++)
            {
                if (current_node.currentShotNode == current_node.shot_list[s])
                    currentNode_info += "\n-> Shot#" + s + " - " + current_node.shot_list[s].getShotVideoFileName() + "*";
                else
                    currentNode_info += "\n-> Shot#" + s + " - " + current_node.shot_list[s].getShotVideoFileName();
            }

            //update frames
            currentNode_currentframes = current_node.current_frame;

            //update 
            if (current_node.end_actions.Count > 0)
            {
                currentNode_endAction = current_node.end_actions[0].action_flag.ToString();
            }
            else
            {
                currentNode_endAction = "Non-Defined";
            }

        }





    }

    void FixedUpdate()
    {
        //
        //Detect headset gazing to ROIs
        //

        //get forward vector of headset
        Vector3 fwd = headset_cam.TransformDirection(Vector3.forward);
        RaycastHit hitInfo;
        Ray ray = new Ray(headset_cam.transform.position, fwd);

        if (Physics.Raycast(ray, out hitInfo))
        {
            //draw debug collision ray
            Debug.DrawLine(ray.origin, hitInfo.point, Color.red);
            target_anchor.SetActive(true);
            target_anchor.transform.position = hitInfo.point;
            Debug.Log("HIT: " + hitInfo.collider.GetComponent<RegionOfInterestObject>().ToString());


            //which type of roi was hit
            switch (hitInfo.collider.GetComponent<RegionOfInterestObject>().roi.flag)
            {
                case RegionOfInterestFlag.Active:
                    //  hitInfo.collider.GetComponent<RegionOfInterestObject>().roi.triggerAction();
                    triggerROI(hitInfo.collider.GetComponent<RegionOfInterestObject>().roi);
                    break;
                case RegionOfInterestFlag.Passive:
                    if (CurrentGazingROI != null && !CurrentGazingROI.GetComponent<RegionOfInterestObject>().Equals( hitInfo.collider.gameObject.GetComponent<RegionOfInterestObject>()))
                    {
                        CurrentGazingROI.GetComponent<RegionOfInterestObject>().OnExit();
                    }

                    LastSeenROI = hitInfo.collider.gameObject;
                    CurrentGazingROI= hitInfo.collider.gameObject; 
                    currentNode_info_lastSeen_roi = LastSeenROI.name;
                    hitInfo.collider.GetComponent<RegionOfInterestObject>().roi.addScore(1);
                    hitInfo.collider.GetComponent<RegionOfInterestObject>().OnEntered();


                    if (FirstSeenROI == null)
                    {
                        FirstSeenROI = LastSeenROI;
                        currentNode_info_firstSeen_roi = FirstSeenROI.name;
                    }
                    break;
            }

            //sort roi list
            SortROI();

        }
        else
        {
            if (CurrentGazingROI != null)
            {
                //gazing nothing
                CurrentGazingROI.GetComponent<RegionOfInterestObject>().OnExit();   
            }
            CurrentGazingROI = null;
            target_anchor.SetActive(false);
        }



        //refresh rois list in inspector view
        getPassiveROIScoringList();

    }


    public void SortROI()
    {
        if (current_node == null || current_node.currentShotNode == null)
            return;

        //Sort only passive ROIs in current shot node
        current_node.currentShotNode.Passive_ROIList.Sort((x, y) => -1 * x.score.CompareTo(y.score));
        foreach (RegionOfInterest roi in current_node.currentShotNode.Passive_ROIList)
        {
            Debug.Log("##" + roi.mesh_object);
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
            SceneNodeList.Clear();


            foreach (SceneNode sn in data.scene_nodes)
            {
                SceneNodeList.Add(sn);
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
                Debug.Log("VR Video:" + mp.name + " First frame ready");
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
        current_node.loadShotNode();

        //set camera angle
        Debug.Log("Set Initial Angle:" + current_node.currentShotNode.camera_orientation.getUnityVector3());
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

        current_node = SceneNodeList[index];


    }


    //trigger ROI
    public void triggerROI(RegionOfInterest roi)
    {
        Debug.Log("TRRRRRRRRWAEWQAEQE");
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
    public void setROIVisibility(bool visible)
    {

        //set all ROIs' visibility that exist within the list

        for (int g = 0; g < 2; g++)
        {
            for (int i = 0; i < ROI_group.transform.GetChild(g).childCount; i++)
            {
                ROI_group.transform.GetChild(g).GetChild(i).GetComponent<Renderer>().enabled = visible;

            }
        }

    }
    public string getPassiveROIScoringList()
    {
        currentNode_info_roi_score_list = "----Passive ROIs Scoring List----\n";
        if (current_node != null && current_node.currentShotNode != null)
        {
            for (int i = 0; i < current_node.currentShotNode.Passive_ROIList.Count; i++)
            {
                RegionOfInterest roi = current_node.currentShotNode.Passive_ROIList[i];
                currentNode_info_roi_score_list += roi.mesh_object.gameObject.name + " - " +
                   roi.score + "\n";
            }
        }
        return currentNode_info_roi_score_list;
    }


    public void addGazingLog(RegionOfInterestObject roi_mesh, GazingAction flag)
    {
        //add new log to the list
        GazingLog_List.Add(new GazingLog(roi_mesh.roi, currentNode_currentframes, flag));


        currentNode_info_roi_log = "";
        int i = 0;
        foreach (GazingLog log in GazingLog_List)
        {
            currentNode_info_roi_log += i++.ToString("D3")+". "+log + "\n";
            
        }
    }


}

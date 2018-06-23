//
//Last Update: 10 Sept 2017
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Babel.System.Data;
using System.IO;
using Newtonsoft.Json;
#if UNITY_EDITOR
using UnityEditor;
#endif
using RenderHeads.Media.AVProVideo;
using System;
using System.Threading;
using uFileBrowser;

public class VRPlayerCore : MonoBehaviour
{
    //File Browser
    public FileBrowser browser;

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
    public MeshRenderer videoSphere;



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

    //Loading Thread
    Thread loading_thread;

    // Newly created 10/6/2018
    private JsonReader reader;
    [HideInInspector]
    public int currSceneNumber, currShotNumber;
    [HideInInspector]
    public bool isFileOpened = false;
    [HideInInspector]
    public string relativePath = "";

    void Start()
    {
        //create new scene node
        SceneNodeList = new List<SceneNode>();

        //creqte new log next
        GazingLog_List = new List<GazingLog>();

        Debug.Log("VR Player Ready.");

        MediaPlayer m = new MediaPlayer();

        // Newly created 10/6/2018
        reader = GetComponent<JsonReader>();
    }

    public void startVR(String folderPath)
    {
        //open file
        Debug.Log("Try Opening..." + folderPath);

        // Check .vrs file exist or not
        string vrsPath = CheckVRSFileExist(folderPath);
        if (vrsPath == null)
        {
            Debug.LogError(".vrs file not found. Please make sure all contents are inside one single folder.");
            return;
        }

        relativePath = folderPath;

        openFile(vrsPath);

        // Newly created 21/6/2018
        SwitchSceneNode(0);
        //Load Objects in Game 
        //Load();
        //** wait for implement, load first scene node as default node
        //switchSceneNode(0);

        // For inspector
        isFileOpened = true;
    }

    void Load()
    {
        foreach (SceneNode node in SceneNodeList)
        {
            node.Load(this);
        }
    }



    // Update is called once per frame
    void Update()
    {
        // Keep UI File Browser opening
        if (!isFileOpened)
            browser.gameObject.SetActive(true);

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
            //      Debug.Log("HIT: " + hitInfo.collider.GetComponent<RegionOfInterestObject>().ToString());


            //which type of roi was hit
            switch (hitInfo.collider.GetComponent<RegionOfInterestObject>().roi.flag)
            {
                case RegionOfInterestFlag.Shot:
                    //  hitInfo.collider.GetComponent<RegionOfInterestObject>().roi.triggerAction();
                    triggerROI(hitInfo.collider.GetComponent<RegionOfInterestObject>().roi);
                    break;
                case RegionOfInterestFlag.Scene:
                    if (CurrentGazingROI != null && !CurrentGazingROI.GetComponent<RegionOfInterestObject>().Equals(hitInfo.collider.gameObject.GetComponent<RegionOfInterestObject>()))
                    {
                        CurrentGazingROI.GetComponent<RegionOfInterestObject>().OnExit();
                    }

                    LastSeenROI = hitInfo.collider.gameObject;
                    CurrentGazingROI = hitInfo.collider.gameObject;
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
        getSceneROIScoringList();

    }


    public void SortROI()
    {
        if (current_node == null || current_node.currentShotNode == null)
            return;

        //Sort only passive ROIs in current shot node
        current_node.currentShotNode.Scene_ROIList.Sort((x, y) => -1 * x.score.CompareTo(y.score));
        foreach (RegionOfInterest roi in current_node.currentShotNode.Scene_ROIList)
        {
            Debug.Log("##" + roi.mesh_object);
            roi.mesh_object.transform.parent = null;
            roi.mesh_object.transform.parent = ROI_group.transform.GetChild(1);
        }
    }


    //open file
    public void openFile(string vrsPath)
    {
        SystemData data;
        //Read File
        using (StreamReader readfile = new StreamReader(vrsPath))
        {
            string serialized_str = readfile.ReadToEnd();


            //deserialize
            data = JsonConvert.DeserializeObject<SystemData>(serialized_str);

            //display message
            Debug.Log(data);

        }

        // Newly created 10/6/2018
        reader.data = data;
    }

    //Video Controller
    public void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
    {
        switch (et)
        {
            case MediaPlayerEvent.EventType.ReadyToPlay:
                //auto play
                //if (mp.Equals(mesh_content.Player))
                //{
                //    mesh_content.Player.Control.Play();
                //}
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
        float last_shot_time = current_node.current_timeMs;

        //set current shot node in current scene node
        current_node.currentShotNode = current_node.shot_list[videolist_index];
        current_node.currentShotNode.SetActive(true);

        //Disable another shot node's ROI and audio object
        //**not video player
        foreach (ShotNode shot_node in current_node.shot_list)
        {
            if (shot_node != current_node.currentShotNode)
            {
                shot_node.SetActive(false);
            }
        }

        ROI_group = current_node.currentShotNode.shotdata_obj.GetChild(2).gameObject;

        Debug.Log(last_shot_time+"#Set Video To" + current_node.currentShotNode.shotdata_obj.GetChild(0).GetChild(0).GetComponent<MediaPlayer>());

        if(mesh_content.Player != null)
            mesh_content.Player.Pause();

        //set player 
        mesh_content.Player = current_node.currentShotNode.shotdata_obj.GetChild(0).GetChild(0).GetComponent<MediaPlayer>();

        //jump to same time
        //mesh_content.Player.Control.Seek(last_shot_time);
        mesh_content.Player.Control.Play();


        //set camera angle
        Debug.Log("#Set Initial Angle:" + current_node.currentShotNode.camera_orientation.getUnityVector3());
        headset_base.rotation = current_node.currentShotNode.camera_orientation.getUnityRotationQuaternion();
    }

    public void switchSceneNode(int index)
    {
        Debug.Log("Switch To Scene Node" + index);

        //clear media player
        mesh_content.Player = null;

        //unload all contents include videos,ROIs, if current node exists
        if (current_node != null)
        {
            current_node.unloadContent();
        }

        //switch to specified scene node
        current_node = SceneNodeList[index];
        current_node.SetActive(true);

        ////Disable another shot node
        //foreach (SceneNode scene_node in SceneNodeList)
        //{
        //    if (scene_node != current_node)
        //    {
        //        scene_node.SetActive(false);
        //    }
        //}


    }


    //trigger ROI
    public void triggerROI(RegionOfInterest roi)
    {

        //reference link
        int content_type;
        int content_index;

        //get content info
        roi.getContentInfo(out content_type, out content_index);

        Debug.Log("ROI Triggerd - Changing to Scene(" + roi.scene_index + ") Shot(" + roi.video_index + ")");
        switch (content_type)
        {
            case 0:  //scene
                // Newly Created 10/6/2018
                SwitchSceneNode(content_index);
                break;
            case 1:  //video
                // Newly Created 10/6/2018
                SwitchShotNode(content_index);
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
    public string getSceneROIScoringList()
    {
        currentNode_info_roi_score_list = "----Scene ROIs Scoring List----\n";
        if (current_node != null && current_node.currentShotNode != null)
        {
            for (int i = 0; i < current_node.currentShotNode.Scene_ROIList.Count; i++)
            {
                RegionOfInterest roi = current_node.currentShotNode.Scene_ROIList[i];
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
            currentNode_info_roi_log += i++.ToString("D3") + ". " + log + "\n";

        }
    }

    // Newly Created 19/6/2018
    public void SwitchSceneNode(int scene_index)
    {
        // Check if it is the first time running
        if(current_node == null)
        {
            // Set current_node to current scene
            current_node = reader.data.scene_nodes[currSceneNumber];


            // Preload all possible path in Scene 0
            reader.LoadNextNode(0);

            currSceneNumber = 0;

            // Set to Shot 0
            SwitchShotNode(0);

            // Play all the video at the same time
            foreach (KeyValuePair<int, GameObject> item in reader.current_scene_shot_objs)
                item.Value.transform.GetChild(0).GetComponentInChildren<MediaPlayer>().Play();
        }
        else
        {            
            // Set current_node to current scene
            current_node = reader.data.scene_nodes[scene_index];


            // Play the preloaded video
            reader.other_scene_shot_objs[scene_index].transform.GetChild(0).GetComponentInChildren<MediaPlayer>().Play();
            // Stop render old shot video
            reader.current_scene_shot_objs[currShotNumber].transform.GetChild(0).GetComponentInChildren<ApplyToMesh>().MeshRenderer = null;
            // Change to render new scene video
            reader.other_scene_shot_objs[scene_index].transform.GetChild(0).GetComponentInChildren<ApplyToMesh>().MeshRenderer = videoSphere;

            // Preload and unload
            reader.LoadNextNode(scene_index);

            // Load the initial shot video from this scene
            SwitchShotNode(reader.GetOtherSceneShotIndex(scene_index));

            // Play all the other video at the same time in this scene
            foreach (KeyValuePair<int, GameObject> item in reader.current_scene_shot_objs)
                item.Value.transform.GetChild(0).GetComponentInChildren<MediaPlayer>().Play();

            currSceneNumber = scene_index;
        }
    }

    // Newly Created 19/6/2018
    public void SwitchShotNode(int shot_index)
    {
        Debug.Log("Switching shot to : Scene " + currSceneNumber + " Shot " + shot_index);

        currShotNumber = shot_index;
        //set current shot node in current scene node
        current_node.currentShotNode = reader.data.scene_nodes[currSceneNumber].shot_list[currShotNumber];

        // Active current shot object and deactive others
        foreach (KeyValuePair<int, GameObject> item in reader.current_scene_shot_objs)
        {
            if (item.Key != shot_index)
            {
                // Reset video renderer to null
                item.Value.transform.GetChild(0).GetComponentInChildren<ApplyToMesh>().MeshRenderer = null;
                // Set volume
                item.Value.transform.GetChild(0).GetComponentInChildren<MediaPlayer>().m_Volume = 0;
                item.Value.transform.GetChild(0).GetComponentInChildren<MediaPlayer>().Control.SetVolume(0);
                // Deactive ROI
                item.Value.transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                // Set video renderer to Sphere
                item.Value.transform.GetChild(0).GetComponentInChildren<ApplyToMesh>().MeshRenderer = videoSphere;
                // Set volume
                item.Value.transform.GetChild(0).GetComponentInChildren<MediaPlayer>().m_Volume = 1;
                item.Value.transform.GetChild(0).GetComponentInChildren<MediaPlayer>().Control.SetVolume(1);
                // Active ROI
                item.Value.transform.GetChild(2).gameObject.SetActive(true);
                ROI_group = item.Value.transform.GetChild(2).gameObject;
            }
        }

        //set camera angle
        Debug.Log("#Set Initial Angle:" + current_node.currentShotNode.camera_orientation.getUnityVector3());
        headset_base.rotation = current_node.currentShotNode.camera_orientation.getUnityRotationQuaternion();
    }

    // Newly Created 21/6/2018
    public ApplyToMesh GetCurrentApplyToMesh()
    {
        return reader.current_scene_shot_objs[currShotNumber].transform.GetChild(0).GetComponentInChildren<ApplyToMesh>();
    }

    // Newly Created 23/6/2018
    public string CheckVRSFileExist(string path)
    {
        // Folder must contain .vrs with this name
        string vrsName = "vrs.vrs";

        // Check exist
        if (File.Exists(path + "\\" + vrsName))
            return path + "\\" + vrsName;

        // No .vrs file found
        return null;
    }
}

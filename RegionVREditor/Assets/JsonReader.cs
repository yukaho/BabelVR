using System.Collections.Generic;
using UnityEngine;
using Babel.System.Data;
using RenderHeads.Media.AVProVideo;
using UnityEngine.UI;

public class JsonReader : MonoBehaviour
{
    // GameObject Prefab
    public GameObject shotdata_prefab;
    public GameObject mediaplayer_prefab;

    // List of storing cureent all shots of current scene
    public Dictionary<int, GameObject> current_scene_shot_objs;
    // List of storing those next possible scene-shot (one only)
    public Dictionary<int, GameObject> other_scene_shot_objs;
    private List<int> scenes_index;
    private List<int> shots_index;

    // serialised data from vrs
    public SystemData data;
    // The current scene node reference
    public SceneNode root_scene_node;

    // VRPlayerCore
    private VRPlayerCore core;

    // ErrorMessage GameObject (UI)
    public GameObject errorMessageObj;
    private float showErrorTime = 5f;
    private float currentErrorAnimTime;

    private void Start()
    {
        current_scene_shot_objs = new Dictionary<int, GameObject>();
        other_scene_shot_objs = new Dictionary<int, GameObject>();
        scenes_index = new List<int>();
        shots_index = new List<int>();

        core = GetComponent<VRPlayerCore>();
    }

    //Load next node and the following path node
    public void LoadNextNode(int scene_index)
    {
        SceneNode scenenode_data = data.scene_nodes[scene_index];

        //Set the selected node to root and unload all unselected node
        if (current_scene_shot_objs.Count > 0)
        {
            //SetSelectedNodeToRoot(scene_index, shot_index);
            UnloadChild(scene_index);
        }
        LoadSceneNode(scene_index, true);
        GetNextSceneNodeIndex(scenenode_data);

        //Debug.Log("child index count: " + scenes_index.Count);

        for(int i = 0; i < scenes_index.Count; i++)
        {
            //Debug.Log("LoadNextNode: scenes_index " + i + "= " + scenes_index[i]);
            LoadSceneNode(scenes_index[i], false);
        }
    }

    //Get the selected scene's shot index
    public int GetOtherSceneShotIndex(int scene_index)
    {

        for (int i = 0; i < data.scene_nodes[scene_index].initailize_actions.Count; i++)
        {
            SceneAction temp_scene_action = data.scene_nodes[scene_index].initailize_actions[i];

            if (temp_scene_action.action_flag.ToString().Equals("SetVideo"))
            {
                return int.Parse(temp_scene_action.parameters_list[0].ToString());
                //Debug.Log("GetOtherSceneShotIndex: shot index = " + shot_index);
            }
        }
        return -1;
    }

    //Remove all unselected node excepted the selected node
    private void UnloadChild(int scene_index)
    {
        for (int i = 0; i < current_scene_shot_objs.Count; i++)
        {
            current_scene_shot_objs[i].transform.GetChild(0).GetComponentInChildren<MediaPlayer>().CloseVideo();
            Destroy(current_scene_shot_objs[i]);
        }

        current_scene_shot_objs.Clear();

        int shot_index = GetOtherSceneShotIndex(scene_index);

        current_scene_shot_objs.Add(shot_index, other_scene_shot_objs[scene_index]);

        foreach (KeyValuePair<int, GameObject> item in other_scene_shot_objs)
        {
            if (item.Key == scene_index)
            {
                continue;
            }

            item.Value.transform.GetChild(0).GetComponentInChildren<MediaPlayer>().CloseVideo();
            Destroy(item.Value);
        }

        other_scene_shot_objs.Clear();
        scenes_index.Clear();
    }

    //Load scene node
    private void LoadSceneNode(int scene_index, bool is_current_scene)
    {
        SceneNode scenenode_data = data.scene_nodes[scene_index];

        if (is_current_scene == true)
        {
            //Debug.Log("LoadSceneNode: shot list size " + scenenode_data.shot_list.Count);
            for (int i = 0; i < scenenode_data.shot_list.Count; i++)
            {
                //Debug.Log("LoadSceneNode: CheckNextNodeExist " + CheckNextNodeExist(i, current_scene_shot_objs));
                //Debug.Log("LoadSceneNode: CheckNextNodeExist current_scene_shot_objs.Count " + current_scene_shot_objs.Count);
                if (!CheckNextNodeExist(i, current_scene_shot_objs))
                {
                    GameObject temp_obj = Instantiate(shotdata_prefab, gameObject.transform);

                    temp_obj.name = "Scene<" + scene_index + ">" + "Shot<" + i + ">";
                    LoadShotNode(temp_obj.transform, scenenode_data.shot_list[i]);
                    temp_obj.transform.GetChild(2).gameObject.SetActive(false);   // deactive roi
                    current_scene_shot_objs.Add(i, temp_obj);
                    //Debug.Log("LoadSceneNode: current_scene_shot_objs size = " + current_scene_shot_objs.Count);
                }
            }
        }
        else
        {
            int temp_shot_index = GetOtherSceneShotIndex(scene_index);

            //Debug.Log("LoadSceneNode: temp_shot_index = " + temp_shot_index);
            //Debug.Log("LoadSceneNode: action_flag is " + temp_scene_action.action_flag);
            //Debug.Log("LoadSceneNode: action_flag == SetVideo? " + temp_scene_action.action_flag.ToString().Equals("SetVideo"));

            if (temp_shot_index >= 0)
            {
                if (!CheckNextNodeExist(scene_index, other_scene_shot_objs))
                {
                    GameObject temp_obj = Instantiate(shotdata_prefab, gameObject.transform);

                    temp_obj.name = "Scene<" + scene_index + ">" + "Shot<" + temp_shot_index + ">";
                    LoadShotNode(temp_obj.transform, scenenode_data.shot_list[temp_shot_index]);
                    temp_obj.transform.GetChild(2).gameObject.SetActive(false);     // deactive roi
                    other_scene_shot_objs.Add(scene_index, temp_obj);
                    Debug.Log("LoadSceneNode: other_scene_shot_objs size = " + other_scene_shot_objs.Count);
                }
            }
        }

        scenenode_data.isLoaded = true;
    }

    //Load shot node
    private void LoadShotNode(Transform scene_obj, ShotNode shotnode_data)
    {
        //
        //Video
        //
        Transform video = scene_obj.GetChild(0);        // Get Video object
        GameObject obj = Instantiate(mediaplayer_prefab, video.transform);
        MediaPlayer videoplayer = obj.GetComponent<MediaPlayer>();
        //core.current_node = scenenode_data;

        videoplayer.m_VideoLocation = new MediaPlayer.FileLocation();
        videoplayer.m_VideoPath = core.videoPath + shotnode_data.movie_dir;
        videoplayer.m_StereoPacking = StereoPacking.TopBottom;
        videoplayer.m_Loop = false;
        //videoplayer.Events.AddListener(shotnode_data.OnVideoEvent);
        videoplayer.Events.AddListener(core.OnVideoEvent);

        string[] split = videoplayer.m_VideoPath.Split('/');
        videoplayer.gameObject.name = "Video<" + split[split.Length - 1] + ">";

        bool found = videoplayer.OpenVideoFromFile(videoplayer.m_VideoLocation, videoplayer.m_VideoPath, false);
        if (!found)
            videoplayer.gameObject.name = "Video< Not Found >";

        //
        //ROI
        //
        Transform group = scene_obj.GetChild(2);        // Get ROI child object
        Transform shot_rois = group.GetChild(0);      // Get Shot ROI
        Transform scene_rois = group.GetChild(1);     // Get Scene ROI

        //load active ROI from current shot node
        foreach (RegionOfInterest roi in shotnode_data.Shot_ROIList)
        {
            GameObject created_mesh = roi.createObject();
            created_mesh.transform.parent = shot_rois;
            created_mesh.GetComponent<RegionOfInterestObject>().roi.flag = RegionOfInterestFlag.Shot;
            created_mesh.name = "Shot_ROI_#" + RegionOfInterest.active_roi_count.ToString("D3");
            RegionOfInterest.active_roi_count++;
            //OnShotNodeContentLoaded(this, new EventArgs());
        }


        //load active ROI from current node
        foreach (RegionOfInterest roi in shotnode_data.Scene_ROIList)
        {
            GameObject created_mesh = roi.createObject();
            created_mesh.transform.parent = scene_rois;
            created_mesh.GetComponent<RegionOfInterestObject>().roi.flag = RegionOfInterestFlag.Scene;
            created_mesh.name = "Scene_ROI_#" + RegionOfInterest.passive_roi_count.ToString("D3");
            RegionOfInterest.passive_roi_count++;
            //OnShotNodeContentLoaded(this, new EventArgs());
        }

        //
        //Audio
        //
        GameObject AudioObjectGroup = scene_obj.GetChild(1).gameObject;

        foreach (AudioData data in shotnode_data.AudioData_List)
        {
            GameObject created_audio_obj = data.createObject(core.audioPath + data.file_dir);
            created_audio_obj.transform.parent = AudioObjectGroup.transform;
            data.audio_obj.Load();
            //OnShotNodeContentLoaded(this, new EventArgs());
        }
    }

    //Get all selectable node according to the root node
    private void GetNextSceneNodeIndex(SceneNode scenenode)
    {
        ShotNode tempShotnode;
        RegionOfInterest tempROI;

        for(int i = 0; i < scenenode.shot_list.Count; i++)
        {
            tempShotnode = scenenode.shot_list[i];

            //If content type is 0, add the scene index to scene_index array
            // Debug.Log("GetNextSceneNodeIndex: shot_list size " + scenenode.shot_list.Count);
            for (int j = 0; j < tempShotnode.Shot_ROIList.Count; j++)
            {
                //Debug.Log("GetNextSceneNodeIndex: Shot_ROIList size " + tempShotnode.Shot_ROIList.Count);
                tempROI = tempShotnode.Shot_ROIList[j];

                if(tempROI.content_type == 0)
                {
                    scenes_index.Add(tempROI.scene_index);
                }
            }

            for(int j = 0; j < tempShotnode.Scene_ROIList.Count; j++)
            {
                tempROI = tempShotnode.Scene_ROIList[j];

                if(tempROI.content_type == 0)
                {
                    scenes_index.Add(tempROI.scene_index);
                }
            }
        }

        //Debug.Log("GetNextSceneNodeIndex: scenes_index size " + scenes_index.Count);
    }

    //Check node is created or not
    private bool CheckNextNodeExist(int index, Dictionary<int, GameObject> dictionary)
    {
        if (dictionary.ContainsKey(index))
        {
            return true;
        }

        return false;
    }

    // Check .vrs error before applying
    public bool CheckVRSError()
    {
        string errorText = "- Error -\n";

        bool hvError = false;

        Text errorMessage = errorMessageObj.GetComponentInChildren<Text>();

        for (int i = 0; i < data.scene_nodes.Count; i++)
        {
            SceneNode node = data.scene_nodes[i];

            // no end actions recorded
            if (node.end_actions.Count <= 0)
            {
                errorMessage.text = errorText + "Missing SwitchSceneDefault end action.\n" + "In Scene ( " + i + " )";
                hvError = true;
                break;
            }

            // more than 2 end actions recorded
            if ( node.end_actions.Count > 2 )
            {
                errorMessage.text = errorText + "Only TWO end actions in each scene are allowed.\n" + "In Scene ( " + i + " )";
                hvError = true;
                break;
            }

            // missing SwitchSceneDefault at the beginning
            if (node.end_actions[0].action_flag != SceneAction.Flag.SwitchSceneDefault)
            {
                errorMessage.text = errorText + "All scenes end actions must have SwitchSceneDefault at first.\n" + "In Scene ( " + i + " )";
                hvError = true;
                break;
            }
        }

        // Display error message
        if (hvError)
        {
            errorMessageObj.SetActive(true);
            currentErrorAnimTime = showErrorTime;
            return true;
        }

        // return no error
        return false;
    }

    // Update error message showing time
    private void Update()
    {
        if (currentErrorAnimTime > 0)
        {
            currentErrorAnimTime -= Time.deltaTime;
        }
        else
        {
            errorMessageObj.SetActive(false);
        }
    }
}

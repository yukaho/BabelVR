using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using RenderHeads.Media.AVProVideo;
using System;

namespace Babel.System.Data
{
    public class ShotNode
    {
        //Shot ID
        [JsonPropertyAttribute]
        public int shot_id;

        //Movie directory path
        [JsonPropertyAttribute]
        public string movie_dir;


        [JsonPropertyAttribute]
        public SimpleVector3 camera_orientation;


        //Region Of Interest List
        [JsonPropertyAttribute]
        public List<RegionOfInterest> Shot_ROIList;

        [JsonPropertyAttribute]
        public List<RegionOfInterest> Scene_ROIList;

        [JsonPropertyAttribute]
        public List<AudioData> AudioData_List;

        //factory id generation
        [JsonIgnoreAttribute]
        public static int shot_count = 0;

        [JsonPropertyIgnore]
        public bool isReadyToPlay;

        //ignore attribute
        [JsonPropertyIgnore]
        public string video_filename;

        [JsonPropertyIgnore]
        public Transform shotdata_obj;

        [JsonPropertyIgnore]
        public event EventHandler OnShotNodeContentLoaded;

        [JsonPropertyIgnore]
        MediaPlayer videoplayer;

        public ShotNode()
        {
            Shot_ROIList = new List<RegionOfInterest>();
            Scene_ROIList = new List<RegionOfInterest>();
            AudioData_List = new List<AudioData>();
            movie_dir = "";
            shot_id = shot_count++;
            isReadyToPlay = false;
        }

        public void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode errorCode)
        {
            switch (et)
            {
                case MediaPlayerEvent.EventType.ReadyToPlay:
                    string[] split = mp.m_VideoPath.Split('/');
                    mp.gameObject.name = "Video<" + split[split.Length - 1] + ">";
                    Debug.Log("##READYTOPLAY");
                    isReadyToPlay = true;
                    break;
                case MediaPlayerEvent.EventType.FirstFrameReady:
                    break;
                case MediaPlayerEvent.EventType.FinishedPlaying:
                    break;
            }
        }

        public int getFilesCount()
        {
            int video_files_count = 1;
            int audio_files_count = AudioData_List.Count;
            int roi_files_count = Shot_ROIList.Count + Scene_ROIList.Count;
            return (video_files_count + audio_files_count + roi_files_count);
        }

        public void Load(VRPlayerCore core, Transform shotdata_obj)
        {
            //Link Game Object to Node Object
            this.shotdata_obj = shotdata_obj;
       
            //
            //Video
            //
            //Instantiate From Prefab, AVPro Media Player Prefab
            Transform video = shotdata_obj.FindChild("Video");

            GameObject obj = Resources.Load<GameObject>("Prefab/AVPro Video Media Player");
            obj = GameObject.Instantiate(obj, video.transform);

            videoplayer = obj.GetComponent<MediaPlayer>();
            videoplayer.m_VideoLocation = new MediaPlayer.FileLocation();
            videoplayer.m_VideoPath = movie_dir;
            videoplayer.m_StereoPacking = StereoPacking.TopBottom;
            videoplayer.m_Loop = false;
            videoplayer.Events.AddListener(OnVideoEvent);
            videoplayer.Events.AddListener(core.OnVideoEvent);
            videoplayer.gameObject.name = "Video<Loading...>";
            bool found = videoplayer.OpenVideoFromFile(videoplayer.m_VideoLocation, videoplayer.m_VideoPath, false);
            if (!found)
            {
                Debug.Log("File not found!");
                videoplayer.gameObject.name = "Video<Not Found>";
            }


            //
            //ROI
            //
            Transform group = shotdata_obj.Find("ROIs").transform;
            Transform active_rois = group.FindChild("ActiveROIs");
            Transform passive_rois = group.FindChild("PassiveROIs");

            //load active ROI from current shot node
            foreach (RegionOfInterest roi in Shot_ROIList)
            {
                GameObject created_mesh = roi.createObject();
                created_mesh.transform.parent = active_rois;
                created_mesh.GetComponent<RegionOfInterestObject>().roi.flag = RegionOfInterestFlag.Shot;
                created_mesh.name = "A_ROI_#" + RegionOfInterest.active_roi_count.ToString("D3");
                RegionOfInterest.active_roi_count++;
                OnShotNodeContentLoaded(this, new EventArgs());
            }


            //load active ROI from current node
            foreach (RegionOfInterest roi in Scene_ROIList)
            {
                GameObject created_mesh = roi.createObject();
                created_mesh.transform.parent = passive_rois;
                created_mesh.GetComponent<RegionOfInterestObject>().roi.flag = RegionOfInterestFlag.Scene;
                created_mesh.name = "P_ROI_#" + RegionOfInterest.passive_roi_count.ToString("D3");
                RegionOfInterest.passive_roi_count++;
                OnShotNodeContentLoaded(this, new EventArgs());
            }

            //
            //Audio
            //

            GameObject AudioObjectGroup = shotdata_obj.FindChild("Audio").gameObject;

            foreach (AudioData data in AudioData_List)
            {
                // string = dataPath
                GameObject created_audio_obj = data.createObject("");
                created_audio_obj.transform.parent = AudioObjectGroup.transform;
                data.audio_obj.Load();
                OnShotNodeContentLoaded(this, new EventArgs());
            }
            
        }



        public string getShotVideoFileName()
        {
            if (video_filename == null)
            {

                string[] split = movie_dir.Split('/');
                video_filename = split[split.Length - 1];
            }

            return video_filename;
        }

        public void SetActive(bool b)
        {            
            //turn on/off audio
            shotdata_obj.GetChild(1).gameObject.SetActive(b);

            //turn on/off roi
            shotdata_obj.GetChild(2).gameObject.SetActive(b);
        }
    }
}

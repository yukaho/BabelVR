﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using RenderHeads.Media.AVProVideo;

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
        public List<RegionOfInterest> Active_ROIList;

        [JsonPropertyAttribute]
        public List<RegionOfInterest> Passive_ROIList;

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

        public ShotNode()
        {
            Active_ROIList = new List<RegionOfInterest>();
            Passive_ROIList = new List<RegionOfInterest>();
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
                    //auto play
                    mp.Control.Play();
                    break;
                case MediaPlayerEvent.EventType.FirstFrameReady:
                    isReadyToPlay = true;
                    break;
                case MediaPlayerEvent.EventType.FinishedPlaying:
                    break;
            }
        }

        public void loadShotNode()
        {

            //
            //ROI
            //
            Transform group = GameObject.Find("RegionOfInterestGroup").transform;


            Transform active_rois = group.FindChild("ActiveROIGroup");
            Transform passive_rois = group.FindChild("PassiveROIGroup");

            //load active ROI from current shot node
            foreach (RegionOfInterest roi in Active_ROIList)
            {
                GameObject created_mesh = roi.createMesh();
                created_mesh.transform.parent = active_rois;
                created_mesh.GetComponent<RegionOfInterestObject>().roi.flag = RegionOfInterestFlag.Active;
                created_mesh.name = "A_ROI_#" + RegionOfInterest.active_roi_count.ToString("D3");
                RegionOfInterest.active_roi_count++;
            }


            //load active ROI from current node
            foreach (RegionOfInterest roi in Passive_ROIList)
            {
                GameObject created_mesh = roi.createMesh();
                created_mesh.transform.parent = passive_rois;
                created_mesh.GetComponent<RegionOfInterestObject>().roi.flag = RegionOfInterestFlag.Passive;
                created_mesh.name = "P_ROI_#" + RegionOfInterest.passive_roi_count.ToString("D3");
                RegionOfInterest.passive_roi_count++;
            }

            //
            //Audio
            //

            GameObject AudioObjectGroup = GameObject.Find("AudioObjectGroup");

            foreach (AudioData data in AudioData_List)
            {
                GameObject created_audio_obj = data.createObject();
                created_audio_obj.transform.parent = AudioObjectGroup.transform;

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
    }
}

using System.Collections;
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

        //factory id generation
        [JsonIgnoreAttribute]
        public static int shot_count = 0;

        [JsonPropertyIgnore]
        public bool isReadyToPlay;


        //ignore attribute

        public ShotNode()
        {
            Active_ROIList = new List<RegionOfInterest>();
            Passive_ROIList = new List<RegionOfInterest>();
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

        public void loadROI()
        {
            Transform group = GameObject.Find("RegionOfInterestGroup").transform;


            //load active ROI from current shot node
            foreach (RegionOfInterest roi in Active_ROIList)
            {
                GameObject created_mesh = roi.createMesh();
                created_mesh.transform.parent = group.FindChild("ActiveROIGroup");
                created_mesh.GetComponent<RegionOfInterestMesh>().roi.flag = RegionOfInterestFlag.Active;
                created_mesh.name = "A_ROI_#" + RegionOfInterest.active_roi_count.ToString("D3");
                RegionOfInterest.active_roi_count++;
            }


            //load active ROI from current node
            foreach (RegionOfInterest roi in Passive_ROIList)
            {
                GameObject created_mesh = roi.createMesh();
                created_mesh.transform.parent = group.FindChild("PassiveROIGroup");
                created_mesh.GetComponent<RegionOfInterestMesh>().roi.flag = RegionOfInterestFlag.Passive;
                created_mesh.name = "P_ROI_#" + RegionOfInterest.passive_roi_count.ToString("D3");
                RegionOfInterest.passive_roi_count++;
            }
        }
    }
}

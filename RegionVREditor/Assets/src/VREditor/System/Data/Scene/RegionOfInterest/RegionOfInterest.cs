using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Babel.System.Data
{

    public class RegionOfInterest
    {


        public enum ROI_Shape { RECTANGLE, CIRCLE }


        /// <summary>
        /// JSON Data
        /// </summary>
        /// 


        [JsonPropertyAttribute]
        public string roi_name;

        [JsonPropertyAttribute]
        public ROI_Shape roi_detection_shape;

        [JsonPropertyAttribute]
        public Keyframe[] animation_data;

        [JsonPropertyAttribute]
        [JsonConverter(typeof(StringEnumConverter))]
        public RegionOfInterestFlag flag;


        [JsonPropertyAttribute]
        public int content_type = -1;

        [JsonPropertyAttribute]
        public int scene_index = -1;

        [JsonPropertyAttribute]
        public int video_index = -1;

        [JsonPropertyAttribute]
        public int audio_index = -1;

        [JsonPropertyAttribute]
        public SimpleVector3 position;

        [JsonPropertyAttribute]
        public SimpleVector3 rotation;

        /// <summary>
        ///  Unity Elements that are not required to save in JSON
        /// </summary>
        /// 



        //interest scoring
        [JsonIgnore]
        public int score;

        [JsonIgnore]
        public GameObject mesh_object;

        //VR Player Core
        [JsonIgnore]
        public VRPlayerCore core;

        [JsonIgnore]
        public static int active_roi_count = 0;

        [JsonIgnore]
        public static int passive_roi_count = 0;

        [JsonIgnore]
        public static int roi_count = 0;

        public RegionOfInterest()
        {
            //default
            roi_name = "ROI#" + (roi_count++);


            //set initial score to zero
            score = 0;

            ////set up initial position
            //position = new SimpleVector3(0, 0, 0);


            ////set up initial rotation
            //rotation = new SimpleVector3(0, 0, 0);


        }


        public GameObject createMesh()
        {

            //get core in game
            this.core = GameObject.Find("VRPlayerSystem").GetComponent<VRPlayerCore>();
            GameObject new_mesh = Resources.Load<GameObject>("Prefab/RegionOfInterestObject");
            new_mesh = GameObject.Instantiate(new_mesh);
            RegionOfInterestObject roi_m = new_mesh.GetComponent<RegionOfInterestObject>();
            roi_m.initializeMesh(this);
            this.mesh_object = new_mesh;

            //update current frame index based current frame        
            int find_index = -1;
            for (int i = 0; i < animation_data.Length; i++)
            {
                if (core.current_node.current_frame >= animation_data[i].time_code)
                {
                    find_index++;
                }
            }
            roi_m.animation_keyframe_current_index = find_index;

            if (roi_m.animation_keyframe_current_index + 1 < animation_data.Length)
            {
                roi_m.animation_keyframe_next_index = roi_m.animation_keyframe_current_index + 1;
            }
            else
            {
                roi_m.animation_keyframe_next_index = roi_m.animation_keyframe_current_index;
            }


            return new_mesh;
        }

        //public void triggerAction()
        //{
        //    //trigger embeded action 
        //    action.execute(this.core);
        //}



        public override string ToString()
        {



            return this.roi_detection_shape.ToString();
            //return vertics[0] + "," +
            //    vertics[1] + "," +
            //    vertics[2] + "," +
            //    vertics[3];
        }

        public void addScore(int score)
        {
            this.score += score;
        }


        public void getContentInfo(out int content_type, out int content_index)
        {
            content_type = this.content_type;
            content_index = -1;
            switch (this.content_type)
            {
                case 0:  //scene
                    content_index = scene_index;
                    break;
                case 1:  //video
                    content_index = video_index;
                    break;
                case 2: //audio
                    content_index = audio_index;
                    break;
            }



        }
    }


    public enum RegionOfInterestFlag
    {
        Passive,
        Active
    }


}
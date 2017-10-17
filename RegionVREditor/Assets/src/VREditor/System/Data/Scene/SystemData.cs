using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Babel.System.Data;

namespace Babel.System.Data
{

    [JsonObject(MemberSerialization.OptIn)]
    public class SystemData
    {
        //Json Saved Data
        //Stored Data
        [JsonPropertyAttribute]
        public DateTime savedTime;

        [JsonPropertyAttribute]
        public String file_name;

        [JsonPropertyAttribute]
        public String version = "0.1a";

        //node list
        [JsonPropertyAttribute]
        public List<SceneNode> scene_nodes { get; set; }


        public SystemData()
        {
            scene_nodes = new List<SceneNode>();
        }


        public void reset()
        {
            foreach (SceneNode n in scene_nodes)
            {
                n.reset();
            }

            scene_nodes.Clear();
        }


        //print out all data
        public override string ToString()
        {
            return "File: <" + file_name + ">\n" +
                "File Version: <" + version + ">\n" +
                "Date: <" + savedTime + ">" + allNode();
        }

        public string allNode()
        {

            string res = "";


            foreach (SceneNode n in scene_nodes)
            {
                res += "\n" + n;
            }
            return res;
        }
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AudioData
{
    //audio name
    public string audio_name;

    //file directory
    public string file_dir;

    //keyframes data
    public Keyframe[] animation_data;

    public GameObject createObject()
    {
        //load prefab from asset
        GameObject return_obj = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/AudioObject"));
        return_obj.GetComponent<AudioObject>().initializeObject(this);
   
        return return_obj;
    }


}


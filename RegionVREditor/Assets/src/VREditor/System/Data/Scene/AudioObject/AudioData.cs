using Babel.System.Data;
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

    //audio playtime
    public int audio_playtime;

    //audio volume
    public float audio_volume;

    //audio volume
    public bool audio_loop;

    //keyframes data
    public Keyframe[] animation_data;

    [JsonPropertyIgnore]
    public AudioObject audio_obj;

    [JsonPropertyIgnore]
    public string absolutePath;

    public GameObject createObject(string audioFilePath)
    {
        absolutePath = audioFilePath;

        //load prefab from asset
        GameObject return_obj = GameObject.Instantiate(Resources.Load<GameObject>("Prefab/AudioObject"));
        return_obj.GetComponent<AudioObject>().initialize(this);
   
        return return_obj;
    }


}


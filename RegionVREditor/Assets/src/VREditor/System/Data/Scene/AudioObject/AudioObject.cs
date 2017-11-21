using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AudioObject : KeyframedMonoBehaviour
{

    AudioData audio_data;
    AudioSource source;
    AudioClip clip;
    WaveFileReader reader;
    bool oneshoted;
    void Start()
    {

    }

    public override void initialize(params object[] obj)
    {
        initializeObject((AudioData)obj[0]);
        base.initialize();
    }

    public void initializeObject(AudioData data)
    {
        Debug.Log("Load Audio");

        //reference audio data
        audio_data = data;

        //set oneshoted equals false
        oneshoted = false;

        //reference the vr player system
        core = GameObject.Find("VRPlayerSystem").GetComponent<VRPlayerCore>();

        //reference animationdata from roi
        this.animation_data = data.animation_data;

        //set name of object
        this.name = data.audio_name;
        this.GetComponent<TextMesh>().name = this.name;

        //load audio source
        loadAudioSource(audio_data.file_dir);
    }

    public void loadAudioSource(string path)
    {
        
        //get audiosource component
        source = GetComponent<AudioSource>();

        //load audio to file reader
        reader = new WaveFileReader(path);

        //create audio clip
        clip = AudioClip.Create(reader.getFileName(), reader.samples_array.Length, 2, reader.getSampleRate(), false);

        //set samples array
        clip.SetData(reader.samples_array, 0);

        //link clip & do configuration
        source.clip = clip;
        source.loop = audio_data.audio_loop;
        source.volume = audio_data.audio_volume;

    }
    void Update()
    {
        //update keyframe from super class
        base.updateAnimation();


        //play at time
        if (!oneshoted && current_frame >= audio_data.audio_playtime && source.clip.loadState == AudioDataLoadState.Loaded)
        {
            source.Play();
            oneshoted = true;
        }
    }
}


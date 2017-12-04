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
        //reference audio data on both side
        audio_data = data;
        data.audio_obj = this;

        //set oneshoted equals false
        oneshoted = false;

        //reference the vr player system
        core = GameObject.Find("VRPlayerSystem").GetComponent<VRPlayerCore>();

        //reference animationdata from roi
        this.animation_data = data.animation_data;

        //set name of object
        string[] split = audio_data.file_dir.Split('/');
        this.name = "Audio<" + split[split.Length - 1] + ">";
        this.GetComponent<TextMesh>().name = this.name;

        //get audiosource component
        source = GetComponent<AudioSource>();


    }

    public void Load()
    {


        //load audio to file reader
        reader = new WaveFileReader(audio_data.file_dir);

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

        Play();
    }

    public void Play()
    {
        if (clip == null)
            return;

        //play at time
        if (!oneshoted && current_frame >= audio_data.audio_playtime && source.clip.loadState == AudioDataLoadState.Loaded)
        {
            source.Play();
            oneshoted = true;
        }
    }
}


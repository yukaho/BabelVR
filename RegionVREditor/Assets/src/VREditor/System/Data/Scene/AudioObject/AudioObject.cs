using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class AudioObject : KeyframedMonoBehaviour
{
    void Start()
    {

    }

    public void initializeObject(AudioData data)
    {
        Debug.Log("Load Audio");
        //reference the vr player system
        core = GameObject.Find("VRPlayerSystem").GetComponent<VRPlayerCore>();

        //reference animationdata from roi
        this.animation_data = data.animation_data;

        //set name of object
        this.name = data.audio_name;
        this.GetComponent<TextMesh>().name = this.name;

        //load audio source
        loadAudioSource("");
    }

    public void loadAudioSource(string path)
    {
        //wait for implement
        //AudioSource source = this.GetComponent<AudioSource>();
        //source.clip = Resources.Load<AudioClip>("Media/Audio/M249 SAW");
        AudioSource source = GetComponent<AudioSource>();
        WaveFileReader reader = new WaveFileReader("X:/Tamar/Work/ProgramTool/WaveFileReader/TestFiles/LeanOn.wav");
        AudioClip clip = AudioClip.Create("TEST", reader.samples_array.Length,2, reader.getSampleRate(),false);
        clip.SetData(reader.samples_array, 0);
        source.clip = clip;
        source.loop = true;
        source.Play();

    }
    void Update()
    {
        //update keyframe from super class
        base.updateAnimation();
    }
}


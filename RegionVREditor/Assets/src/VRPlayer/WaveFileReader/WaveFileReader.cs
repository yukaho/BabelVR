using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


public class WaveFileReader : FileReader
{

    public float[] samples_array;

    public WaveFileReader(string file_dir)
    {
        Read(file_dir);
    }


    protected override void Read(string file_dir)
    {
        //read file to byte arrary
        base.Read(file_dir);

        samples_array = new float[5];

        Console.WriteLine("Raw Audio Data Size:" + getSubchunk2Size());
        Console.WriteLine("BitsPerSample:" + getBitsPerSample());
        int totalSamples = getSubchunk2Size() / (getBitsPerSample() / 8);
        Console.WriteLine("TotalSamples:" + totalSamples);
        samples_array = new float[totalSamples];
        int sample_start_index = 44;

        for (int i = 0; i < totalSamples; i++)
        {
            samples_array[i] = BitConverter.ToInt16(data_array, sample_start_index+(i*2)) /(float) short.MaxValue;
           // Console.WriteLine(samples_array[i]);
        }
    }

    public string getChunkID()
    {
        return Encoding.ASCII.GetString(this.getByteArray(), 0, 4);
    }

    public int getChunkSize()
    {
        return BitConverter.ToInt32(getByteArray(), 4);
    }

    public string getFormat()
    {
        return Encoding.ASCII.GetString(this.getByteArray(), 8, 4);
    }

    public int getAudioFormat()
    {
        return BitConverter.ToInt16(getByteArray(), 20);
    }

    public int getNumChannels()
    {
        return BitConverter.ToInt16(getByteArray(), 22);
    }

    public int getSampleRate()
    {
        return BitConverter.ToInt32(getByteArray(), 24);
    }


    public int getSubchunk2Size()
    {
        return BitConverter.ToInt32(getByteArray(), 40);
    }

    public int getBitsPerSample()
    {
        return BitConverter.ToInt16(getByteArray(), 34);
    }





}


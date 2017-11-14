using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class KeyframedMonoBehaviour : MonoBehaviour
{
    [Header("Animation Data")]
    protected int total_keyframe_length;
    protected string from_keyframe_index;
    protected string to_keyframe_index;
    protected int current_frame;
    protected int target_frame;
    protected int total_frames;
    protected Keyframe current_keyframe;
    protected Keyframe target_keyframe;

    //animation data
    public Keyframe[] animation_data;
    protected VRPlayerCore core;

    //current keyframe
    public int animation_keyframe_current_index = 0;
    public int animation_keyframe_next_index = 0;
    public Keyframe animation_current_keyframe;

    void Update()
    {


    }

    protected void updateAnimation()
    {
        //return if the player is not playing
        if (core==null||
            core.current_node == null ||
            core.current_node.currentShotNode == null ||
            !core.current_node.currentShotNode.isReadyToPlay)
            return;

        //update keyframe
        updateKeyframe();

        //update position,rotation & scale tranformation from keyframes
        updateTransform();
    }


    private bool updateKeyframe()
    {
        if (animation_data.Length <= 0)
            return false;

        //Link totatl frames of video
        total_frames = core.current_node.total_frames;

        //Link total "keyframe" length of array
        total_keyframe_length = animation_data.Length;

        //Link current & next index index of key frame
        from_keyframe_index = animation_keyframe_current_index + "";
        to_keyframe_index = animation_keyframe_next_index + "";

        //set current keyframe
        current_keyframe = animation_data[animation_keyframe_current_index];

        //set target keyframe
        target_keyframe = animation_data[animation_keyframe_next_index];

        //Link current frame of video
        current_frame = core.current_node.current_frame;

        //Link frame of next keyframe
        target_frame = target_keyframe.time_code;



        //if current frame reach target time_code, jump to next key frame
        if (current_frame == target_frame)
        {
            ////update keyframe
            animation_keyframe_current_index = animation_keyframe_next_index;
            animation_keyframe_next_index++;

            //End key frame, stop update
            if (animation_keyframe_next_index >= animation_data.Length)
            {
                to_keyframe_index = "End of KeyFrame";
                animation_keyframe_next_index = animation_keyframe_current_index;
            }
        }


        return true;

    }
    private void updateTransform()
    {
        if (current_keyframe == null || target_keyframe == null)
            return;

        //update positiion from key frame
        updatePosition();

        //update scale frome key frame
        updateScale();

        //update rotation frome key frame
        updateRotation();
    }
    private void updatePosition()
    {

        //create incremental vector
        Vector3 incremental_vector;

        Vector3 test = current_keyframe.position.getUnityVector3();

        //calculate incremental vector
        calLinearVector(current_keyframe.position.getUnityVector3(),
            target_keyframe.position.getUnityVector3(), current_frame,
            target_keyframe.time_code, out incremental_vector);

        //update position of this ROI
        this.transform.position = current_keyframe.position.getUnityVector3() + incremental_vector;



    }
    private void updateScale()
    {
        //create incremental vector
        Vector3 incremental_vector;

        //calculate incremental vector
        calLinearVector(current_keyframe.scale.getUnityVector3(),
            target_keyframe.scale.getUnityVector3(),
            current_frame,
            target_keyframe.time_code, out incremental_vector);

        //update position of this ROI
        Vector3 new_scale = current_keyframe.scale.getUnityVector3() + incremental_vector;

        //prevent non-centre pivot case
        new_scale.z = 1;

        //assign new scale
        this.transform.localScale = new_scale;
        //this.GetComponent<MeshCollider>().transform.localScale = this.transform.localScale;
    }
    private void updateRotation()
    {


        //create incremental vector
        Vector3 incremental_vector;

        //calculate incremental vector
        calLinearVector(current_keyframe.rotation.getUnityVector3(),
            target_keyframe.rotation.getUnityVector3(),
            current_frame,
            target_keyframe.time_code, out incremental_vector);

        //update position of this ROI
        // this.transform.rotation = Quaternion.Euler(current_keyframe.scale.getUnityVector3() + incremental_vector);

        float new_rotation_z = (current_keyframe.scale.getUnityVector3() + incremental_vector).z;


        this.transform.Rotate(Vector3.forward, incremental_vector.z);
        try
        {
            this.GetComponent<MeshCollider>().transform.rotation = this.transform.rotation;
        }
        catch (Exception e)
        {

        }




    }


    /// <summary>
    /// calculate linear vector between 2 vectors
    /// </summary>
    /// <param name="p0"> vector p0 </param>
    /// <param name="p1"> vector p1 </param>
    /// <param name="current_frame"> current frame of video</param>
    /// <param name="target_frame"> target frame of video</param>
    /// <param name="calculated_vector"> result </param>
    public void calLinearVector(Vector3 p0, Vector3 p1, int current_frame, int target_frame, out Vector3 calculated_vector)
    {
        if (current_frame == 0)
        {
            //return zero vector out if current frame is zero
            calculated_vector = new Vector3(0, 0, 0);
            return;
        }
        calculated_vector = new Vector3();
        Vector3 direction_vector = p1 - p0;
        calculated_vector = direction_vector * ((float)current_frame / target_frame);

    }
}


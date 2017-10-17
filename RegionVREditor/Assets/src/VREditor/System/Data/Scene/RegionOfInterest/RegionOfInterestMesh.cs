using Babel.System.Data;
using System.Collections;
using System.Collections.Generic;
using RenderHeads.Media.AVProVideo;
using UnityEngine;

public class RegionOfInterestMesh : MonoBehaviour
{


    Vector3[] vertics_unity;
    public Mesh mesh;
    public Material material;
    Vector2[] uv;
    int[] newTriangles;
    public RegionOfInterest roi;
    public int Interest_Score;

    VRPlayerCore core;
    ApplyToMesh mesh_content;



    [Header("Animation Data")]
    public int total_keyframe_length;
    public string from_keyframe_index;
    public string to_keyframe_index;
    public int current_frame;
    public int total_frames;
    RegionOfInterestAnimationData current_keyframe;
    RegionOfInterestAnimationData target_keyframe;

    // Use this for initialization
    void Start()
    {
        core = GameObject.Find("VRPlayerSystem").GetComponent<VRPlayerCore>();
        mesh_content = core.mesh_content;

    }

    // Update is called once per frame
    void Update()
    {

        if (mesh != null)
        {
            // will make the mesh appear in the scene at origin position
            Graphics.DrawMesh(mesh, this.transform.position, this.transform.rotation, material, 0);
        }

        transform.LookAt(GameObject.Find("Camera (eye)").transform);

        Interest_Score = roi.score;


        //update key frame positiion
        updateKeyframePosition();

    }

    public void initializeMesh(RegionOfInterest roi)
    {
        //set up roi
        this.roi = roi;

        //set position
        this.transform.position = roi.position.getUnityVector3();

        //set rotation
        this.transform.rotation = Quaternion.Euler(roi.rotation.getUnityVector3());



        //create mesh by detection shape type
        switch (roi.roi_detection_shape)
        {
            case RegionOfInterest.ROI_Shape.RECTANGLE:
                mesh = Resources.Load<Mesh>("SystemMeshes/SimpleMeshes/SimpleMesh_Rectangle");
                break;
            case RegionOfInterest.ROI_Shape.CIRCLE:
                mesh = Resources.Load<Mesh>("SystemMeshes/SimpleMeshes/SimpleMesh_Circle");
                break;
        }


        //set up mesh and colider on object
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;



        return;


        //abandoned method 09 Oct 2017
        ////draw mesh
        //Vector3[] vertics_unity = new Vector3[roi.vertics.Length];

        //for (int i = 0; i < roi.vertics.Length; i++)
        //{
        //    vertics_unity[i] = roi.vertics[i].getUnityVector3();
        //}


        //Vector2[] uv = new Vector2[4];
        //uv[0] = new Vector2(0, 0);
        //uv[1] = new Vector2(1, 0);
        //uv[2] = new Vector2(0, 1);
        //uv[3] = new Vector2(1, 1);

        //newTriangles = new int[6];
        //newTriangles[0] = 0;
        //newTriangles[1] = 2;
        //newTriangles[2] = 1;

        //newTriangles[3] = 2;
        //newTriangles[4] = 3;
        //newTriangles[5] = 1;


        //mesh = new Mesh();
        //GetComponent<MeshFilter>().mesh = mesh;
        //GetComponent<MeshCollider>().sharedMesh = mesh;


        //mesh.vertices = vertics_unity;
        //mesh.uv = uv;
        //mesh.triangles = newTriangles;

        //mesh.name = "Generated Mesh";
    }

    public void updateKeyframePosition()
    {
        //return if the player is not playing
        if (!core.current_node.currentShotNode.isReadyToPlay)
            return;

        //Link current frame of video
        current_frame = core.current_node.current_frame;

        //Link totatl frames of video
        total_frames = core.current_node.total_frames;

        //Link total "keyframe" length of array
        total_keyframe_length = roi.animation_data.Length;

        //Link current & next index index of key frame
        from_keyframe_index = roi.animation_keyframe_current_index+"";
        to_keyframe_index = roi.animation_keyframe_current_index+1+"";

        //End key frame, stop update
        if (roi.animation_keyframe_current_index >= roi.animation_data.Length - 1)
        {
            to_keyframe_index = "End of KeyFrame";
            return;
        }
            
     
        ////update keyframe
        current_keyframe = roi.animation_data[roi.animation_keyframe_current_index];
        target_keyframe = roi.animation_data[roi.animation_keyframe_current_index+1];

        //calculate direction vector
        Vector3 p0 = current_keyframe.position.getUnityVector3();
        Vector3 p1 = target_keyframe.position.getUnityVector3();
        Vector3 direction_vector = p1 - p0;

        //update position of this ROI
        Debug.Log((float)core.current_node.current_frame / core.current_node.total_frames);
        this.transform.position = p0 + (direction_vector * ((float)core.current_node.current_frame / target_keyframe.time_code));

        //if current frame reach target time_code, jump to next key frame
        if (target_keyframe.time_code == current_frame)
        {
            roi.animation_keyframe_current_index++;
        }

    }
}

using Babel.System.Data;
using System.Collections;
using System.Collections.Generic;
using RenderHeads.Media.AVProVideo;
using UnityEngine;

public class RegionOfInterestMesh : MonoBehaviour
{

    //
    //Vector3[] vertics_unity;
    //Vector2[] uv;
    //int[] newTriangles;

    public Mesh mesh;
    public Material material;

    public RegionOfInterest roi;
    public int Interest_Score;

    VRPlayerCore core;
    ApplyToMesh mesh_content;

    //camera
    [SerializeField]
    private Camera eye_cam;


    [Header("Animation Data")]
    public int total_keyframe_length;
    public string from_keyframe_index;
    public string to_keyframe_index;
    private int current_frame;
    private int target_frame;
    private int total_frames;
    RegionOfInterestAnimationData current_keyframe;
    RegionOfInterestAnimationData target_keyframe;


    public Transform look_at_transform;
    public float x;
    public float y;

    // Use this for initialization
    void Start()
    {


    }



    // Update is called once per frame
    void Update()
    {

        //if (mesh != null)
        //{
        //    // will make the mesh appear in the scene at origin position
        //    Graphics.DrawMesh(mesh, this.transform.position, this.transform.rotation, material, 0);
        //}


        //keep the mesh facing to camera

        lookAt(eye_cam.transform);

        //update interest scoring
        Interest_Score = roi.score;


        //return if the player is not playing
        if (core.current_node==null||
            core.current_node.currentShotNode==null||
            !core.current_node.currentShotNode.isReadyToPlay)
            return;

        //update keyframe
        updateKeyframe();

        //update position,rotation & scale tranformation from keyframes
        updateTransform();






    }

    public void lookAt(Transform t)
    {

        this.transform.LookAt(eye_cam.transform);




    }

    public void initializeMesh(RegionOfInterest roi)
    {
        //set up roi
        this.roi = roi;

        //set position
        this.transform.position = roi.position.getUnityVector3();

        //set rotation
        this.transform.rotation = Quaternion.Euler(roi.rotation.getUnityVector3());


        core = GameObject.Find("VRPlayerSystem").GetComponent<VRPlayerCore>();
        mesh_content = core.mesh_content;
        eye_cam = GameObject.Find("Camera (eye)").GetComponent<Camera>();



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

        //assign material based on type
        switch (roi.flag)
        {
            case RegionOfInterestFlag.Active:
                material = Resources.Load<Material>("Materials/ROI_Active");
                break;
            case RegionOfInterestFlag.Passive:
                material = Resources.Load<Material>("Materials/ROI_Passive");
                break;
        }

        //set up mesh and colider on object
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = material;
        GetComponent<Renderer>().enabled = true;
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

    private bool updateKeyframe()
    {
        if (roi.animation_data.Length <= 0)
            return false;

        //Link totatl frames of video
        total_frames = core.current_node.total_frames;

        //Link total "keyframe" length of array
        total_keyframe_length = roi.animation_data.Length;

        //Link current & next index index of key frame
        from_keyframe_index = roi.animation_keyframe_current_index + "";
        to_keyframe_index = roi.animation_keyframe_next_index + "";

        //set current keyframe
        current_keyframe = roi.animation_data[roi.animation_keyframe_current_index];

        //set target keyframe
        target_keyframe = roi.animation_data[roi.animation_keyframe_next_index];

        //Link current frame of video
        current_frame = core.current_node.current_frame;

        //Link frame of next keyframe
        target_frame = target_keyframe.time_code;



        //if current frame reach target time_code, jump to next key frame
        if (current_frame == target_frame)
        {
            ////update keyframe
            roi.animation_keyframe_current_index = roi.animation_keyframe_next_index;
            roi.animation_keyframe_next_index++;

            //End key frame, stop update
            if (roi.animation_keyframe_next_index >= roi.animation_data.Length)
            {
                to_keyframe_index = "End of KeyFrame";
                roi.animation_keyframe_next_index = roi.animation_keyframe_current_index;
            }
        }


        return true;

    }


    private void updateTransform()
    {
        if (current_keyframe == null || target_keyframe == null)
            return;

        Debug.Log("UPDATING TRANSFORM");

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
        this.GetComponent<MeshCollider>().transform.rotation = this.transform.rotation;




    }
    public void calLinearVector(Vector3 p0, Vector3 p1, int current_frame, int target_frame, out Vector3 calculated_vector)
    {
        calculated_vector = new Vector3();
        Vector3 direction_vector = p1 - p0;
        calculated_vector = direction_vector * ((float)current_frame / target_frame);
    }

}

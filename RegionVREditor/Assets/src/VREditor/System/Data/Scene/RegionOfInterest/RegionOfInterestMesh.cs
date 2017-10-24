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
        core = GameObject.Find("VRPlayerSystem").GetComponent<VRPlayerCore>();
        mesh_content = core.mesh_content;
        eye_cam = GameObject.Find("Camera (eye)").GetComponent<Camera>();

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
        this.transform.LookAt(eye_cam.transform);
        lookAt(eye_cam.transform);

        //update interest scoring
        Interest_Score = roi.score;

        //update position,scale and rotation from keyframes
        //return if the player is not playing
        if (!core.current_node.currentShotNode.isReadyToPlay)
            return;

        //update keyframe
        if (!updateKeyframe())
            return;

        //update positiion from key frame
        updatePosition();

        //update scale frome key frame
        updateScale();

        //update rotation frome key frame
        updateRotation();




    }

    public void lookAt(Transform t)
    {
        Vector3 dir = t.position - this.transform.position;

        float r = Vector3.Distance(this.transform.position, t.position);
        y = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        x = Mathf.Atan2(dir.z, dir.y) * Mathf.Rad2Deg;



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
        //Link totatl frames of video
        total_frames = core.current_node.total_frames;

        //Link total "keyframe" length of array
        total_keyframe_length = roi.animation_data.Length;

        //Link current & next index index of key frame
        from_keyframe_index = roi.animation_keyframe_current_index + "";
        to_keyframe_index = roi.animation_keyframe_current_index + 1 + "";

        //End key frame, stop update
        if (roi.animation_keyframe_current_index >= roi.animation_data.Length - 1)
        {
            to_keyframe_index = "End of KeyFrame";
            return false;
        }

        ////update keyframe
        current_keyframe = roi.animation_data[roi.animation_keyframe_current_index];
        target_keyframe = roi.animation_data[roi.animation_keyframe_current_index + 1];

        //Link current frame of video
        current_frame = core.current_node.current_frame;

        //Link frame of next keyframe
        target_frame = target_keyframe.time_code;

        //if current frame reach target time_code, jump to next key frame
        if (target_keyframe.time_code == current_frame)
        {
            roi.animation_keyframe_current_index++;
        }

        return true;

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
        this.transform.localScale = current_keyframe.scale.getUnityVector3() + incremental_vector;
    }

    private void updateRotation()
    {

        return;
        //create incremental vector
        Vector3 incremental_vector;

        //calculate incremental vector
        calLinearVector(current_keyframe.rotation.getUnityVector3(),
            target_keyframe.rotation.getUnityVector3(),
            current_frame,
            target_keyframe.time_code, out incremental_vector);

        //update position of this ROI
        this.transform.rotation = Quaternion.Euler(current_keyframe.scale.getUnityVector3() + incremental_vector);

    }
    public void calLinearVector(Vector3 p0, Vector3 p1, int current_frame, int target_frame, out Vector3 calculated_vector)
    {
        calculated_vector = new Vector3();
        Vector3 direction_vector = p1 - p0;
        calculated_vector = direction_vector * ((float)current_frame / target_frame);
    }

}

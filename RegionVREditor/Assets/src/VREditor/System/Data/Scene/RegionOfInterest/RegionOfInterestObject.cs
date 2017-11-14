using Babel.System.Data;
using System.Collections;
using System.Collections.Generic;
using RenderHeads.Media.AVProVideo;
using UnityEngine;

public class RegionOfInterestObject : KeyframedMonoBehaviour
{

    //
    //Vector3[] vertics_unity;
    //Vector2[] uv;
    //int[] newTriangles;

    public Mesh mesh;
    public Material material;

    public RegionOfInterest roi;
    public int Interest_Score;


    ApplyToMesh mesh_content;

    //camera
    [SerializeField]
    private Camera eye_cam;





    public Transform look_at_transform;
    public float x;
    public float y;


    //collision of gazing
    private bool entered;

    public void OnEntered()
    {
        if (entered)
            return;

        entered = true;
        core.addGazingLog(this, GazingAction.ENTERED);
    }

    public void OnExit()
    {
        entered = false;
        core.addGazingLog(this, GazingAction.LEFT);
    }

    // Use this for initialization
    void Start()
    {


    }



    // Update is called once per frame
    void Update()
    {
        //keep the mesh facing to camera
        lookAt(eye_cam.transform);
  
        //update interest scoring
        Interest_Score = roi.score;

        //update keyframe from super class
        base.updateAnimation();
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

        //reference animationdata from roi
        this.animation_data = roi.animation_data;

        //reference the vr player system
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




}

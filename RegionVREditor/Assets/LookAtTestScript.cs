using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTestScript : MonoBehaviour {


    [SerializeField]
    private Camera cam;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        this.transform.LookAt(cam.transform);

        Vector3 dir = cam.transform.position - this.transform.position;

        
        this.transform.Rotate(new Vector3(0,0,1), angle++);


	}

    float angle = 0;
}

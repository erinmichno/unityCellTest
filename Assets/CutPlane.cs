using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutPlane : MonoBehaviour {


    public Vector3 Normal { get { return transform.up; } }
    float d;

    Vector3 lastNormal;
    Vector3 lastPositon;
    Spheroid spheroid;
	// Use this for initialization
	void Start () {
        spheroid = FindObjectOfType<Spheroid>(); // only one in demo
		
	}
	
	// Update is called once per frame
	void Update () {
        //use position and normal to define the plane
        d = -(Vector3.Dot(Normal, transform.position));
        if(lastNormal != Normal || lastPositon != transform.position )
        {
            //recalulate
            //spheroid.UpdateVisibleCells();
        }
	}

    private void LateUpdate()
    {
        lastNormal = Normal;
        lastPositon = transform.position;
    }

    //Returns true if above the plane and false above the plane
    public bool TestCullWithPlane(Vector3 point, float radius)
    {
        float value = Vector3.Dot(point, Normal) + d;  // negative already in d
        if(Mathf.Abs(value) >= radius)
        {
            if(value >= 0) //it's above the plane so cull 
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //partially intersecting the plane (keep for now)
        return value >= 0;
    }
}

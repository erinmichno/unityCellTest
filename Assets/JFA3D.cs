using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct DataType
{
    Vector3 pos;
}


public class JFA3D : MonoBehaviour {

    const int seedCount = 32;
    DataType[] data = new DataType[seedCount];
    ComputeBuffer seedBuffer;
	// Use this for initialization
	void Start () {

        seedBuffer = new ComputeBuffer(data.Length, 3*sizeof(float));
        //init
        seedBuffer.SetData(data);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

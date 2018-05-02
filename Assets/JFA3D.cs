using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct DataType
{
   public Vector3 pos;
}


public class JFA3D : MonoBehaviour {

    const int seedCount = 32;
    DataType[] data = new DataType[seedCount];
    ComputeBuffer seedBuffer;

    RenderTexture rt;
    int res = 128;

    // Use this for initialization
    void Start () {

        seedBuffer = new ComputeBuffer(data.Length, 3*sizeof(float));
        //init
        for(int i = 0; i < data.Length; ++i)
        {
            data[i].pos = Random.insideUnitSphere * (res / 2);
        }

        seedBuffer.SetData(data);



        rt = new RenderTexture(res, res, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
        rt.wrapMode = TextureWrapMode.Clamp;
        rt.filterMode = FilterMode.Bilinear;//may need to be point
        rt.enableRandomWrite = true;
        rt.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
        rt.volumeDepth = res;
        rt.useMipMap = false;
        rt.Create();

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

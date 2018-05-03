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
    Vector3[] seedVelocity = new Vector3[seedCount];
    RenderTexture rt;
    int res = 128;
    public ComputeShader jfaComputeShader;
    Material outputRayMarchingMaterial;

    // Use this for initialization
    void Start () {

        seedBuffer = new ComputeBuffer(data.Length, 3*sizeof(float));
        //init
        for(int i = 0; i < data.Length; ++i)
        {
            data[i].pos = Random.insideUnitSphere * (res / 2);
            seedVelocity[i] = Random.insideUnitSphere * 0.1f;
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

        outputRayMarchingMaterial = GetComponent<MeshRenderer>().sharedMaterial;
        outputRayMarchingMaterial.SetTexture("_Volume", rt);


        jfaComputeShader.SetInt("SeedCount", seedCount);
        jfaComputeShader.SetTexture(0, "Result", rt);
        jfaComputeShader.SetTexture(1, "Result", rt);
        

        jfaComputeShader.SetBuffer(0, "SeedBuffer", seedBuffer);
        jfaComputeShader.SetBuffer(1, "SeedBuffer", seedBuffer);

    }
	
	// Update is called once per frame
	void Update () {
        jfaComputeShader.Dispatch(0, res / 8, res / 8, res/8); //just a test
    }
}

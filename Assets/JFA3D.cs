using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct DataType
{
    public float x;
    public float y;
    public float z;
}


public class JFA3D : MonoBehaviour
{

    const int seedCount = 64;
    float[] data = new float[seedCount*3];
    ComputeBuffer seedBuffer;
    Vector3[] seedVelocity = new Vector3[seedCount];
    RenderTexture rt;
    int res = 128;
    public ComputeShader jfaComputeShader;
    Material outputRayMarchingMaterial;

    // Use this for initialization
    void Start()
    {
        UnityEngine.Random.InitState(1);//to keep starting points the same init

        seedBuffer = new ComputeBuffer(seedCount, 3 * sizeof(float));
        //init
        for (int i = 0; i < seedCount * 3; i += 3)
        {
            data[i + 0] = Random.value * (res - 5) + 2;
            data[i + 1] = Random.value * (res - 5) + 2;
            data[i + 2] = 0;// Random.value * (res - 5) + 2;
            seedVelocity[i / 3] = Random.insideUnitSphere * 0.1f;
        }

        seedBuffer.SetData(data);
       


        rt = new RenderTexture(res, res, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        rt.wrapMode = TextureWrapMode.Clamp;
        rt.filterMode = FilterMode.Bilinear;//may need to be point
        rt.enableRandomWrite = true;
        rt.dimension = UnityEngine.Rendering.TextureDimension.Tex3D;
        rt.volumeDepth = res;
       // rt.useMipMap = true;
        rt.Create();

        outputRayMarchingMaterial = GetComponent<MeshRenderer>().sharedMaterial;
        outputRayMarchingMaterial.SetTexture("_Volume", rt);
        outputRayMarchingMaterial.SetInt("_SeedNumber", seedCount);

        jfaComputeShader.SetInt("SeedCount", seedCount);
        jfaComputeShader.SetTexture(0, "Result", rt);
        jfaComputeShader.SetTexture(1, "Result", rt);
        jfaComputeShader.SetTexture(2, "Result", rt);


        jfaComputeShader.SetBuffer(0, "SeedBuffer", seedBuffer);
        jfaComputeShader.SetBuffer(1, "SeedBuffer", seedBuffer);

        jfaComputeShader.Dispatch(2, res / 8, res / 8, res / 8); //clear 
        jfaComputeShader.Dispatch(1, seedCount / 8, 1, 1); //seed 

    }

    // Update is called once per frame
    void Update()
    {
        jfaComputeShader.Dispatch(1, seedCount / 8, 1, 1); //seed 
        jfaComputeShader.Dispatch(0, res / 8, res / 8, res/8); //just a test
        

    }
   

    private float Wrap0Res(float d)
    {
        float amt = res - 5;
        if(d < 0)
        {
            d = d + amt;
        }
        if(d >= amt)
        {
            d= d - amt;
        }
        return d ;
    }


    private float Wrap(float d,  float max)
    {
        if(d < 0)
        {
            d += max;
        }
        if(d >= max)
        {
            d -= max;
        }
        return d;
    }
    private void LateUpdate()
    {
        for (int i = 0; i < seedCount * 3; i += 3)
        {
            data[i] = Wrap0Res(data[i] + seedVelocity[i / 3].x);
            data[i + 1] = Wrap0Res(data[i + 1] + seedVelocity[i / 3].y);
            data[i + 2] = Wrap0Res(data[i + 2] + seedVelocity[i / 3].z);
        }
        seedBuffer.SetData(data);

    }
    private void OnDestroy()
    {
        seedBuffer.Release();
    }
}

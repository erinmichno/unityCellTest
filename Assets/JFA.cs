using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JFA : MonoBehaviour {


    RenderTexture rt;
   public Material consumeMaterial;
   public  ComputeShader jfaComputeShader;
   const int res = 128;
    ComputeBuffer seedBuffer;
    // Use this for initialization
    void Start () {

        seedBuffer = new ComputeBuffer(res * res, sizeof(float)*4, ComputeBufferType.Default); //could make low res and expand
        float[] seeds = new float[res * res*4 ];
        int seedCount = 24;
        for(int i = 0; i < res; ++i)
        {
            for(int j = 0; j < res; ++j)
            {
                seeds[(i + j * res)*4 + 0] = 0;
                seeds[(i + j * res) * 4 + 1] = 0;
                seeds[(i + j * res) * 4 + 2] = 0;
                seeds[(i + j * res) * 4 + 3] = 0;
                if (seedCount > 0 && Random.value > 0.9994)
                {
                    seeds[(i + j * res) * 4 + 0] = 1;
                    seeds[(i + j * res) * 4 + 1] = 0;
                    seeds[(i + j * res) * 4 + 2] = 0;
                    seeds[(i + j * res) * 4 + 3] = 1;
                    --seedCount;
                }
            }
        }
        seedBuffer.SetData(seeds);
     

        rt = new RenderTexture(res, res, 1, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        rt.wrapMode = TextureWrapMode.Clamp;
        rt.filterMode = FilterMode.Bilinear;//may need to be point
        rt.enableRandomWrite = true;
        rt.Create();

        jfaComputeShader.SetInt("_width", res);
        jfaComputeShader.SetBuffer(0, "SeedBuffer", seedBuffer);
        jfaComputeShader.SetTexture(0, "Result", rt);
        jfaComputeShader.SetBuffer(1, "SeedBuffer", seedBuffer);
        jfaComputeShader.SetTexture(1, "Result", rt);

        consumeMaterial.mainTexture = rt;

        jfaComputeShader.Dispatch(1, res / 8, res / 8, 1);
       
    }
	
	// Update is called once per frame
	void Update () {
        jfaComputeShader.Dispatch(0, res / 8, res / 8, 1); //just a test
    }

    private void OnDestroy()
    {
      
        seedBuffer.Release();
    }
}

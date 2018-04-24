using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JFA : MonoBehaviour {


    RenderTexture rt;
    RenderTexture startRT;
   public Material consumeMaterial;
   public  ComputeShader jfaComputeShader;
   const int res = 128;
    const int seedCount = 32; // /8 
    ComputeBuffer seedBuffer;
    float[] seeds = new float[seedCount * 2];
    Vector2[] seedVelocity = new Vector2[seedCount];
    // Use this for initialization
    void Start () {
      
        seedBuffer = new ComputeBuffer(seedCount, sizeof(float)*2, ComputeBufferType.Default); //could make low res and expand
        
        for (int i = 0; i < seedCount*2; i += 2)
        {
            seeds[i + 0] = Random.Range(0, res);
            seeds[i + 1] = Random.Range(0, res);
           
            
        }
        for(int i = 0; i < seedCount; ++i)
        {
            seedVelocity[i] = Random.insideUnitCircle * 0.1f;
        }
        seedBuffer.SetData(seeds);
     

        rt = new RenderTexture(res, res, 1, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
        rt.wrapMode = TextureWrapMode.Clamp;
        rt.filterMode = FilterMode.Bilinear;//may need to be point
        rt.enableRandomWrite = true;
        rt.Create();

        startRT = new RenderTexture(res, res, 1, RenderTextureFormat.ARGBHalf);
        startRT.wrapMode = TextureWrapMode.Clamp;
        startRT.filterMode = FilterMode.Bilinear;
        startRT.enableRandomWrite = true;
        startRT.Create();



        jfaComputeShader.SetInt("_width", res);
       
        jfaComputeShader.SetTexture(0, "Result", rt);
        jfaComputeShader.SetTexture(1, "Result", rt);
        jfaComputeShader.SetTexture(2, "Result", rt);

      
        jfaComputeShader.SetBuffer(1, "SeedBuffer", seedBuffer);


        consumeMaterial.mainTexture = rt;

      
       
    }
	
	// Update is called once per frame
	void Update () {
        jfaComputeShader.Dispatch(2, res / 8, res / 8, 1); //clear
        jfaComputeShader.Dispatch(1, seedCount / 8, 1, 1); //seed 
        jfaComputeShader.Dispatch(0, res / 8, res / 8, 1); //just a test
    }

    private void LateUpdate()
    {
        for (int i = 0; i < seedCount; i += 2)
        {
            seeds[i + 0] += seedVelocity[i + 0].x;
            seeds[i + 1] += seedVelocity[i + 1].y;
            seeds[i + 0] = seeds[i + 0] % res;
            seeds[i + 1] = seeds[i + 1] % res;
        }
        seedBuffer.SetData(seeds);
    }



    private void OnDestroy()
    {
      
        seedBuffer.Release();
    }
}

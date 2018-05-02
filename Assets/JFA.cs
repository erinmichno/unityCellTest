using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JFA : MonoBehaviour {


    RenderTexture rt;
    RenderTexture startRT;
  
   public  ComputeShader jfaComputeShader;
   const int res = 128;
    const int seedCount = 32; // /8 
    ComputeBuffer seedBuffer;
    ComputeBuffer secondBuffer;
    float[] seeds = new float[seedCount * 2];
    float[] seconds = new float[seedCount * 2];
    Vector2[] seedVelocity = new Vector2[seedCount];
    // Use this for initialization

     Texture2D pallete;
    Material outputMaterial;

    void PopulateSecondSeedData()
    {
        for (int i = 0; i < seedCount * 2; i += 2)
        {
            seconds[i + 0] = ((i / 2) + 1) % seedCount; //int division
            Vector2 featurePt = new Vector2(seeds[i], seeds[i + 1]);
            Vector2 secondPt = new Vector2(seeds[(int)seconds[i + 0] * 2], seeds[(int)seconds[i + 0] * 2 + 1]);
            seconds[i + 1] = (secondPt - featurePt).magnitude;//dist
        }
    }
    void Start () {
      
        seedBuffer = new ComputeBuffer(seedCount, sizeof(float)*2, ComputeBufferType.Default); //could make low res and expand
        secondBuffer = new ComputeBuffer(seedCount, sizeof(float) * 2, ComputeBufferType.Default);


        for (int i = 0; i < seedCount*2; i += 2)
        {
            seeds[i + 0] = Random.Range(0, res);
            seeds[i + 1] = Random.Range(0, res);

           
        }
        PopulateSecondSeedData();
        secondBuffer.SetData(seconds);

            for (int i = 0; i < seedCount; ++i)
        {
            seedVelocity[i] = Random.insideUnitCircle * 0.1f;
        }
        seedBuffer.SetData(seeds);
        pallete = new Texture2D(seedCount, 1);
        for(int i = 0; i < seedCount; ++i)
        {
            Vector3 r = Random.insideUnitSphere;
            Color c = new Color(r.x, r.y, r.z);
            pallete.SetPixel(i, 0, c );
            pallete.SetPixel(i, 1, c);
        }
        pallete.Apply();
        outputMaterial = GetComponent<MeshRenderer>().sharedMaterial;
        //set
        outputMaterial.SetTexture("_Pallete", pallete);
       
        rt = new RenderTexture(res, res, 1, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        rt.wrapMode = TextureWrapMode.Clamp;
        rt.filterMode = FilterMode.Bilinear;//may need to be point
        rt.enableRandomWrite = true;
        rt.Create();


        outputMaterial.SetTexture("_MainTex", rt);
        outputMaterial.SetFloat("_Width", seedCount);


        jfaComputeShader.SetInt("SeedCount", seedCount);
       
        jfaComputeShader.SetTexture(0, "Result", rt);
        jfaComputeShader.SetTexture(1, "Result", rt);
        jfaComputeShader.SetTexture(2, "Result", rt);

        jfaComputeShader.SetBuffer(0, "SeedBuffer", seedBuffer);
        jfaComputeShader.SetBuffer(1, "SeedBuffer", seedBuffer);

       
        jfaComputeShader.SetBuffer(1, "SecondBuffer", secondBuffer);



        jfaComputeShader.Dispatch(2, res / 8, res / 8, 1); //clear
        jfaComputeShader.Dispatch(1, seedCount/8 , 1, 1); //seed 
    }
	
	// Update is called once per frame
	void Update () {

       
      
          jfaComputeShader.Dispatch(0, res / 8, res / 8, 1); //just a test
    }

    private void LateUpdate()
    {
        for (int i = 0; i < seedCount; i += 2)
        {
            seeds[i + 0] += seedVelocity[i ].x;
            seeds[i + 1] += seedVelocity[i ].y;
            seeds[i + 0] = seeds[i + 0] % res;
            seeds[i + 1] = seeds[i + 1] % res;
        }
        //seeds[seedCount - 2] = 999; //last seed is off map to help define a -1 / unknown state
        //seeds[seedCount - 1] = 999;

       // PopulateSecondSeedData();
       // secondBuffer.SetData(seconds);

        seedBuffer.SetData(seeds);
    }

    private void OnPostRender()
    {
       // jfaComputeShader.Dispatch(2, res / 8, res / 8, 1); //clear
        jfaComputeShader.Dispatch(1, seedCount / 8, 1, 1); //seed 
    }

    private void OnDestroy()
    {
        seedBuffer.Release();
        secondBuffer.Release();
    }
}

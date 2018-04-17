using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellScript : MonoBehaviour {

    //agent params
    public Color color;  
    public float O2Level;
    public float age = 0;
    public int GenerationLevel { get { return generationLevel; } }
    public int anInterestingParameter;

    private Rigidbody rb;
    private float DeathAge = 100;
    private float SplitAge = 50;
    private float ageRate = 10.0f;
    
    
    
    private int generationLevel = 0;
    //end agent params (position in transform)
    private Material currentMaterial;
    private Spheroid parentSpheroid;
    List<GameObject> SubcellList = new List<GameObject>();

    bool aboveISO = false;

    public MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Use this for initialization
    void Start () {
        currentMaterial = GetComponent<MeshRenderer>().material;
        parentSpheroid = FindObjectOfType<Spheroid>(); //only showing one at the moment
        Reset();
        age = UnityEngine.Random.Range(1, 100);
        anInterestingParameter = (transform.position - new Vector3(4, 6, 2)).sqrMagnitude < 25 ? 1 : 0;


    }

    public void Reset()
    {
        rb = GetComponent<Rigidbody>();
       
        age = 0;
        DeathAge = UnityEngine.Random.Range(50, 500);
        SplitAge = UnityEngine.Random.Range(25, 600);
    }

    public void SetGenerationLevel(int GenLevel)
    {
        generationLevel = GenLevel;
    }

    void TurnOnOffChildren(bool on)
    {
        for (int index = 0; index < transform.childCount; ++index)
        {
           Transform child =  transform.GetChild(index);
            child.gameObject.SetActive(on);
        }
    }

    bool IsoValue(float val)
    {
       if(val < -0.6f)
        {
            return true;
        }
        return false;
        //return aboveISO;
    }
	
	// Update is called once per frame
	void Update () {
        float val = Noise.Simplex3D(transform.position, 2.0f).value; //frequency = zoom ish
        float val01 = val * 0.5f + 0.5f;
        aboveISO = IsoValue(val);
        Color grey = Color.white * val01; //+ Color.red*(1 - val01);
        //grey.a = aboveISO ? 1.0f : 0.1f;// 0.1f;
        //TurnOnOffChildren(aboveISO);
        //currentMaterial.SetColor("_Color", grey);// aboveISO ? Color.cyan : grey);
        Vector4 ageVec = new Vector4(generationLevel, age, parentSpheroid.minGeneration, parentSpheroid.maxGeneration);
        currentMaterial.SetVector("_GenAgeMinMax", ageVec);
        currentMaterial.SetInt("_UseAgeVis", parentSpheroid.ShowGeneration.isOn ? 1 : 0);
        O2Level = TestO2ThresholdPositionBased(parentSpheroid);

        Color cellColor = parentSpheroid.showO2.isOn ? (O2Level > parentSpheroid.O2Threshold) ? Color.red : Color.blue : grey;



        cellColor = anInterestingParameter > 0 && parentSpheroid.showParam1.isOn ? Color.green : cellColor;
        bool cullIt = (parentSpheroid.CutWithPlanes(transform.position, transform.lossyScale.x)) && !((O2Level <= parentSpheroid.O2Threshold) && !parentSpheroid.cutAllParams.isOn);
        //cellColor.a = cullIt ? 0.1f : 1.0f;
        CullItems(cullIt, parentSpheroid.transparentCull.isOn);
      
        
        

    
        
        cellColor.a = Mathf.Min(currentMaterial.color.a, cellColor.a);
        currentMaterial.color = cellColor;

         age += Time.deltaTime * ageRate;

        if(age > DeathAge)
        {
            parentSpheroid.KillCell(this);
        }
        if(age > SplitAge)
        {
            parentSpheroid.SplitCell(this);
        }
        UpdateScale();
        Vector3 noise = UnityEngine.Random.insideUnitSphere;
        rb.AddForce((Vector3.zero - transform.position).normalized*5.5f + noise); // should be spheroid center not 0 but placeholder
	}

    internal void SetScale(float v)
    {
        transform.localScale = new Vector3(v,v,v);
    }

    void UpdateScale()
    {
        if(transform.localScale.x < 1)
        {
            float v = Mathf.Min(1.0f, transform.localScale.x + Time.deltaTime*0.5f);
            SetScale(v);
        }
    }


    float TestO2ThresholdPositionBased(Spheroid parent)
    {
      return ((transform.position - parentSpheroid.transform.position).sqrMagnitude);
    }

    void CullItems(bool cullit, bool doTransparentOption = false)
    {
        MeshRenderer[] mrs = GetComponentsInChildren<MeshRenderer>();
        if (doTransparentOption)
        {
            foreach (MeshRenderer mr in mrs)
            {
                mr.enabled = true;
                Color c = mr.material.color;
                c.a = cullit ? 0.1f : 1.0f;
                mr.material.color = c;
            }
        }
        else
        {
            foreach (MeshRenderer mr in mrs)
            {
                mr.enabled = !cullit;
                Color c = mr.material.color;
                c.a =  1.0f;
                mr.material.color = c;
            }
        }
    }
}

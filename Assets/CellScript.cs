using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellScript : MonoBehaviour {

    //agent params
    public Color color;  
    public int O2Level;
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



	// Use this for initialization
	void Start () {
        currentMaterial = GetComponent<MeshRenderer>().material;
        parentSpheroid = FindObjectOfType<Spheroid>(); //only showing one at the moment
        Reset();
        age = UnityEngine.Random.Range(1, 100);

    }

    public void Reset()
    {
        rb = GetComponent<Rigidbody>();
       
        age = 0;
        DeathAge = UnityEngine.Random.Range(50, 500);
        SplitAge = UnityEngine.Random.Range(25, 700);
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
        //float val = Noise.Simplex3D(transform.position, 2.0f).value; //frequency = zoom ish
        //float val01 = val * 0.5f + 0.5f;
        //aboveISO = IsoValue(val);
        //Color grey = Color.grey*val01 + Color.red*(1 - val01);
        //grey.a = aboveISO ? 1.0f : 0.1f;// 0.1f;
        //TurnOnOffChildren(aboveISO);
        //currentMaterial.SetColor("_Color", grey);// aboveISO ? Color.cyan : grey);

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
}

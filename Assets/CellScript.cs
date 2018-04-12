using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellScript : MonoBehaviour {

    //agent params
    private Color color;
    private CellScript parent;
    private int O2Level;
    //end agent params (position in transform)
    private Material currentMaterial;

    List<GameObject> SubcellList = new List<GameObject>();



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

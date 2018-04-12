using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class Spheroid : MonoBehaviour {

    public CellScript cellTemplate;
    public int StartingCellNumber = 16;
    List<CellScript> currentCells = new List<CellScript>();
    // Use this for initialization
    float currentRad = 1.01f;
	void Start () {
        currentCells.Clear();
    }
	
	// Update is called once per frame
	void Update () {

        if(currentCells.Count == 0)
        {
            GenerateSomeCells();
        }
		
	}

    void GenerateSomeCells()
    {
        if(cellTemplate == null)
        {
            return;
        }
        for (int i = 0; i < StartingCellNumber; ++i)
        {
            Vector3 pos = cellTemplate.transform.position;
            Vector3 offset = Random.onUnitSphere * currentRad;
            CellScript cs = GameObject.Instantiate<CellScript>(cellTemplate, pos + offset, Quaternion.identity, this.transform);
            currentCells.Add(cs);

        }
    }
}

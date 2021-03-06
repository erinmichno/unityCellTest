﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Spheroid : MonoBehaviour {
    
    const int CutTypeStraight = 0;
    const int CutTypeO2Based = 1;
    const int CutTypeDataPt1 = 1 << 1;
    const int CutTypeDataPt2 = 1 << 2;

    public int planeCutType = CutTypeStraight;

    public CellScript cellTemplate;
    private int numCells;
    List<CellScript> currentCells = new List<CellScript>();
    // Use this for initialization
    float currentRad = 1.01f;
    float startingRad = 1.01f;
    public CutPlane cutPlane;
    public CutPlane cutPlane2;
    public float O2Threshold = 20;

    private bool cutWithPlane1 = false;
    private bool cutWithPlane2 = false;
    public UnityEngine.UI.Toggle togglePlane1;
    public UnityEngine.UI.Toggle togglePlane2;
    public UnityEngine.UI.Toggle cutAllParams;
    public UnityEngine.UI.Toggle transparentCull;
    public UnityEngine.UI.Toggle showParam1;
    public UnityEngine.UI.Toggle showO2;
    public UnityEngine.UI.Toggle ShowGeneration;
    public UnityEngine.UI.Toggle BubbleCull;
    public int minGeneration = 0;
    public int maxGeneration = 20;
    public float BubbleRadius = 4.0f;
    [Range(0.0f, 1.0f)]
    public float TranparentCullColor = 0.2f;

    void Start () {

        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);
        numCells = 64;
        startingRad = currentRad;
        currentCells.Clear();
    }
	
	// Update is called once per frame
	void Update () {

        if(currentCells.Count == 0 || transform.childCount == 0)
        {
            // GenerateSomeCells();
            // Generate72Cells();
            GenerateFibSphere(13, 1, true);
            GenerateFibSphere(50, 2);
            GenerateFibSphere(160, 3);
            GenerateFibSphere(250, 4);
            GenerateFibSphere(380, 5);
            GenerateFibSphere(450, 6);
          
        }
        //do something about children...

       // UpdateVisibleCells();


    }

    void GenerateSomeCells()
    {
        currentCells.Clear();
       CellScript[] cells =  transform.GetComponentsInChildren<CellScript>();
        foreach(CellScript c in cells)
        {
            GameObject.Destroy(c.gameObject);
        }

        if (cellTemplate == null)
        {
            return;
        }
        currentRad = startingRad;
        for (int i = 0; i < numCells; ++i)
        {
            if (i > numCells / 4)
            {
                currentRad = startingRad * 1.5f;
            }
         
            Vector3 pos = cellTemplate.transform.position;
            Vector3 offset = Random.onUnitSphere * currentRad;
            CellScript cs = GameObject.Instantiate<CellScript>(cellTemplate, pos + offset, Quaternion.identity, this.transform);
            currentCells.Add(cs);

        }
    }

    void Generate72Cells()
    {
        currentCells.Clear();
        CellScript[] cells = transform.GetComponentsInChildren<CellScript>();
        foreach (CellScript c in cells)
        {
            GameObject.Destroy(c.gameObject);
        }
        Vector3[] cellposUnitSphere = Get72VecPlace();
        foreach(Vector3 p in cellposUnitSphere)
        {
           Vector3 pos  = p  / 0.4f;
            CellScript cs = GameObject.Instantiate<CellScript>(cellTemplate, pos, Quaternion.identity, this.transform);
            currentCells.Add(cs);
        }
    }
    void GenerateFibSphere(int numberOfSphere, float distanceOut, bool clear = false)
    {
        if (clear)
        {
            currentCells.Clear();
            CellScript[] cells = transform.GetComponentsInChildren<CellScript>();
            foreach (CellScript c in cells)
            {
                GameObject.Destroy(c.gameObject);
            }
        }
        Vector3[] cellposUnitSphere = fibonacciSpherePlacement(numberOfSphere);
        
        foreach (Vector3 p in cellposUnitSphere)
        {
            Vector3 pos = p * distanceOut;
            CellScript cs = GameObject.Instantiate<CellScript>(cellTemplate, pos, Quaternion.identity, this.transform);
            currentCells.Add(cs);
        }

    }

    Vector3[] fibonacciSpherePlacement(int numSamples)
    {
        Vector3[] returnval = new Vector3[numSamples];

        float offset = 2.0f / numSamples;
        float increment = Mathf.PI * (3.0f - Mathf.Sqrt(5));
        for(int i = 0; i < numSamples; ++i)
        {
            Vector3 vec = Vector3.zero;
            vec.y = ((i * offset) - 1) + (offset / 2.0f);
            float r = Mathf.Sqrt(1 - vec.y * vec.y);
            float phi = ((i + 1) % numSamples) * increment;
            vec.x = Mathf.Cos(phi) * r;
            vec.z = Mathf.Sin(phi) * r;
            returnval[i] = vec;
        }
        return returnval;
    }


    Vector3[] Get72VecPlace() 
    {
        //assuming radius = 0.196078624
        return new Vector3[] { new Vector3(0.365142892803682839000000000000f, 0.368988570632518387000000000000f, -0.613846624222014858000000000000f),
    new Vector3(0.050320587481783155000000000000f, -0.531136876629041299000000000000f, - 0.601374289611330215000000000000f),
    new Vector3(0.712648540781815831000000000000f, -0.332161893929239838000000000000f, - 0.167601052736207706000000000000f),
    new Vector3(-0.311366006101928217000000000000f, -0.592947909590187061000000000000f, - 0.444694906864035178000000000000f),
    new Vector3(-0.691995879685380788000000000000f, -0.404442600547515518000000000000f, - 0.062108485607051851200000000000f),
    new Vector3(-0.252045885532391933000000000000f, -0.302164614519069175000000000000f, - 0.701041365649105464000000000000f),
     new Vector3(0.219733155253111462000000000000f, -0.131470599487650941000000000000f, - 0.393287546668305787000000000000f),
    new Vector3(-0.145715460803957891000000000000f, 0.092770681526746168200000000000f, - 0.785143416229040381000000000000f),
      new Vector3(0.035575387080422474200000000000f, -0.399640337658212053000000000000f, 0.164753931293529304000000000000f),
     new Vector3(0.129162232729993498000000000000f, -0.186578513127819534000000000000f, - 0.771229636005407060000000000000f),
     new Vector3(0.505195478444795509000000000000f, -0.379546822212735280000000000000f, - 0.497002331335563263000000000000f),
   new Vector3(-0.382604723388845436000000000000f, 0.647551197972866310000000000000f, - 0.283867311717188597000000000000f),
  new Vector3(-0.070578117058970490900000000000f, 0.609870328681016649000000000000f, - 0.519005289509013923000000000000f),
  new Vector3(-0.012098914965412351300000000000f, 0.037661903340762081800000000000f, - 0.041888612602723057600000000000f),
  new Vector3(0.145346935034737584000000000000f, 0.345030095067422160000000000000f, - 0.711419763736948263000000000000f),
  new Vector3(-0.168359283737344351000000000000f, 0.319222543882309584000000000000f, - 0.274567127275713307000000000000f),
  new Vector3(0.299618502176972334000000000000f, 0.634696162837079969000000000000f, - 0.392019275977928128000000000000f),
  new Vector3(-0.595234746200556497000000000000f, -0.322606603273001280000000000000f, - 0.433293307210191769000000000000f),
  new Vector3(0.530908624322481648000000000000f, -0.553620326937620644000000000000f, 0.231082178251523412000000000000f),
  new Vector3(0.249713502778070834000000000000f, -0.758716533773899138000000000000f, 0.047967917469968236700000000000f),
  new Vector3(-0.728833947087550449000000000000f, -0.201444546911113520000000000000f, 0.272966575924076471000000000000f),
  new Vector3(-0.057793530017871998200000000000f, -0.774647338259847107000000000000f, - 0.207053105622638400000000000000f),
  new Vector3(0.328588077967749082000000000000f, -0.358777599581546869000000000000f, - 0.092838811480612973300000000000f),
  new Vector3(0.298865303729057619000000000000f, -0.668838807750261721000000000000f, - 0.331094788241849958000000000000f),
  new Vector3(-0.162050291278787451000000000000f, -0.045497863808921498700000000000f, - 0.418534234574554465000000000000f),
  new Vector3(-0.788194322344701770000000000000f, -0.035586071412791908700000000000f, - 0.154184697918627378000000000000f),
  new Vector3(0.417701581331646588000000000000f, 0.064224556812114044800000000000f, - 0.683878771381243866000000000000f),
  new Vector3(0.687742441536788429000000000000f, -0.029133922523426700900000000000f, - 0.415272291507955638000000000000f),
  new Vector3(-0.290351777570426128000000000000f, 0.744861912476579180000000000000f, 0.084653145421028572200000000000f),
  new Vector3(-0.659750626677549246000000000000f, 0.258255471283862226000000000000f, - 0.379898397356358719000000000000f),
  new Vector3(0.213973135885475563000000000000f, 0.256282534869193501000000000000f, - 0.334964004537836701000000000000f),
  new Vector3(0.583478202653497258000000000000f, 0.353457902429232973000000000000f, - 0.424854673447704056000000000000f),
  new Vector3(0.077763154490342767400000000000f, 0.441157496084452483000000000000f, 0.005709470247917345850000000000f),
  new Vector3(0.008054941636219083850000000000f, 0.785584665967952067000000000000f, - 0.170186827086959341000000000000f),
  new Vector3(0.378699696204070901000000000000f, 0.708975329594596415000000000000f, - 0.015169070234018608100000000000f),
  new Vector3(-0.416569831202107055000000000000f, -0.683154469346673676000000000000f, - 0.077840384768356807300000000000f),
  new Vector3(-0.120914969257860674000000000000f, -0.774126981802794822000000000000f, 0.179990459952169113000000000000f),
  new Vector3(0.155090208882665004000000000000f, -0.658932420605271796000000000000f, 0.433641132603371171000000000000f),
  new Vector3(-0.531255585854838341000000000000f, -0.540167282363147816000000000000f, 0.268842683808440841000000000000f),
  new Vector3(-0.495114731721955803000000000000f, -0.000501985404837169470000000000f, - 0.633363298987779877000000000000f),
  new Vector3(0.302684019815686489000000000000f, 0.217284523415259140000000000000f, 0.236137473728342095000000000000f),
  new Vector3(0.799574818136708831000000000000f, 0.065667548163257413700000000000f, - 0.051550544948826417000000000000f),
  new Vector3(-0.404890396792436602000000000000f, 0.041208460151676042100000000000f, - 0.123071879273109322000000000000f),
  new Vector3(-0.042495565512685018500000000000f, -0.384902073866436267000000000000f, - 0.248302411500691794000000000000f),
  new Vector3(-0.222582632310256212000000000000f, -0.582903280286867775000000000000f, 0.506922396588679769000000000000f),
  new Vector3(0.411256417818097075000000000000f, 0.024532265211204402200000000000f, - 0.087661704774446083700000000000f),
  new Vector3(-0.647333022265909719000000000000f, 0.473804202334904334000000000000f, - 0.052527264697945233400000000000f),
  new Vector3(-0.303402480572859123000000000000f, 0.352991744455802725000000000000f, 0.092052953002371346000000000000f),
  new Vector3(0.710755609189086202000000000000f, 0.242123882758043002000000000000f, 0.287214307455563089000000000000f),
  new Vector3(0.478205120125143102000000000000f, 0.554782104504067730000000000000f, 0.331400448154025140000000000000f),
  new Vector3(0.153732047745482742000000000000f, 0.564433385955025124000000000000f, 0.551426322728036489000000000000f),
  new Vector3(-0.764570155727782419000000000000f, 0.176541672736697464000000000000f, 0.174800146449135496000000000000f),
  new Vector3(0.670803335714189486000000000000f, 0.439435527895015832000000000000f, - 0.050534385456473394900000000000f),
  new Vector3(-0.211918952578331599000000000000f, -0.066073340131686753000000000000f, 0.759763673185611887000000000000f),
  new Vector3(0.043927044444492260900000000000f, -0.377548643646880511000000000000f, 0.708390440309138003000000000000f),
  new Vector3(0.356916449527891588000000000000f, -0.170850378702308875000000000000f, 0.250189131618210048000000000000f),
  new Vector3(-0.477978487555439424000000000000f, -0.289701924927943055000000000000f, 0.570562556602601245000000000000f),
  new Vector3(-0.319212312977379509000000000000f, -0.315046183675845637000000000000f, 0.020677154692941113900000000000f),
  new Vector3(0.280403487924036632000000000000f, -0.067546462642914417700000000000f, 0.750400518908086456000000000000f),
  new Vector3(0.743848076918517842000000000000f, -0.223150775297313003000000000000f, 0.207806023081886559000000000000f),
 new Vector3(-0.607551005240175912000000000000f, 0.077248823091312326900000000000f, 0.520772477978292847000000000000f),
  new Vector3(0.410032038593905523000000000000f, -0.393332657946550090000000000000f, 0.568729045164089064000000000000f),
  new Vector3(0.049833948602119974800000000000f, 0.249432517439114904000000000000f, 0.762620202288114002000000000000f),
  new Vector3(0.603471764809066169000000000000f, -0.047366913107012992300000000000f, 0.529025315150935316000000000000f),
  new Vector3(-0.556722798398451624000000000000f, 0.439223902343028882000000000000f, 0.378723736701544245000000000000f),
  new Vector3(-0.327200430203272996000000000000f, -0.003457538356428875150000000000f, 0.258708850509407062000000000000f),
  new Vector3(0.011984035030897078800000000000f, -0.107616595928592324000000000000f, 0.425718120503987052000000000000f),
  new Vector3(0.416210507152887321000000000000f, 0.283007197831244506000000000000f, 0.626869451991908355000000000000f),
  new Vector3(0.074856974931970660100000000000f, 0.767967891835186878000000000000f, 0.225635385084888923000000000000f),
  new Vector3(-0.054928033258031180900000000000f, 0.276712320841883130000000000000f, 0.385701050314073257000000000000f),
  new Vector3(-0.220429137227870831000000000000f, 0.627123099039437637000000000000f, 0.452125189853263998000000000000f),
  new Vector3(-0.326082659577084266000000000000f, 0.307659394611485448000000000000f, 0.667312050783284594000000000000f)};
    }

    public void KillCell(CellScript cellToDie)
    {
        currentCells.Remove(cellToDie);
        GameObject.Destroy(cellToDie.gameObject, 2.0f);
    }

    public void SplitCell(CellScript cellToSplit)
    {
        cellToSplit.SetScale(0.5f);
        cellToSplit.Reset();
        Vector3 posoffset = Random.onUnitSphere * 0.25f;
        CellScript newCell = GameObject.Instantiate<CellScript>(cellToSplit);
        cellToSplit.transform.position += posoffset;
        newCell.transform.position -= posoffset;
        newCell.Reset();
        //increment generation level
        newCell.SetGenerationLevel(cellToSplit.GenerationLevel + 1);
        cellToSplit.SetGenerationLevel(cellToSplit.GenerationLevel + 1);
        currentCells.Add(newCell);
        newCell.transform.parent = this.transform;


    }

    //public void UpdateVisibleCells()
    //{
    //    //cutPlane

    //    foreach(CellScript cell in currentCells)
    //    {
    //        //if o2  check that fist
    //        //if 

    //       bool abovePlane = cutPlane.TestCullWithPlane(cell.transform.position, cell.transform.lossyScale.x);
       
            

            
    //    }
    //}
    

   

    public bool CutWithPlanesBubble(Vector3 pos, float r)
    {
        if(BubbleCull.isOn)
        {
           Vector3 camPos = Camera.main.transform.position;
            //if sphere is inside cull
            //If radii A+B >= center / center distance
           if( BubbleRadius + r >= (camPos - pos).magnitude)
            {
                //inside 
                return true;
            }
            return false;
        }

        bool plane1 = false;
        bool plane2 = false;
        if(togglePlane1.isOn)
        {

            plane1 = cutPlane.TestCullWithPlane(pos, r);
        }
        if(togglePlane2.isOn)
        {
            plane2 = cutPlane2.TestCullWithPlane(pos, r);
        }
        if(togglePlane1.isOn && togglePlane2.isOn)
        {
            return plane1 && plane2;
        }
        return plane1 || plane2; 
      //  return plane1 || plane2;
    }
}

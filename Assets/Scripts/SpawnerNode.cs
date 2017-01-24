using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Random = UnityEngine.Random;

public class SpawnerNode : Nodes {

    public List<int> Connections = new List<int>();
    private List<Vector3> ConnectionPos = new List<Vector3>();
    private LinkedList<GameObject> VehiclesToSpawn = new LinkedList<GameObject>(); 
    //List of spawnables
    public GameObject CarPrefab;
    public GameObject TruckPrefab;
    public GameObject BikePrefab;
    public GameObject JeepPrefab;

    public bool SpawnCar = true;
    public bool SpawnJeep = false;
    public bool SpawnBike = false;
    public bool SpawnTruck = false;

    public int CarSpawnPerc = 1;
    public int JeepSpawnPerc = 1;
    public int BikeSpawnPerc = 1;
    public int TruckSpawnPerc = 1;

    public float GeneralSpawnRate;

    private float _timeCounter = 0;
    private int totalPerc = 0;


    // Use this for initialization
    void Start () {
	    MainManager.Main.AddNode(this);
        MainManager.Main.AddSpawner(this);
        GeneralSpawnRate = MainManager.Main.GeneralSpawnrate;
        NodeType = Type.Spawner;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    if (!MainManager.Main._isEditMode) return;
	    _timeCounter += Time.deltaTime;// + Random.Range(0,Time.deltaTime)
        if (SpawnCar)totalPerc += CarSpawnPerc;
        if (SpawnJeep) totalPerc += JeepSpawnPerc;
        if (SpawnBike) totalPerc +=BikeSpawnPerc;
        if (SpawnTruck) totalPerc += TruckSpawnPerc;
	    totalPerc = CarSpawnPerc*(SpawnCar ? 1 : 0) + JeepSpawnPerc* (SpawnJeep ? 1 : 0) + BikeSpawnPerc* (SpawnBike ? 1 : 0) + TruckSpawnPerc* (SpawnTruck ? 1 : 0);
	    if (GeneralSpawnRate <= _timeCounter && VehiclesToSpawn.Count != 0 && Connections.Count > 0)
	    {
	        _timeCounter = 0;
            Spawn(VehiclesToSpawn.First.Value);
            VehiclesToSpawn.RemoveFirst();
	    }
        foreach (int con in Connections)
        {
            foreach (Vehicle veh in MainManager.Main.GetCon(con).Vehicles)
            {
                if (veh == null) continue;
                if (Vector3.Distance(veh.transform.position, transform.position) < transform.localScale.x + 0.2 &&
                    veh.GetComponent<UnitBehaviorTree>().GetStartNode() != this)
                {
                    MainManager.Main.GetCon(con).Vehicles.Remove(veh);
                    Destroy(veh.gameObject);
                    break;
                }
            }
        }
    }

    public override float GetFScore(SpawnerNode endNode)
    {
        if (endNode == this) return 0;
        return (float.MaxValue/3)*2;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.GetComponentInParent<Connection>() != null)
        {
            bool alreadyExists = false;
            foreach (var con in Connections)
            {
                if (con == col.gameObject.GetComponent<Connection>().Serial)
                {
                    alreadyExists = true;
                    continue;
                }
            }
            if (!alreadyExists)
            {
                Connections.Add(col.gameObject.transform.parent.GetComponent<Connection>().Serial);
                ConnectionPos.Add(col.gameObject.transform.position);
                if (col.GetComponentInParent<Connection>().Val1 == null)
                    col.GetComponentInParent<Connection>().Val1 = this;
                else if (col.GetComponentInParent<Connection>().Val2 == null)
                    col.GetComponentInParent<Connection>().Val2 = this;
            }
        }
    }

    public void AddVehicle()
    {
        float perc = Random.Range(0, totalPerc);
        float tempperc = CarSpawnPerc*(SpawnCar ? 1 : 0);
        if (perc < tempperc) { VehiclesToSpawn.AddFirst(CarPrefab);
            return;
        }
        tempperc += JeepSpawnPerc*(SpawnJeep ? 1 : 0);
        if (perc < tempperc)
        {
            VehiclesToSpawn.AddFirst(JeepPrefab);
            return;
        }
        tempperc += TruckSpawnPerc*(SpawnTruck ? 1 : 0);
        if (perc < tempperc)
        {
            VehiclesToSpawn.AddFirst(TruckPrefab);
            return;
        }
        tempperc += BikeSpawnPerc * (SpawnBike ? 1 : 0);
        if (perc < tempperc)
        {
            VehiclesToSpawn.AddFirst(BikePrefab);
        }
    }

    public void ClearSpawn()
    {
        VehiclesToSpawn.Clear();
    }
    private void Spawn(GameObject v)
    {
        int conNr = Random.Range(0, Connections.Count);
        int LaneNr = Random.Range(1, MainManager.Main.GetCon(Connections[conNr]).NrOfLanes);

        GameObject g = Instantiate(v, ConnectionPos[conNr],
            Quaternion.FromToRotation(transform.position, ConnectionPos[conNr]));
        g.transform.parent = MainManager.Main.transform;
        MainManager.Main.Vehicles.Add(g.GetComponent<Vehicle>());
        g.GetComponent<UnitBehaviorTree>().SetStartAndEnd(this,MainManager.Main.GetRandomSpawner(this));
        g.GetComponent<UnitBehaviorTree>().Lane = LaneNr;
    }
}

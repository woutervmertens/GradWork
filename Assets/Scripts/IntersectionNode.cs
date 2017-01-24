using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;

public class IntersectionNode : Nodes
{
    private float _lightCounter = 0f;
    public int OpenConnectionIndex = 0;
    public float LightSwitchingRate = 5;
    public float SpeedLimit;
    public List<int> Connections = new List<int>();
    public Dictionary<Vehicle, int> Vehicles = new Dictionary<Vehicle, int>();
    private List<Vehicle> releaseAbleVehicles = new List<Vehicle>();
    private int _releasedIndex = 0;
    private float _releaseTimer = 0;
    private float _releaseRate = 0.5f;
    // Use this for initialization
    void Start () {
	    MainManager.Main.AddNode(this);
        NodeType = Type.Intersection;
    }
	
	// Update is called once per frame
	void Update () {
	    //Handle Lights
	    _releaseTimer += Time.deltaTime;
	    _lightCounter += Time.deltaTime;
	    if (_lightCounter >= LightSwitchingRate)
	    {
            _releasedIndex = 0;
            foreach (var rel in releaseAbleVehicles)
	        {
	            if(rel != null && Vector3.Distance(rel.transform.position, transform.position) < transform.localScale.x) Vehicles.Add(rel,OpenConnectionIndex);
	        }
            releaseAbleVehicles.Clear();
	        OpenConnectionIndex++;
	        _lightCounter = 0;
	        if (OpenConnectionIndex > Connections.Count) OpenConnectionIndex = 0;
            
	        foreach (var veh in Vehicles)
	        {
	            if (veh.Value == Connections[OpenConnectionIndex])
	            {
	                releaseAbleVehicles.Add(veh.Key);
	            }
	        }
	    }

	    foreach (int con in Connections)
	    {
	        foreach (Vehicle veh in MainManager.Main.GetCon(con).Vehicles)
	        {
	            if (veh == null)
                { continue;}
	            if (Vector3.Distance(veh.transform.position, transform.position) < transform.localScale.x && !Vehicles.ContainsKey(veh))
	            {
                    Vehicles.Add(veh, veh.GetComponent<UnitBehaviorTree>().NextConnection);
                    veh.GetComponent<UnitBehaviorTree>().IsOnIntersection = true;
	                veh.transform.position = transform.position;
	            }
	        }
	    }
        //DO THE THING JULIEE
	    if (_releaseTimer > _releaseRate && _releasedIndex < releaseAbleVehicles.Count)
	    {
	        if (releaseAbleVehicles[_releasedIndex] == null) return;
	        releaseAbleVehicles[_releasedIndex].GetComponent<UnitBehaviorTree>().IsOnIntersection = false;
	        _releaseTimer = 0;
	        _releasedIndex++;
	    }
    }

    public override float GetFScore(SpawnerNode endNode)
    {
        return Vector3.Distance(transform.position, endNode.transform.position) + Vehicles.Count;
    }

    void OnTriggerEnter(Collider col)
    {
        Transform tr = col.transform.parent;
        if (tr.GetComponent<Connection>() != null)
        {
            bool alreadyExists = false;
            foreach (var con in Connections)
            {
                if (con == tr.GetComponent<Connection>().Serial)
                {
                    alreadyExists = true;
                    continue;
                }
            }
            if (!alreadyExists)
            {
                Connections.Add(tr.GetComponent<Connection>().Serial);
                if (tr.GetComponent<Connection>().Val1 == null)
                    tr.GetComponent<Connection>().Val1 = this;
                else if(tr.GetComponent<Connection>().Val2 == null)
                    tr.GetComponent<Connection>().Val2 = this;
            }
        }
    }
}

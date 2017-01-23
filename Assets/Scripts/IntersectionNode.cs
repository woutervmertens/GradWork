using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntersectionNode : Nodes
{
    private float _lightCounter = 0f;
    public int OpenConnectionIndex = 0;
    public float LightSwitchingRate = 5;
    public float SpeedLimit;
    public List<int> Connections = new List<int>();
    public Dictionary<Vehicle, int> Vehicles = new Dictionary<Vehicle, int>();
    // Use this for initialization
    void Start () {
	    MainManager.Main.AddNode(this);
	}
	
	// Update is called once per frame
	void Update () {
	    //Handle Lights
	    _lightCounter += Time.deltaTime;
	    if (_lightCounter >= LightSwitchingRate)
	    {
	        OpenConnectionIndex++;
	        _lightCounter = 0;
	        if (OpenConnectionIndex > Connections.Count) OpenConnectionIndex = 0;
	    }

	    foreach (int con in Connections)
	    {
	        foreach (Vehicle veh in MainManager.Main.GetCon(con).Vehicles)
	        {
	            if (Vector3.Distance(veh.transform.position, transform.position) < transform.localScale.x && !Vehicles.ContainsKey(veh))
	            {
                    Vehicles.Add(veh, veh.GetComponent<UnitBehaviorTree>().NextConnection);
                    veh.transform.parent.GetComponent<UnitBehaviorTree>().IsOnIntersection = true;
                }
	        }
	    }

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

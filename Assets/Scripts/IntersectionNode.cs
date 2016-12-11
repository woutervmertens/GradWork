using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntersectionNode : MonoBehaviour {

    public int NrVehiclesCont;
    public float Fitness;
    public float SpeedLimit;
    public List<Connection> Connections = new List<Connection>();
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void VehicleEntered(Vehicle v)
    {
        //Check if light green else stop
        //Turn and Go Towards Next StartNode

    }

    void OnTriggerEnter(Collider col)
    {
        if (col.GetType() == typeof(Vehicle))
        {
            VehicleEntered(col.GetComponent<Vehicle>());
        }
        else if (col.gameObject.GetType() == typeof(Connection))
        {
            bool alreadyExists = false;
            foreach (var con in Connections)
            {
                if (con.GetStartNode() == col.gameObject.GetComponent<Node>() ||
                    con.GetEndNode() == col.gameObject.GetComponent<Node>())
                {
                    alreadyExists = true;
                    continue;
                }
            }
            if (alreadyExists)
            {
                Connections.Add(col.gameObject.transform.parent.GetComponent<Connection>());
            }
        }
    }
}

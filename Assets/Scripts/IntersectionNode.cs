using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntersectionNode : MonoBehaviour {

    public int NrVehiclesCont;
    public float Fitness;
    public float SpeedLimit;
    public List<int> Connections = new List<int>();
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    //Handle Lights
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.GetComponentInParent<Connection>() != null)
        {
            bool alreadyExists = false;
            foreach (var con in Connections)
            {
                if (con == col.GetComponentInParent<Connection>().Serial)
                {
                    alreadyExists = true;
                    continue;
                }
            }
            if (!alreadyExists)
            {
                Connections.Add(col.GetComponentInParent<Connection>().Serial);
            }
        }
    }
}

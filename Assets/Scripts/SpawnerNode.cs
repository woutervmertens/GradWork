using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnerNode : MonoBehaviour {

    public List<int> Connections = new List<int>();
    //List of spawnables

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    //Spawn if i want to
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.GetType() == typeof(Vehicle))
        {
            if (col.gameObject.GetComponent<Vehicle>().GetTargetNode() == this.GetComponent<Node>())
            {
                Destroy(col.gameObject);
            }
        }
        else if (col.transform.GetComponentInParent<Connection>() != null)
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
            }
        }
    }
}

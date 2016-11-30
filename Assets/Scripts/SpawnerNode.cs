using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnerNode : MonoBehaviour {

    public Vector3 Position;
    public List<Connection> Connections = new List<Connection>();
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
        else if (col.gameObject.transform.parent.GetType() == typeof(Connection))
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

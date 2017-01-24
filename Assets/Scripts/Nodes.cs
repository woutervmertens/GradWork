using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type
{
    Connection,
    Spawner,
    Intersection
}

public class Nodes : MonoBehaviour {

	public Type NodeType = Type.Spawner;

    public Nodes Parent { get; set; }

    public float GetFScore()
    {
        return 1;
    }

    public List<int> GetConnections()
    {
        if (NodeType == Type.Spawner)
        {
            return GetComponent<SpawnerNode>().Connections;
        }
        else
        {
            return GetComponent<IntersectionNode>().Connections;
        }
    }
}

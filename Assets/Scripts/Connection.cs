using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Connection : MonoBehaviour
{
    public GameObject ConnectionNodePrefab;

    public List<Transform> nodes = new List<Transform>();
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Add(List<Transform> l)
    {
        nodes = l;//work on this later
    }

    public void Add(Vector3 v)
    {
        GameObject n = Instantiate(ConnectionNodePrefab, v, Quaternion.identity) as GameObject;
        n.transform.parent = this.transform;
        nodes.Add(n.transform);
    }

    public Node GetStartNode()
    {
        return null;
    }

    public Node GetEndNode()
    {
        return null;
    }

    public int GetCount()
    {
        return nodes.Count;
    }
}

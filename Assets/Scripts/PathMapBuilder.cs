using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathMapBuilder : MonoBehaviour {


    class Connection
    {
        public GSDSplineC Spline { get; set; }
        public List<GSDSplineN> Nodes { get; set; }

        public Connection()
        {
            Nodes = new List<GSDSplineN>();
        }
    }

    private List<Vector3> Intersections = new List<Vector3>();
    private List<Connection> Connections = new List<Connection>();

    public GameObject RoadNetwork;

	// Use this for initialization
	void Start () {
	    foreach (Transform child in RoadNetwork.transform)
	    {
	        var tempCon = new Connection();
            //Ignore intersections
            if (!child.GetComponent<GSDRoad>()) continue;
	        var road = child.GetChild(0);
	        var spline = road.gameObject.GetComponent<GSDSplineC>();
	        foreach (var node in spline.mNodes)
	        {
	            if (node.bIsEndPoint)
	            {
	                tempCon.Nodes.Add(node);
	            }
	            if (node.bIsIntersection)
	            {
	                tempCon.Nodes.Add(node);
                    Intersections.Add(node.pos);
	            }
                if (tempCon.Nodes.Count > 1)
	            {
	                tempCon.Spline = spline;
                    Connections.Add(tempCon);
                    tempCon = new Connection();
                    if(!node.bIsEndPoint)tempCon.Nodes.Add(node);
	            }
	        }
	    }

	    Debug.Log("Intersections: " + Intersections.Count);
        //Remove Intersection duplicates
        HashSet<Vector3> hash = new HashSet<Vector3>();
	    foreach (var pos in Intersections)
	    {
	        hash.Add(pos);
	    }
	    Intersections = new List<Vector3>(hash);
        Debug.Log("Intersections no dups: " + Intersections.Count);
        Debug.Log("Connections: " + Connections.Count);
	}
	
}

using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    public PathMapBuilder.IntersectionPoint StartIntersectionPoint;

    public PathMapBuilder.IntersectionPoint EndIntersectionPoint;
    public List<PathMapBuilder.IntersectionPoint> Route = new List<PathMapBuilder.IntersectionPoint>();
    public List<PathData> Path = new List<PathData>();

    public void ConvertRouteToPath()
    {
        for (int j = 1; j < Route.Count-1; j++)
        {
            PathData path = new PathData();

            Node startNode = new Node();
            startNode.Pos = Route[j - 1].Node1.pos;
            startNode.IsIntersection = !Route[j - 1].bIsSpawner;
            path.StartNode = startNode;

            Node endNode = new Node();
            endNode.Pos = Route[j].Node1.pos;
            startNode.IsIntersection = !Route[j].bIsSpawner;
            path.EndNode = endNode;

            PathMapBuilder.Connection con = null;
            foreach (var road in Route[j-1].Roads)
            {
                if (road.StartEndPoint.Contains(Route[j]))
                {
                    con = road;
                    break;
                }
            }
            if(con == null) { Debug.LogError("ConvertRouteToPath: Connection not found!");
                return;
            }

            path.Spline = con.Spline;
            path.RoadData = con.RoadData;
            if (con.StartEndPoint[0] == Route[j - 1])
            {
                path.StartF = con.GetStartF();
                path.EndF = con.GetEndF();
            }
            else
            {
                path.StartF = con.GetEndF();
                path.EndF = con.GetStartF();
            }
            path.Length = con.GetLength();

            if (path.StartF < path.EndF) path.Direction = 1;
            else path.Direction = -1;

            Path.Add(path);
        }
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Vehicles.Behaviors;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using Connection = PathMapBuilder.Connection;

public class Vehicle : MonoBehaviour
{
    public PathMapBuilder.IntersectionPoint StartIntersectionPoint;
    public PathMapBuilder.IntersectionPoint EndIntersectionPoint;
    public List<PathMapBuilder.IntersectionPoint> Route = new List<PathMapBuilder.IntersectionPoint>();
    public List<PathData> Path = new List<PathData>();
    public GSDRoad TestRoad;
    public bool bUseTestRoad = true;

    public void ConvertRouteToPath()
    {
        for (int j = 1; j < Route.Count; j++)
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

            Connection con = null;
            foreach (var road in Route[j-1].Roads)
            {
                if (road.StartEndPoint.Contains(Route[j]))
                {
                    con = road;
                    break;
                }
            }
            if(con == null) {
                if (RoadManager.DebubMode) Debug.LogError("ConvertRouteToPath: Connection not found!");
                return;
            }

            path.Spline = con.Spline;
            path.RoadData = con.RoadData;
            path.RoadData.NrOfLanes = path.Spline.tRoad.opt_Lanes / 2;
            path.RoadData.MaxSpeed = RoadManager.BaseMaxSpeed + RoadManager.MaxSpeedAddedPerLane * path.RoadData.NrOfLanes;
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

    public float GetFValue()
    {
        return GetComponent<VehicleBehavior>()._currentSplinePos;
    }

    public int GetCurrentLane()
    {
        return GetComponent<VehicleBehavior>().CurrentLane;
    }

    public float GetSpeed()
    {
        return GetComponent<VehicleBehavior>().Speed;
    }

    private void UseTestRoute()
    {
        int i = 0;
        foreach (var node in TestRoad.GSDSpline.mNodes)
        {
            var inter =  new PathMapBuilder.IntersectionPoint();
            inter.Node1 = node;
            inter.bIsSpawner = node.bIsEndPoint;
            if (i > 0)
            {
                var con = new Connection();
                con.Nodes.Add(node);
                con.Nodes.Add(TestRoad.GSDSpline.mNodes[i-1]);
                con.Spline = TestRoad.GSDSpline;
                con.StartEndPoint.Add(Route[i-1]);
                con.StartEndPoint.Add(inter);
                inter.Roads.Add(con);
                Route[i-1].Roads.Add(con);
            }
            Route.Add(inter);
            i++;
        }
        if(RoadManager.DebubMode) Debug.Log("Route set: " + Route.Count);
        ConvertRouteToPath();
    }
	// Use this for initialization
	void Start () {
		if(bUseTestRoad) UseTestRoute();
	}
}

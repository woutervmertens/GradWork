using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;
using Connection = PathMapBuilder.Connection;
using IntersectionPoint = PathMapBuilder.IntersectionPoint;

public class RoadManager : MonoBehaviour {
    public static List<Connection> Roads = new List<Connection>();
    public static List<IntersectionPoint> IntersectionPoints = new List<IntersectionPoint>();
    public static List<IntersectionPoint> Spawners = new List<IntersectionPoint>();
    public static List<Vehicle> Vehicles = new List<Vehicle>();

    public static IntersectionPoint GetRandomOtherIntersectionPoint(IntersectionPoint current)
    {
        List<IntersectionPoint> others = IntersectionPoints;
        others.Remove(current);
        return others[Mathf.FloorToInt((Random.value * others.Count))];
    }
    public static IntersectionPoint GetRandomIntersectionPoint()
    {
        return IntersectionPoints[Mathf.FloorToInt((Random.value * IntersectionPoints.Count))];
    }

    public static DijkstraTable CalculatePathTable(IntersectionPoint start)
    {
        List<IntersectionPoint> Checked = new List<IntersectionPoint>();
        List<IntersectionPoint> Unchecked = new List<IntersectionPoint>(IntersectionPoints);

        IntersectionPoint current = start;

        DijkstraTable dijkstraTable = new DijkstraTable();
        dijkstraTable.Fill(IntersectionPoints, start);
        while (Unchecked.Count > 0)
        {
            //Remove from unchecked
            var test = Unchecked.Remove(current);
            if(!test)
                Debug.LogError("List remove unsuccesfull!");

            //Add to checked
            Checked.Add(current);

            List<DijkstraSingleConnectionData> connectedIntersectionPoints = new List<DijkstraSingleConnectionData>();

            //Find all connected Intersections and save distance data
            foreach (Connection road in current.Roads)
            {
                if (Unchecked.Contains(road.StartEndPoint[0]))
                {
                    connectedIntersectionPoints.Add(new DijkstraSingleConnectionData
                    {
                        Distance = road.GetLength(), TargetIntersectionNode = road.StartEndPoint[0]
                    });
                }
                if (Unchecked.Contains(road.StartEndPoint[1]))
                {
                    connectedIntersectionPoints.Add(new DijkstraSingleConnectionData
                    {
                        Distance = road.GetLength(),
                        TargetIntersectionNode = road.StartEndPoint[1]
                    });
                }
            }

            //compare new distance with old dist, replace if shorter
            foreach (var dijkstraSingleConnectionData in connectedIntersectionPoints)
            {
                float newDistance = dijkstraTable.Table[current].Distance + dijkstraSingleConnectionData.Distance;
                float oldDistance = dijkstraTable.Table[dijkstraSingleConnectionData.TargetIntersectionNode].Distance;

                if (newDistance < oldDistance)
                {
                    dijkstraTable.Table[dijkstraSingleConnectionData.TargetIntersectionNode].Distance = newDistance;
                    dijkstraTable.Table[dijkstraSingleConnectionData.TargetIntersectionNode].PrevIntersectionNode =
                        current;
                }
            }

            //find next point to be checked
            PathMapBuilder.IntersectionPoint nextIntersectionPoint = current;
            float distance = float.MaxValue;

            List<PathMapBuilder.IntersectionPoint> ignoreIntersectionPoints = Checked;
            foreach (var dijkstraTableData in dijkstraTable.Table)
            {
                if (!ignoreIntersectionPoints.Contains(dijkstraTableData.Key))
                {
                    if (dijkstraTableData.Value.Distance < distance)
                    {
                        nextIntersectionPoint = dijkstraTableData.Key;
                        distance = dijkstraTableData.Value.Distance;
                    }
                }
            }
            current = nextIntersectionPoint;
        }
        return dijkstraTable;
    }

    public static List<PathMapBuilder.IntersectionPoint> CalculateRoute(DijkstraTable table, PathMapBuilder.IntersectionPoint start,
        PathMapBuilder.IntersectionPoint end)
    {
        List<PathMapBuilder.IntersectionPoint> path = new List<PathMapBuilder.IntersectionPoint>();
        path.Add(end);

        PathMapBuilder.IntersectionPoint current = end;
        while (current != start)
        {
            current = table.Table[current].PrevIntersectionNode;
            path.Add(current);
        }
        path.Reverse();
        return path;
    }

    public static void CalculateDijkstraTables()
    {
        foreach (var intersection in IntersectionPoints)
        {
            if (intersection.bIsSpawner)
            {
                intersection.DijkstraTable = CalculatePathTable(intersection);
            }
        }
    }

    public static void AddRoad(Connection road)
    {
        if (!Roads.Contains(road))
        {
            Roads.Add(road);
        }
    }

    public static void AddIntersection(PathMapBuilder.IntersectionPoint intersectionPoint)
    {
        if (!IntersectionPoints.Contains(intersectionPoint))
        {
            IntersectionPoints.Add(intersectionPoint);
            if(intersectionPoint.bIsSpawner) Spawners.Add(intersectionPoint);
        }
    }

    public static void AddVehicle(Vehicle vehicle)
    {
        Vehicles.Add(vehicle);
    }
}

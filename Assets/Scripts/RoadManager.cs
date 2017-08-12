using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoadManager : MonoBehaviour {
    public static List<Road> Roads = new List<Road>();
    public static List<IntersectionPoint> IntersectionPoints = new List<IntersectionPoint>();

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
            Unchecked.Remove(current);

            //Add to checked
            Checked.Add(current);

            List<DijkstraSingleConnectionData> connectedIntersectionPoints = new List<DijkstraSingleConnectionData>();

            //Find all connected Intersections and save distance data
            foreach (Road road in current.Roads)
            {
                if (Unchecked.Contains(road.StartIntersectionPoint))
                {
                    connectedIntersectionPoints.Add(new DijkstraSingleConnectionData
                    {
                        Distance = road.Length, TargetIntersectionNode = road.StartIntersectionPoint
                    });
                }
                if (Unchecked.Contains(road.EndIntersectionPoint))
                {
                    connectedIntersectionPoints.Add(new DijkstraSingleConnectionData
                    {
                        Distance = road.Length,
                        TargetIntersectionNode = road.EndIntersectionPoint
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
            IntersectionPoint nextIntersectionPoint = current;
            float distance = float.MaxValue;

            List<IntersectionPoint> ignoreIntersectionPoints = Checked;
            foreach (var dijkstraTableData in dijkstraTable.Table)
            {
                if (ignoreIntersectionPoints.Contains(dijkstraTableData.Key))
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

    public static List<IntersectionPoint> CalculateRoute(DijkstraTable table, IntersectionPoint start,
        IntersectionPoint end)
    {
        List<IntersectionPoint> path = new List<IntersectionPoint>();
        path.Add(end);

        IntersectionPoint current = end;
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

    public static void AddRoad(Road road)
    {
        if (!Roads.Contains(road))
        {
            Roads.Add(road);
        }
    }

    public static void AddIntersection(IntersectionPoint intersectionPoint)
    {
        if (!IntersectionPoints.Contains(intersectionPoint))
        {
            IntersectionPoints.Add(intersectionPoint);
        }
    }

    public static void AddVehicle(Vehicle vehicle)
    {
        Vehicles.Add(vehicle);
    }
}

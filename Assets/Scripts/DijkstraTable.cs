using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DijkstraSingleConnectionData
{
    public PathMapBuilder.IntersectionPoint TargetIntersectionNode;
    public float Distance;
}

public class DijkstraTableData
{
    public float Distance;
    public PathMapBuilder.IntersectionPoint PrevIntersectionNode;
}

public class DijkstraTable : MonoBehaviour {

	public Dictionary<PathMapBuilder.IntersectionPoint, DijkstraTableData> Table = new Dictionary<PathMapBuilder.IntersectionPoint, DijkstraTableData>();

    public void Fill(List<PathMapBuilder.IntersectionPoint> list, PathMapBuilder.IntersectionPoint start)
    {
        foreach (var intersectionNode in list)
        {
            Table.Add(intersectionNode, new DijkstraTableData() {Distance = float.MaxValue, PrevIntersectionNode = null});
        }

        Table[start].Distance = 0;
    }
}

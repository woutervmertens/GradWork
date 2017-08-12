using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DijkstraSingleConnectionData
{
    public IntersectionPoint TargetIntersectionNode;
    public float Distance;
}

public class DijkstraTableData
{
    public float Distance;
    public IntersectionPoint PrevIntersectionNode;
}

public class DijkstraTable : MonoBehaviour {

	public Dictionary<IntersectionPoint, DijkstraTableData> Table = new Dictionary<IntersectionPoint, DijkstraTableData>();

    public void Fill(List<IntersectionPoint> list, IntersectionPoint start)
    {
        foreach (var intersectionNode in list)
        {
            Table.Add(intersectionNode, new DijkstraTableData() {Distance = float.MaxValue, PrevIntersectionNode = null});
        }

        Table[start].Distance = 0;
    }
}

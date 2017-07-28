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

        public float GetLength()
        {
            return Mathf.Abs(Nodes[0].tDist - Nodes[1].tDist);
        }
    }

    private List<Vector3> Intersections = new List<Vector3>();
    private List<GSDRoadIntersection> IntTest = new List<GSDRoadIntersection>();
    private List<Connection> Connections = new List<Connection>();

    public GameObject RoadNetwork;

    public bool Show = false;
    public float DrawDetail = 0.1f;

	// Use this for initialization
	void Start () {
	    foreach (Transform child in RoadNetwork.transform)
	    {
	        var tempCon = new Connection();
            //Ignore intersections
	        if (!child.GetComponent<GSDRoad>())
	        {
	            foreach (Transform inter in child.transform)
	            {
	                var i = inter.GetComponent<GSDRoadIntersection>();
                    IntTest.Add(i);
	            }
                continue;
	        }
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

    void Update()
    {
        if (Show)
            Draw();
    }

    private void Draw()
    {
        Color c = Color.green;
        //Connections
        Vector3 firstPoint, secondPoint, firstPointAlt, secondPointAlt;
        foreach (var con in Connections)
        {
            float detail = DrawDetail;
            float prevDetail = 0.0f;
            while (detail < 1)
            {
                firstPoint = con.Spline.GetSplineValue(prevDetail);
                secondPoint = con.Spline.GetSplineValue(detail);
                firstPoint.y = secondPoint.y = 5;
                Debug.DrawLine(firstPoint, secondPoint, c,Time.deltaTime);
                prevDetail = detail;
                detail += DrawDetail;
            }
            firstPoint = con.Spline.GetSplineValue(1 - DrawDetail);
            secondPoint = con.Spline.GetSplineValue(0.999999f);
            firstPoint.y = secondPoint.y = 5;
            Debug.DrawLine(firstPoint, secondPoint, c, Time.deltaTime);
        }

        //Intersections
        foreach (var i in IntTest)
        {
            float detail = DrawDetail;
            float prevDetail = 0.0f;
            Vector3 control = i.Node1.pos;

            firstPoint = i.Node1.GSDSpline.GetSplineValue(i.Node1.tTime - DrawDetail*2);
            firstPointAlt = i.Node1.GSDSpline.GetSplineValue(i.Node1.tTime + DrawDetail * 2);
            secondPoint = i.Node2.GSDSpline.GetSplineValue(i.Node2.tTime - DrawDetail*2);
            secondPointAlt = i.Node2.GSDSpline.GetSplineValue(i.Node2.tTime + DrawDetail * 2);
            while (detail < 1)
            {
                Debug.DrawLine(CalculateSplinePoint(prevDetail, firstPoint, secondPoint, control), CalculateSplinePoint(detail, firstPoint, secondPoint, control), c, Time.deltaTime);
                Debug.DrawLine(CalculateSplinePoint(prevDetail, firstPointAlt, secondPoint, control), CalculateSplinePoint(detail, firstPointAlt, secondPoint, control), c, Time.deltaTime);
                Debug.DrawLine(CalculateSplinePoint(prevDetail, firstPoint, secondPointAlt, control), CalculateSplinePoint(detail, firstPoint, secondPointAlt, control), c, Time.deltaTime);
                Debug.DrawLine(CalculateSplinePoint(prevDetail, firstPointAlt, secondPointAlt, control), CalculateSplinePoint(detail, firstPointAlt, secondPointAlt, control), c, Time.deltaTime);
                prevDetail = detail;
                detail += DrawDetail;
            }
        }
    }

    private Vector3 CalculateSplinePoint(float t, Vector3 startpoint, Vector3 endpoint, Vector3 control)
    {
        Vector3 r = (Mathf.Pow((1 - t), 3) * startpoint)
               + (3 * Mathf.Pow((1 - t), 2) * t * control)
               + (3 * (1 - t) * Mathf.Pow(t, 2) * control)
               + (Mathf.Pow(t, 3) * endpoint);
        r.y = 5;
        return r;
    }
}

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
        
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

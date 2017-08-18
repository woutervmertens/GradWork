using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    //Datainterfaces
    public class RoadData
    {
        public List<Vehicle> PosVehicles { get; private set; }
        public List<Vehicle> NegVehicles { get; private set; }
        public float MaxSpeed { get; private set; }
        public int NrOfLanes { get; private set; }
    }

    //public class Road
    //{
    //    public IntersectionPoint StartIntersectionPoint;
    //    public IntersectionPoint EndIntersectionPoint;
    //    public float Length;
    //}

    public class VehicleData
    {
        public float MaxSpeed { get; private set; }
        public float AccelerationSpeed { get; private set; }
        public float BreakSpeed { get; private set; }
        public float Length { get; private set; }
        public float Width { get; private set; }
        public float ChangeSpeed { get; private set; }
        public float PreferredLane { get; private set; }
        public float BufferLength { get; private set; }
    }

    public class Node
    {
        public Vector3 Pos { get; set; }
        public bool IsIntersection { get; set; }
    }

    public class PathData
    {
        public Node StartNode { get; set; }
        public float StartF { get; set; }
        public Node EndNode { get; set; }
        public float EndF { get; set; }
        public GSDSplineC Spline { get; set; }
        public RoadData RoadData { get; set; }
        public float Length { get; set; }
        public int Direction { get; set; }
    }
}

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
        public Vector3 Pos { get; private set; }
        public bool IsIntersection { get; private set; }
    }

    public class PathData
    {
        public Node StartNode { get; private set; }
        public float StartF { get; private set; }
        public Node EndNode { get; private set; }
        public float EndF { get; private set; }
        public GSDSplineC Spline { get; private set; }
        public RoadData RoadData { get; private set; }
        public int Direction { get; private set; }
    }
}

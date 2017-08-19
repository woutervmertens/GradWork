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

    public enum VehicleType
    {
        Car = 0,
        Jeep,
        Van
    }

    [System.Serializable]
    public class VehicleData
    {
        public float MaxSpeed;
        public float AccelerationSpeed;
        public float BreakSpeed;
        public float Length;
        public float Width;
        public float ChangeSpeed;
        public float PreferredLane;
        public float BufferLength;
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

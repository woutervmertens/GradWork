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
        public List<Vehicle> PosVehicles;
        public List<Vehicle> NegVehicles;
        public float MaxSpeed;
        public int NrOfLanes;

        public RoadData()
        {
            PosVehicles = new List<Vehicle>();
            NegVehicles = new List<Vehicle>();
        }
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

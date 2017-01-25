using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum VehicleType
{
    Car,
    Truck,
    Van,
    Bike
}

public class Vehicle: MonoBehaviour
{
    public VehicleType VT;

    //Attributes
    public float Speed;
    public float Width;
    public float Lenght;
    public float LaneChangeSpeed;
    public float BreakSpeed;
    public float AccelSpeed;
    public float BufferLength;
    public int Lane;

    public void SetToLane(float _laneWidth)
    {
        Vector3 l = Vector3.right * (_laneWidth*(Lane + 1) - (_laneWidth*0.5f));
        transform.GetChild(0).localPosition = l;
    }

    public bool CheckLeftLane(List<Transform> n, float lw)
    {
        switch (VT)
        {
            case VehicleType.Car:
                return transform.GetChild(0).GetComponent<Car>().CheckLeft(n,lw);
                break;
            case VehicleType.Truck:
                return transform.GetChild(0).GetComponent<Truck>().CheckLeft(n,lw);
                break;
            case VehicleType.Van:
                return transform.GetChild(0).GetComponent<Van>().CheckLeft(n,lw);
                break;
            case VehicleType.Bike:
                return transform.GetChild(0).GetComponent<Bike>().CheckLeft(n,lw);
                break;
            default:
                return false;
        }
        return false;
    }
    public bool CheckRightLane(List<Transform> n, float lw)
    {
        switch (VT)
        {
            case VehicleType.Car:
                return transform.GetChild(0).GetComponent<Car>().CheckRight(n, lw);
                break;
            case VehicleType.Truck:
                return transform.GetChild(0).GetComponent<Truck>().CheckRight(n, lw);
                break;
            case VehicleType.Van:
                return transform.GetChild(0).GetComponent<Van>().CheckRight(n, lw);
                break;
            case VehicleType.Bike:
                return transform.GetChild(0).GetComponent<Bike>().CheckRight(n, lw);
                break;
            default:
                return false;
        }
        return false;
    }

    public float GetVehicleLength()
    {
        switch (VT)
        {
            case VehicleType.Car:
                return transform.GetChild(0).GetComponent<Car>().GetLength();
                break;
            case VehicleType.Truck:
                return transform.GetChild(0).GetComponent<Truck>().GetLength();
                break;
            case VehicleType.Van:
                return transform.GetChild(0).GetComponent<Van>().GetLength();
                break;
            case VehicleType.Bike:
                return transform.GetChild(0).GetComponent<Bike>().GetLength();
                break;
            default:
                return 0;
        }
        return 0;
    }

    public void Destroy()
    {
        while (MainManager.Main.Vehicles.Contains(this))
        {
            MainManager.Main.Vehicles.Remove(this);
        }
        foreach (var nodes in MainManager.Main.NodeList)
        {
            Nodes node = nodes as Nodes;
            switch (node.NodeType)
            {
                case Type.Connection:
                    if (node.GetComponent<Connection>().Vehicles.Contains(this)) node.GetComponent<Connection>().Vehicles.Remove(this);
                    break;
                case Type.Spawner:
                    if (node.GetComponent<SpawnerNode>().VehiclesToSpawn.Contains(this.gameObject)) node.GetComponent<SpawnerNode>().VehiclesToSpawn.Remove(this.gameObject);
                    break;
                case Type.Intersection:
                    if (node.GetComponent<IntersectionNode>().Vehicles.ContainsKey(this)) node.GetComponent<IntersectionNode>().Vehicles.Remove(this);
                    if (node.GetComponent<IntersectionNode>().releaseAbleVehicles.Contains(this)) node.GetComponent<IntersectionNode>().releaseAbleVehicles.Remove(this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        for (int i = 0; i < MainManager.Main.GetConnectionCount()+1; i++)
        {
            if (MainManager.Main.GetCon(i).Vehicles.Contains(this)) MainManager.Main.GetCon(i).Vehicles.Remove(this);
        }
        Destroy(this.gameObject);
    }
}

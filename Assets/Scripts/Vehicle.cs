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

    public bool RayTest(float length)
    {
        switch (VT)
        {
            case VehicleType.Car:
                return transform.GetChild(0).GetComponent<Car>().CheckHit(length);
                break;
            case VehicleType.Truck:
                return transform.GetChild(0).GetComponent<Truck>().CheckHit(length);
                break;
            case VehicleType.Van:
                return transform.GetChild(0).GetComponent<Van>().CheckHit(length);
                break;
            case VehicleType.Bike:
                return transform.GetChild(0).GetComponent<Bike>().CheckHit(length);
                break;
            default:
                return false;
        }
        return false;
    }

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
}

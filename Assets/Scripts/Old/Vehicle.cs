using System;
using UnityEngine;
using System.Collections;

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

    public bool CheckLeftLane()
    {
        switch (VT)
        {
            case VehicleType.Car:
                return transform.GetChild(0).GetComponent<Car>().CheckLeft();
                break;
            case VehicleType.Truck:
                return transform.GetChild(0).GetComponent<Truck>().CheckLeft();
                break;
            case VehicleType.Van:
                return transform.GetChild(0).GetComponent<Van>().CheckLeft();
                break;
            case VehicleType.Bike:
                return transform.GetChild(0).GetComponent<Bike>().CheckLeft();
                break;
            default:
                return false;
        }
        return false;
    }
    public bool CheckRightLane()
    {
        switch (VT)
        {
            case VehicleType.Car:
                return transform.GetChild(0).GetComponent<Car>().CheckRight();
                break;
            case VehicleType.Truck:
                return transform.GetChild(0).GetComponent<Truck>().CheckRight();
                break;
            case VehicleType.Van:
                return transform.GetChild(0).GetComponent<Van>().CheckRight();
                break;
            case VehicleType.Bike:
                return transform.GetChild(0).GetComponent<Bike>().CheckRight();
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

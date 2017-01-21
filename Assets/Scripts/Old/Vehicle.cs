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
}

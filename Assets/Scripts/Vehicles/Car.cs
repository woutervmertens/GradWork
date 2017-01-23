using UnityEngine;
using System.Collections;
using System.Xml.Schema;

public class Car : Vehicle {
    // Use this for initialization
    VehicleType VT = VehicleType.Car;

    public bool CheckHit(float lenght)
    {
        Vector3 fwd = Vector3.forward;
        int layerCast = 1 << 9;
        return Physics.Raycast(transform.position, fwd, lenght, layerCast);
    }

    public bool CheckLeft()
    {

        return false;
    }

    public bool CheckRight()
    {
        return false;
    }

    public float GetLength()
    {
        return transform.localScale.z;
    }
    
}

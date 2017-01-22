using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bike : Vehicle {

    VehicleType VT = VehicleType.Bike;

    public bool CheckHit(float lenght)
    {
        Vector3 fwd = Vector3.forward;
        return Physics.Raycast(transform.position, fwd, lenght);
    }
    public bool CheckLeft()
    {
        return false;
    }

    public bool CheckRight()
    {
        return false;
    }
}

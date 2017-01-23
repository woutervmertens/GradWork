using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Van : Vehicle {

    VehicleType VT = VehicleType.Van;

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

    public float GetLength()
    {
        return transform.localScale.z;
    }
}

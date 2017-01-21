using UnityEngine;
using System.Collections;
using System.Xml.Schema;

public class Car : Vehicle {
    // Use this for initialization
    VehicleType VT = VehicleType.Car;

    public bool CheckHit(float lenght)
    {
        Vector3 fwd = Vector3.forward;
        return Physics.Raycast(transform.position, fwd, lenght);
    }
    
}

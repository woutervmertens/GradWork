using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

    public bool CheckLeft(List<Transform> n, float lw)
    {
        Vector3 leftVector3 = transform.TransformVector(Vector3.left * lw);
        
        foreach (Transform _n in n)
        {
            if (Vector3.Distance(_n.position, transform.position + leftVector3) < transform.localScale.z) return false;
        }
        return true;
    }

    public bool CheckRight(List<Transform> n, float lw)
    {
        Vector3 rightVector3 = transform.TransformVector(Vector3.right * lw);

        foreach (Transform _n in n)
        {
            if (Vector3.Distance(_n.position, transform.position + rightVector3) < transform.localScale.z) return false;
        }
        return true;
    }

    public float GetLength()
    {
        return transform.localScale.z;
    }
    
}

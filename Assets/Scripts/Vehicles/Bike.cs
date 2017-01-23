using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bike : Vehicle {

    VehicleType VT = VehicleType.Bike;
    private bool _isOnIntersection = false;

    void Update()
    {
        if (_isOnIntersection != GetComponent<UnitBehaviorTree>().IsOnIntersection)
        {
            _isOnIntersection = GetComponent<UnitBehaviorTree>().IsOnIntersection;
            GetComponent<BoxCollider>().enabled = !_isOnIntersection;
        }
    }
    public bool CheckHit(float lenght)
    {
        Vector3 fwd = Vector3.forward;
        return Physics.Raycast(transform.position, fwd, lenght);
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

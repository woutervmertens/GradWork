using UnityEngine;
using System.Collections;

public class Vehicle: MonoBehaviour
{
    public int Type;
    public float Length;
    public float Mass;
    public float MaxSpeed;
    public float AccelerationRate;
    public float DecelerationRate;
    public float DesiredSpeedPerc;
    public float DesiredBubbleSize;
    public float TimeOnRoad;

    public Nodes GetTargetNode()
    {
        return null;
    }
}

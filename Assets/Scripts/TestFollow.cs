using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFollow : MonoBehaviour
{
    private float timer = 0.0f;

    public bool reset = false;

    public GSDRoad road;

    private GSDSplineC spline;

    public float speed = 10;

    void Start()
    {
        spline = road.GSDSpline;
    }
    
	// Update is called once per frame
	void Update ()
	{
	    timer += Time.deltaTime;
	    if (reset)
	    {
	        reset = false;
	        timer = 0.0f;
	    }

	    float posAlongSpline = timer/spline.distance;
	    transform.position = spline.GetSplineValue((posAlongSpline*speed)%1);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralManager : MonoBehaviour
{

    public bool DebugMode = false;

    public Text VehicleNr;

    public Text Fps;
	// Use this for initialization
	void Start ()
	{
	    RoadManager.DebubMode = DebugMode;
	}

    void Update()
    {
        VehicleNr.text = ("#Vehicles: " + RoadManager.NumberOfVehicles).ToString();
        Fps.text = ("FPS: " + (int)(1.0 / Time.deltaTime)).ToString();
    }
}

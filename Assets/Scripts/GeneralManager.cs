using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class GeneralManager : MonoBehaviour
{

    public bool DebugMode = false;

    public Text VehicleNr;

    public Text Fps;

    public VehicleData CarData;

    public VehicleData JeepData;

    public VehicleData VanData;

    public float BaseMaxRoadSpeed = 30;

    public float RoadSpeedAddedPerLane = 5;
	// Use this for initialization
	void Start ()
	{
	    RoadManager.DebubMode = DebugMode;
	    RoadManager.BaseMaxSpeed = BaseMaxRoadSpeed;
	    RoadManager.MaxSpeedAddedPerLane = RoadSpeedAddedPerLane;
        if(RoadManager.VehicleDictionary.Count < 1) LoadInVehicleDictionary();
	}

    void Update()
    {
        VehicleNr.text = ("#Vehicles: " + RoadManager.NumberOfVehicles).ToString();
        Fps.text = ("FPS: " + (int)(1.0 / Time.deltaTime)).ToString();
    }

    public void LoadInVehicleDictionary()
    {
        RoadManager.VehicleDictionary = new Dictionary<VehicleType, VehicleData>();
        RoadManager.VehicleDictionary.Add(VehicleType.Car, CarData);
        RoadManager.VehicleDictionary.Add(VehicleType.Jeep, JeepData);
        RoadManager.VehicleDictionary.Add(VehicleType.Van, VanData);
    }
}

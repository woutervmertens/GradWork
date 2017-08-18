using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldVehicleSpawn : MonoBehaviour
{

    public int MaxNumberOfVehicles = 1;

    public int CurrentNumberOfVehicles = 0;

    public float SpawnSpeed = 0.5f;

    private float _spawnTimer = 0;
	
	// Update is called once per frame
	void Update () {
	    if (CurrentNumberOfVehicles < MaxNumberOfVehicles)
	    {
	        _spawnTimer += Time.deltaTime;
	        if (_spawnTimer > SpawnSpeed)
	        {
	            Spawn();
	            _spawnTimer = 0;
	        }
	    }
	}

    private void Spawn()
    {
        CurrentNumberOfVehicles++;
        RoadManager.GetRandomIntersectionPoint().SpawnVehicle();
    }
}

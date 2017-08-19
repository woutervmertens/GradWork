using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralManager : MonoBehaviour
{

    public bool DebugMode = false;
	// Use this for initialization
	void Start ()
	{
	    RoadManager.DebubMode = DebugMode;
	}
}

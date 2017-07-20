using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoCheck : MonoBehaviour
{

    public GameObject RoadSystem;

    public bool CheckNodes = false;
	
	// Update is called once per frame
	void Update () {
	    if (CheckNodes)
	    {
	        int nr = 0;
	        CheckNodes = false;
	        foreach (Transform child in RoadSystem.transform)
	        {
	            if (child.GetComponent<GSDRoad>() != null)
	            {
	                nr++;
	                Debug.LogFormat("Road {0} has {1} nodes.", nr, child.GetComponent<GSDRoad>().GSDSpline.mNodes.Count);
	            }
	        }
	    }
	}
}

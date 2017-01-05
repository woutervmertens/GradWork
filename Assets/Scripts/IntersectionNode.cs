using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IntersectionNode : MonoBehaviour
{
    private float _lightCounter = 0f;
    public int OpenConnectionIndex = 0;
    public float LightSwitchingRate = 5;
    public float SpeedLimit;
    public List<int> Connections = new List<int>();
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    //Handle Lights
	    _lightCounter += Time.deltaTime;
	    if (_lightCounter >= LightSwitchingRate)
	    {
	        OpenConnectionIndex++;
	        _lightCounter = 0;
	        if (OpenConnectionIndex > Connections.Count) OpenConnectionIndex = 0;
	    }

	}

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.GetComponentInParent<Connection>() != null)
        {
            bool alreadyExists = false;
            foreach (var con in Connections)
            {
                if (con == col.GetComponentInParent<Connection>().Serial)
                {
                    alreadyExists = true;
                    continue;
                }
            }
            if (!alreadyExists)
            {
                Connections.Add(col.GetComponentInParent<Connection>().Serial);
            }
        }
    }
}

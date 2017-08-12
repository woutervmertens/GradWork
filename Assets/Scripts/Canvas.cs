using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Canvas : MonoBehaviour
{
    public Text txtFps;
    public Text txtVeh;
    private int _vehNr = 0;
	
	// Update is called once per frame
	void Update ()
	{
	    float fps = 1/Time.deltaTime;
	    int fpsInt = (int)fps;
	    txtFps.text = "FPS: " + fpsInt.ToString();
	    int _actualVehNr = 0;
        //if(MainManager.Main != null) _actualVehNr = MainManager.Main.VehiclesNr;
	    if (_actualVehNr != _vehNr)
	    {
            _vehNr = _actualVehNr;
	    }
        txtVeh.text = "Vehicles: " + _vehNr.ToString();
    }
}

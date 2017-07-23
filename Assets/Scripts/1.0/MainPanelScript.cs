using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanelScript : MonoBehaviour
{
    public InputField InVeh;
    public InputField InD;
    public InputField UnSp;

    private int mv;
    private float mdt;
    private float gsr;

	// Use this for initialization
	void Start ()
	{
	    mv = MainManager.Main.MaxVehicles;
	    mdt = MainManager.Main.MaxDeltaTime;
	    gsr = MainManager.Main.GeneralSpawnrate;
	    InVeh.text = mv.ToString();
	    InVeh.placeholder.GetComponent<Text>().text = mv.ToString();
        InD.text = mdt.ToString();
        InD.placeholder.GetComponent<Text>().text = mdt.ToString();
        UnSp.text = gsr.ToString();
        UnSp.placeholder.GetComponent<Text>().text = gsr.ToString();
    }

    public void MaxVeh(string i)
    {
        i = InVeh.text;
        int b = StripNonInts(i);
        if (b > 0) mv = b;
    }

    public void MaxD(string i)
    {
        i = InVeh.text;
        float b = StripNonFloats(i);
        if (b > 0) mdt = b;
    }

    public void GenSp(string i)
    {
        i = InVeh.text;
        float b = StripNonFloats(i);
        if (b > 0) gsr = b;
    }

    public void Show(bool t)
    {
        if (t) Start();
        else Close();
    }
    private void Close()
    {
        if (MainManager.Main.MaxVehicles != mv) MainManager.Main.MaxVehicles = mv;
        if (MainManager.Main.MaxDeltaTime != mdt) MainManager.Main.MaxDeltaTime = mdt;
        if (MainManager.Main.GeneralSpawnrate != gsr) MainManager.Main.GeneralSpawnrate = gsr;
    }

    private float StripNonFloats(string i)
    {
        string resString = "";
        bool hadDot = false;
        for (var j = 0; j < i.Length; j++)
        {
            var sub = i.Substring(j, 1);
            int tempInt;

            if (sub == "." && !hadDot)
            {
                hadDot = true;
                resString += sub;
            }
            else if (int.TryParse(sub, out tempInt))
            {
                resString += sub;
            }
        }
        return float.Parse(resString);
    }
    private int StripNonInts(string i)
    {
        var resString = "";
        for (int j = 0; j < i.Length; j++)
        {
            var sub = i.Substring(j, 1);
            int tempint;
            if (int.TryParse(sub, out tempint))
            {
                resString += sub;
            }
        }
        return int.Parse(resString);
    }
}

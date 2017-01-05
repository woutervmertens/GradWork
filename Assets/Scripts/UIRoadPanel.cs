using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRoadPanel : MonoBehaviour
{
    private Connection _selected;
    public int NrOfLanes;
    public float MaxSpeed;
    public float LaneWidth;

    public InputField LanesIn;
    public InputField WidthIn;
    public InputField SpeedIn;
    // Use this for initialization
    void Start () {
		_selected = MainManager.Main.LastSelectedGameObject.GetComponent<Connection>();
        NrOfLanes = _selected.NrOfLanes;
        MaxSpeed = _selected.MaxSpeed;
        LaneWidth = _selected.LaneWidth;
    }

    public void Close()
    {
        _selected.NrOfLanes = NrOfLanes;
        _selected.MaxSpeed = MaxSpeed;
        _selected.LaneWidth = LaneWidth;

        transform.parent.gameObject.SetActive(false);
    }

    public void Show()
    {
        LanesIn.text = NrOfLanes.ToString();
        WidthIn.text = LaneWidth.ToString();
        SpeedIn.text = MaxSpeed.ToString();
    }

    public void ChangeLaneWidth(string i)
    {
        i = WidthIn.text;
        float b = StripNonFloats(i);
        if (b > 0) LaneWidth = b;
    }

    public void ChangeMaxSpeed(string i)
    {
        i = SpeedIn.text;
        float b = StripNonFloats(i);
        if (b > 0) MaxSpeed = b;
    }

    public void ChangeNrLanes(string i)
    {
        i = LanesIn.text;
        int y = StripNonInts(i);
        if (y > 0) NrOfLanes = y;
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

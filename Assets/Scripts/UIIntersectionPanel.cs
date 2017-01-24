using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIIntersectionPanel : MonoBehaviour
{

    private IntersectionNode _selected;
    private float _lightSwitchRate;
    //private float _speedLimit;

    public InputField SwitchIn;
    //public InputField SpeedIn;

    void Start()
    {
        _selected = MainManager.Main.LastSelectedGameObject.GetComponent<IntersectionNode>();
        _lightSwitchRate = _selected.LightSwitchingRate;
        //_speedLimit = _selected.SpeedLimit;
    }

    public void Show()
    {
        Start();
        SwitchIn.text = _lightSwitchRate.ToString();
        SwitchIn.placeholder.GetComponent<Text>().text = _lightSwitchRate.ToString();
        //SpeedIn.text = _speedLimit.ToString();
    }

    public void Close()
    {
        _selected.LightSwitchingRate = _lightSwitchRate;
        //_selected.SpeedLimit = _speedLimit;
        transform.parent.gameObject.SetActive(false);
    }

    public void ChangeLightRate(string i)
    {
        i = SwitchIn.text;
        float b = StripNonFloats(i);
        if (b > 0) _lightSwitchRate = b;
    }

    //public void ChangeSpeedLimit(string i)
    //{
    //    i = SpeedIn.text;
    //    float b = StripNonFloats(i);
    //    if (b > 0) _speedLimit = b;
    //}

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
}

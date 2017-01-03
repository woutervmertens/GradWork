using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRoadPanel : MonoBehaviour
{
    private Connection _selected;
    public int NrOfLanes;
    public float MaxSpeed;
    public float LaneWidth;
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
    }
}

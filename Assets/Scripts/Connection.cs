using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Connection : MonoBehaviour
{
    public GameObject ConnectionNodePrefab;
    public int NrOfLanes = 1;
    public float MaxSpeed = 50;
    public float LaneWidth = 1;

    public int Serial;

    public Nodes Val1 = null;
    public Nodes Val2 = null;

    public List<Vehicle> Vehicles = new List<Vehicle>();

    public List<Transform> nodes = new List<Transform>();

    void Start()
    {
        Serial = MainManager.Main.GetConnectionCount() + 1;
        MainManager.Main.AddConnection(this, Serial);
    }

    public void Add(List<Transform> l)
    {
        nodes = l;//work on this later
    }

    public void Add(Vector3 v)
    {
        GameObject n = Instantiate(ConnectionNodePrefab, v, Quaternion.identity) as GameObject;
        n.transform.parent = this.transform;
        nodes.Add(n.transform);
    }

    public int GetCount()
    {
        return nodes.Count;
    }

    public float GetFScore()
    {
        float length = 0;
        for (int i = 0; i < nodes.Count-1; i++)
        {
            length += Vector3.Distance(nodes[i].position, nodes[i + 1].position);
        }
        return (length/MaxSpeed)/NrOfLanes;
    }

    public DuoNodes GetEndNodes()
    {
        return new DuoNodes(Val1,Val2);
    }

    public void Draw()
    {
        GetComponent<LineRenderer>().numPositions = transform.childCount;
        Vector3 pos = new Vector3();
        for (int i = 0; i < transform.childCount; i++)
        {
            pos = transform.GetChild(i).position;
            pos.y += 0.1f;
            GetComponent<LineRenderer>().SetPosition(i, pos);
            //can't stop linerenderer billboarding: check out https://github.com/geniikw/drawLine
        }
    }
}

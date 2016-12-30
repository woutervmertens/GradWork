using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Connection : MonoBehaviour
{
    public GameObject ConnectionNodePrefab;

    public int Serial;

    public List<Transform> nodes = new List<Transform>();

    void Start()
    {
        Serial = MainManager.Main.GetConnectionCount() + 1;
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

    public void Draw()
    {
        GetComponent<LineRenderer>().numPositions = transform.childCount;
        for (int i = 0; i < transform.childCount; i++)
        {
            GetComponent<LineRenderer>().SetPosition(i, transform.GetChild(i).position);
        }
    }
}

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DuoNodes
{
    public DuoNodes(Nodes v1, Nodes v2)
    {
        Val1 = v1;
        Val2 = v2;
    }
    public Nodes Val1 { get; set; }
    public Nodes Val2 { get; set; }
}

public class MainManager : MonoBehaviour
{

    public static MainManager Main;
    public double VehiclesNr { get; set; }

    private ArrayList nodeList = new ArrayList();
    public ArrayList NodeList
    {
        get { return nodeList; }
    }

    public List<Vehicle> Vehicles = new List<Vehicle>(); 

    private Dictionary<int,Connection> conDic = new Dictionary<int, Connection>();

    private int connectionCount = 0;

    public GameObject LastSelectedGameObject;

    void Awake()
    {
        if (Main == null)
        {
            DontDestroyOnLoad(gameObject);
            Main = this;
        }
        else if (Main != this)
        {
            Destroy(gameObject);
        }
    }

    public void AddNode<T>(T node)
    {
        nodeList.Add(node);
    }

    public void AddConnection(Connection con, int n)
    {
        conDic.Add(n,con);
        connectionCount += con.GetCount();
    }

    public void RemoveConnection(int n)
    {
        conDic.Remove(n);
    }

    public void RemoveNode<T>(T node)
    {
        
    }

    public Connection GetCon(int n)
    {
        return conDic[n];
    }

    public int GetConnectionCount()
    {
        return conDic.Count;
    }

    public int GetNodeCount()
    {
        return connectionCount;
    }
}

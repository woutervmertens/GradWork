using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.WSA;
using Random = UnityEngine.Random;

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
    public int VehiclesNr { get { return Vehicles.Count; } }
    public int MaxVehicles = 1;
    public float MaxDeltaTime = 0.1f;

    private bool _isEditMode = false;

    private ArrayList nodeList = new ArrayList();
    public ArrayList NodeList
    {
        get { return nodeList; }
    }

    public List<Vehicle> Vehicles = new List<Vehicle>();
    private List<SpawnerNode> spawners = new List<SpawnerNode>();

    private Dictionary<int,Connection> conDic = new Dictionary<int, Connection>();

    private int connectionCount = 0;

    public GameObject LastSelectedGameObject = null;

    public float GeneralSpawnrate = 5;

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

    public void ChangeMode(bool isEdit)
    {
        _isEditMode = isEdit;
        if (!isEdit) DeleteVehicles();
    }

    private void DeleteVehicles()
    {
        foreach (var veh in Vehicles)
        {
            Destroy(veh.gameObject);
        }
        Vehicles.Clear();
        foreach (var s in spawners)
        {
            s.ClearSpawn();
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

    public void AddSpawner(SpawnerNode s)
    {
        spawners.Add(s);
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

    public SpawnerNode GetRandomSpawner(SpawnerNode n)
    {
        SpawnerNode r;
        do
        {
            int rand = Random.Range(0, spawners.Count);
            r = spawners[rand];
        } while (r == n);
        return r;
    }

    void Update()
    {
        if (_isEditMode)
        {
            while (VehiclesNr < MaxVehicles && Time.deltaTime < MaxDeltaTime)
            {
                int rand = Random.Range(0, spawners.Count);
                spawners[rand].AddVehicle();
                Debug.Log("Vehicle added");
            }
        }
    }
}

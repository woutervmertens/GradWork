using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainManager : MonoBehaviour
{

    public static MainManager Main;
    public double Vehicles { get; set; }

    private ArrayList spawnerNodeList = new ArrayList();
    public ArrayList SpawnerNodeList
    {
        get { return spawnerNodeList; }
    }

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
        spawnerNodeList.Add(node);
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

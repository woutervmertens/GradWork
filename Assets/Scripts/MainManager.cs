using UnityEngine;
using System.Collections;

public class MainManager : MonoBehaviour
{

    public static MainManager Main;
    public double Vehicles { get; set; }

    private ArrayList nodeList = new ArrayList();
    public ArrayList NodeList
    {
        get { return nodeList; }
    }

    private ArrayList connectionList = new ArrayList();
    public ArrayList ConnectionList { get { return connectionList;} }

    private int connectionCount = 0;

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

    public void AddConnection(Connection con)
    {
        connectionList.Add(con);
        connectionCount += con.GetCount();
    }

    public int GetConnectionCount()
    {
        return connectionCount;
    }
}

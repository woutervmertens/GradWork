using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum MODE
{
    CREATE,
    EDIT
}

public enum NODE
{
    SPAWNER,
    INTERSECTION,
    CONNECTION
}

public class ModeSelect : MonoBehaviour
{
    public GameObject SpawnerPrefab;
    public GameObject IntersectionPrefab;
    public GameObject ConnectionNodePrefab;

    public Button BtnToggleMode;

    private MODE mode = MODE.CREATE;
    public MODE Mode { get { return mode; } }

    private NODE node = NODE.SPAWNER;

    private ArrayList Connection = new ArrayList();

    // Use this for initialization
    void Start () {
        BtnToggleMode.GetComponent<Text>().text = "Edit";
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonUp(0) && mode == MODE.CREATE)
        {
            var screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(screenRay, out hitInfo))
            {
                switch (node)
                {
                    case NODE.SPAWNER:
                        //Create new spawner and add to main nodes list
                        GameObject s = Instantiate(SpawnerPrefab, hitInfo.point, Quaternion.identity) as GameObject;
                        MainManager.Main.AddNode(s);
                        break;
                    case NODE.INTERSECTION:
                        //Create new intersection and add to main nodes list
                        GameObject i = Instantiate(IntersectionPrefab, hitInfo.point, Quaternion.identity) as GameObject;
                        MainManager.Main.AddNode(i);
                        break;
                    case NODE.CONNECTION:
                        //Create new connection node and add to local Connectionlist
                        GameObject c = Instantiate(ConnectionNodePrefab,hitInfo.point,Quaternion.identity) as GameObject;
                        Connection.Add(c);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        if (Input.GetMouseButtonUp(1) && mode == MODE.CREATE && node == NODE.CONNECTION)
        {
            //Add local connectionlist to main connectionlist and clear local
            Connection con = new Connection();
            con.Add(Connection);
            MainManager.Main.AddConnection(con);
            Connection.Clear();
        }
    }

    public void BtnClick()
    {
        if (mode == MODE.CREATE)
        {
            BtnToggleMode.GetComponent<Text>().text = "Create";
            mode = MODE.EDIT;
        }
        else
        {
            BtnToggleMode.GetComponent<Text>().text = "Edit";
            mode = MODE.CREATE;
        }
    }

    
}

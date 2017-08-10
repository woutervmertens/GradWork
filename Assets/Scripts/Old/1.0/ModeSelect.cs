using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
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
    public GameObject ConnectionParentPrefab;

    public Camera FlyCam;
    public Camera DrawCam;

    private GameObject selectedObject = null;

    public RectTransform EditBoxesParent;
    public RectTransform EditCanvas;
    public RectTransform CreateCanvas;
    public RectTransform SimStopCanvas;
    public RectTransform SimCanvas;
    public RectTransform MainPanelCanvas;

    public Button BtnToggleMode;
    public Button BtnSpawner;
    public Button BtnIntersection;
    public Button BtnConnection;
    public Button BtnToggleSim;
    public Button BtnMainPanel;

    

    private MODE mode = MODE.CREATE;
    public MODE Mode { get { return mode; } }

    private NODE node = NODE.SPAWNER;

    private ArrayList Connection = new ArrayList();

    private bool _isDrawingRoad = false;
    private GameObject _lastRoadParent = null;

    // Use this for initialization
    void Start () {
        BtnToggleMode.GetComponentInChildren<Text>().text = "Edit";
        FlyCam.gameObject.SetActive(false);
        DrawCam.gameObject.SetActive(true);
        CloseAllUIBoxes();
    }
	
	// Update is called once per frame
	void Update () {
	    if (mode == MODE.CREATE)
	    {
	        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
	        {
	            var screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
	            RaycastHit hitInfo;
                int layerCast = 1 << 8;
                if (Physics.Raycast(screenRay, out hitInfo,Mathf.Infinity, layerCast))
	            {
	                switch (node)
	                {
	                    case NODE.SPAWNER:
                            if(_isDrawingRoad) AddRoad();
	                        //Create new spawner and add to main nodes list
	                        GameObject s = Instantiate(SpawnerPrefab, hitInfo.point, Quaternion.identity) as GameObject;
	                        s.transform.parent = this.transform;
	                        //MainManager.Main.AddNode(s);
	                        break;
	                    case NODE.INTERSECTION:
                            if(_isDrawingRoad) AddRoad();
	                        //Create new intersection and add to main nodes list
	                        GameObject i =
	                            Instantiate(IntersectionPrefab, hitInfo.point, Quaternion.identity) as GameObject;
	                        i.transform.parent = this.transform;
	                        //MainManager.Main.AddNode(i);
	                        break;
	                    case NODE.CONNECTION:
	                        //Create new connection node and add to local Connectionlist
	                        if (!_isDrawingRoad)
	                        {
	                            GameObject c =
	                                Instantiate(ConnectionParentPrefab, hitInfo.point, Quaternion.identity) as GameObject;
	                            c.transform.parent = this.transform;
	                            _lastRoadParent = c;
	                            c.GetComponent<Connection>().Add(hitInfo.point + new Vector3(0, 0.5f, 0));
	                            _isDrawingRoad = true;
	                        }
	                        else
	                        {
	                            _lastRoadParent.GetComponent<Connection>().Add(hitInfo.point + new Vector3(0, 0.5f, 0));
	                        }
	                        break;
	                    default:
	                        throw new ArgumentOutOfRangeException();
	                }
	            }
	        }
	        if (Input.GetMouseButtonUp(1) && node == NODE.CONNECTION)
	        {
                AddRoad();
	            GameObject[] delete = GameObject.FindGameObjectsWithTag("RoadDraw");
	            int deleteCount = delete.Length;
	            for (int i = deleteCount - 1; i >= 0; i--)
	            {
	                Destroy(delete[i]);
	            }
	        }
	    }
        else if (mode == MODE.EDIT)
	    {
	        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
	        {
                var screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
	            //int layerCast = 1 << 8;
	            //layerCast = ~layerCast;
	            if (Physics.Raycast(screenRay, out hitInfo/*, layerCast*/))
	            {
	                if (hitInfo.collider.gameObject.GetComponent<IntersectionNode>() != null)
	                {
                        Debug.Log("IntersectionPrefab selected");
	                    MainManager.Main.LastSelectedGameObject = hitInfo.collider.gameObject;
                        CloseAllUIBoxes();
                        OpenUIBoxPos(0);
	                }
	                else if (hitInfo.collider.gameObject.GetComponent<SpawnerNode>() != null)
	                {
                        Debug.Log("SpawnerPrefab selected");
                        MainManager.Main.LastSelectedGameObject = hitInfo.collider.gameObject;
                        CloseAllUIBoxes();
                        OpenUIBoxPos(1);
	                }
	                else if(hitInfo.collider.gameObject.GetComponent<ConnectionNodeScript>() != null)
	                {
                        Debug.Log("ConnectionNodePrefab selected");
                        MainManager.Main.LastSelectedGameObject = hitInfo.collider.transform.parent.gameObject;
                        CloseAllUIBoxes();
                        OpenUIBoxPos(2);
	                }
                }
	        }
	    }

	}


    private void AddRoad()
    {
        _lastRoadParent.GetComponent<Connection>().Draw();
        _isDrawingRoad = false;
    }

    private void CloseAllUIBoxes()
    {
        foreach (Transform uibox in EditBoxesParent.transform)
        {
            uibox.gameObject.SetActive(false);
        }
        EditBoxesParent.gameObject.SetActive(false);
    }

    private void OpenUIBoxPos(int index)
    {
        Debug.Log(EditBoxesParent.transform.childCount);
        EditBoxesParent.gameObject.SetActive(true);
        Vector3 offsetpos = Input.mousePosition;
        offsetpos.x += EditBoxesParent.rect.width/2;
        offsetpos.y += EditBoxesParent.rect.height/2;
        EditBoxesParent.position = offsetpos;
        var child = EditBoxesParent.transform.GetChild(index);
        child.gameObject.SetActive(true);
        switch (index)
        {
            case 0:
                child.GetComponent<UIIntersectionPanel>().Show();
                break;
            case 1:
                child.GetComponent<UISpawnerPanel>().Show();
                break;
            case 2:
                child.GetComponent<UIRoadPanel>().Show();
                break;
            default:
                break;

        }
    }

    public void BtnClick()
    {
        if (mode == MODE.CREATE)
        {
            EditCanvas.gameObject.SetActive(true);
            MainManager.Main.SetSim(false);
            SimCanvas.gameObject.SetActive(false);
            SimStopCanvas.gameObject.SetActive(true);
            CreateCanvas.gameObject.SetActive(false);
            if(node == NODE.CONNECTION && Connection.Count > 1) AddRoad();
            BtnToggleMode.GetComponentInChildren<Text>().text = "Create";
            mode = MODE.EDIT;
            MainManager.Main.ChangeMode(true);
            BtnIntersection.enabled = false;
            BtnConnection.enabled = false;
            BtnSpawner.enabled = false;
            if(_isDrawingRoad) AddRoad();
        }
        else
        {
            EditCanvas.gameObject.SetActive(false);
            CreateCanvas.gameObject.SetActive(true);
            BtnToggleMode.GetComponentInChildren<Text>().text = "Edit";
            mode = MODE.CREATE;
            MainManager.Main.ChangeMode(false);
            BtnIntersection.enabled = true;
            BtnConnection.enabled = true;
            BtnSpawner.enabled = false;
            CloseAllUIBoxes();
        }
        FlyCam.gameObject.SetActive(!FlyCam.gameObject.active);
        DrawCam.gameObject.SetActive(!DrawCam.gameObject.active);
    }

    public void BtnSpawnerClick()
    {
        if (node == NODE.CONNECTION && Connection.Count > 1) AddRoad();
        BtnIntersection.enabled = true;
        BtnConnection.enabled = true;
        BtnSpawner.enabled = false;
        node = NODE.SPAWNER;
    }

    public void BtnIntersectionClick()
    {
        if (node == NODE.CONNECTION && Connection.Count > 1) AddRoad();
        BtnIntersection.enabled = false;
        BtnConnection.enabled = true;
        BtnSpawner.enabled = true;
        node = NODE.INTERSECTION;
    }

    public void BtnConnectionClick()
    {
        BtnIntersection.enabled = true;
        BtnConnection.enabled = false;
        BtnSpawner.enabled = true;
        node = NODE.CONNECTION;
    }

    public void BtnSimClick()
    {
        MainManager.Main.SetSim(!MainManager.Main.IsSimMode);
        SimCanvas.gameObject.SetActive(MainManager.Main.IsSimMode);
        SimStopCanvas.gameObject.SetActive(!MainManager.Main.IsSimMode);
        BtnToggleSim.GetComponentInChildren<Text>().text = (MainManager.Main.IsSimMode)
            ? "Stop Simulation"
            : "Start Simulation";
    }

    public void BtnMainPanelClick()
    {
        MainPanelCanvas.GetComponent<MainPanelScript>().Show(!MainPanelCanvas.gameObject.activeInHierarchy);
        MainPanelCanvas.gameObject.SetActive(!MainPanelCanvas.gameObject.activeInHierarchy);
        MainPanelCanvas.GetComponent<MainPanelScript>().Show(!MainPanelCanvas.gameObject.activeInHierarchy);
    }
}

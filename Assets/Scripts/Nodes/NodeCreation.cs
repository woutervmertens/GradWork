using UnityEngine;
using System.Collections;


public class NodeCreation : MonoBehaviour {

	private ArrayList StreetNodes = new ArrayList();
	
	// Update is called once per frame
	void Update () {
	    if (Input.GetMouseButtonUp(0) && StreetManager.Instance.GlobalState == GLOBALSTATE.CREATE)
	    {
	        var screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
	        RaycastHit hitInfo;
	        if (Physics.Raycast(screenRay, out hitInfo))
	        {
                Debug.Log(hitInfo.point);
                Debug.Log(StreetNodes.Count);
                GetComponent<LineRenderer>().SetVertexCount(StreetNodes.Count+1);
                GetComponent<LineRenderer>().SetPosition(StreetNodes.Count, hitInfo.point);
	            StreetNodes.Add(hitInfo.point);
	        }
	    }
	    if (Input.GetMouseButtonUp(1) && StreetManager.Instance.GlobalState == GLOBALSTATE.CREATE)
	    {
	        StreetManager.Instance.Streets.Add(StreetNodes);
            StreetNodes.Clear();
	    }
	}
}

using UnityEngine;
using System.Collections;

public class Connection : MonoBehaviour
{

    public Node StartNode;
    public Node EndNode;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public Node GetStartNode()
    {
        return StartNode;
    }

    public Node GetEndNode()
    {
        return EndNode;
    }
}

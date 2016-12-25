using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCamScript : MonoBehaviour
{
    public float zoomSpeed = 5f;
    public float moveSpeed = 0.5f;
    private float xMove = 0;
    private float zMove = 0f;
    // Update is called once per frame
    void Update ()
	{
        xMove = 0f;
        zMove = 0f;
	    if (Input.GetKey(KeyCode.KeypadPlus)) GetComponent<Camera>().orthographicSize -= zoomSpeed*Time.deltaTime;
	    if (Input.GetKey(KeyCode.KeypadMinus)) GetComponent<Camera>().orthographicSize += zoomSpeed*Time.deltaTime;
	    if (Input.GetKey(KeyCode.Q)) xMove -= moveSpeed;
	    if (Input.GetKey(KeyCode.D)) xMove += moveSpeed;
	    if (Input.GetKey(KeyCode.S)) zMove -= moveSpeed;
	    if (Input.GetKey(KeyCode.Z)) zMove += moveSpeed;

        transform.Translate(xMove,0,zMove);
	}
}

using System;
using UnityEngine;
using System.Collections;
using UnityEditor;

public enum GLOBALSTATE
{
    CREATE,
    EDIT
}

public class StreetManager : MonoBehaviour
{
    private static StreetManager instance;

    public static StreetManager Instance { get { return instance;} }

    private GLOBALSTATE globalstate = GLOBALSTATE.CREATE;

    public GLOBALSTATE GlobalState { get { return globalstate; } }

    private ArrayList streets = new ArrayList();
    public ArrayList Streets { get { return streets; } }

    private void Awake()
    {
        if (instance != null && instance != this)
        { 
            Destroy(this.gameObject);
        }
        else {
            DontDestroyOnLoad(this);
            instance = this;
        }
    }
}

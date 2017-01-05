using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIIntersectionPanel : MonoBehaviour {

    public void Show()
    {
        
    }

    public void Close()
    {
        transform.parent.gameObject.SetActive(false);
    }
}

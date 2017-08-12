//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.Networking;

//public enum NodeType
//{
//    Intersection,
//    Spawner
//}

//public class Node : MonoBehaviour
//{
//    public Vector3 Position;
//    public int NrVehiclesCont;
//    public NodeType Type;
//    public float Fitness;
//    public float SpeedLimit;
//    public List<Connection> Connections = new List<Connection>();

//    void Start()
//    {
        
//    }

//    void VehicleEntered(Vehicle v)
//    {
//        //Check if light green else stop
//        //Turn and Go Towards Next StartNode
        
//    }

//    void OnTriggerEnter(Collider col)
//    {
//        if (Type == NodeType.Spawner)
//        {
//            if (col.GetType() == typeof(Vehicle))
//            {
//                if (col.gameObject.GetComponent<Vehicle>().GetStartNode() == this.GetComponent<Node>())
//                {
//                    Destroy(col.gameObject);
//                }
//            }
//            else if (col.gameObject.transform.parent.GetType() == typeof(Connection))
//            {
//                bool alreadyExists = false;
//                foreach (var con in Connections)
//                {
//                    //if (con.GetStartNode() == col.gameObject.GetComponent<Node>() ||
//                    //    con.GetEndNode() == col.gameObject.GetComponent<Node>())
//                    //{
//                    //    alreadyExists = true;
//                    //    continue;
//                    //}
//                }
//                if (alreadyExists)
//                {
//                    Connections.Add(col.gameObject.transform.parent.GetComponent<Connection>());
//                }
//            }
//        }
//        else if (Type == NodeType.Intersection)
//        {
//            if (col.GetType() == typeof(Vehicle))
//            {
//                VehicleEntered(col.GetComponent<Vehicle>());
//            }
//            else if (col.gameObject.transform.parent.GetType() == typeof(Connection))
//            {
//                bool alreadyExists = false;
//                foreach (var con in Connections)
//                {
//                    //if (con.GetStartNode() == col.gameObject.GetComponent<Node>() ||
//                    //    con.GetEndNode() == col.gameObject.GetComponent<Node>())
//                    //{
//                    //    alreadyExists = true;
//                    //    continue;
//                    //}
//                }
//                if (alreadyExists)
//                {
//                    Connections.Add(col.gameObject.transform.parent.GetComponent<Connection>());
//                }
//            }
//        }
//    }
//}

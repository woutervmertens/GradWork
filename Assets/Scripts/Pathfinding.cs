using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{

    public List<Nodes> Path;

    public Pathfinding(SpawnerNode startNode, SpawnerNode endNode)
    {
        //Clear
        Path.Clear();

        //Temp cont
        LinkedList<Nodes> openList = new LinkedList<Nodes>();
        LinkedList<Nodes> closedList = new LinkedList<Nodes>();

        //Add StartNode to OpenList
        Nodes currNodes = null;
        openList.AddFirst(startNode.GetComponent<Nodes>());

        //while open list is not empty
        while (openList.Count != 0)
        {
            //Get node with lowest F
            float lowestFScore = float.MaxValue;
            foreach (var n in openList)
            {
                if (n.GetComponent<Connection>().GetFScore() < lowestFScore)
                {
                    currNodes = n;
                    lowestFScore = n.GetComponent<Connection>().GetFScore();
                }
            }
            //Pop current off the open list and push it to the closed
            openList.Remove(currNodes);
            closedList.AddFirst(currNodes);

            //retrieve the chosen nodes adjacent nodes
            
        }
    }
}

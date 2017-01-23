using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{

    public LinkedList<Nodes> Path;
    private Nodes _startNodes, _endNodes;
    public int FailSafe = 10000; //cap nr of loops
    private int _failCheck = 0;

    public LinkedList<Nodes> FindPath(SpawnerNode startNode, SpawnerNode endNode)
    {
        //Clear
        Path.Clear();

        //Temp cont
        LinkedList<Nodes> openList = new LinkedList<Nodes>();
        LinkedList<Nodes> closedList = new LinkedList<Nodes>();

        //Add StartNode to OpenList
        _startNodes = startNode.GetComponent<Nodes>();
        _endNodes = endNode.GetComponent<Nodes>();
        Nodes currNodes = null;
        openList.AddFirst(_startNodes);

        //while open list is not empty
        while (openList.Count != 0 && _failCheck < FailSafe)
        {
            ++_failCheck;
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
            List<Nodes> adj = new List<Nodes>();
            List<int> adjNrs = (currNodes.NodeType == Type.Intersection)
                ? currNodes.GetComponent<IntersectionNode>().Connections
                : currNodes.GetComponent<SpawnerNode>().Connections;
            foreach (var i in adjNrs)
            {
                adj.Add((MainManager.Main.GetCon(i).Val1 == currNodes)? MainManager.Main.GetCon(i).Val2 : MainManager.Main.GetCon(i).Val1);
            }

            //Check if any of neighbours is goal
            if (adj.Contains(_endNodes))
            {
                _endNodes.Parent = currNodes;
                openList.Clear();
                break;
            }
            //else go over all the elements
            foreach (var nodese in adj)
            {
                //if node is in closed, ignore it
                if (closedList.Contains(nodese)) { }
                else
                {
                    //if node not in open list, compute score and add it
                    if (!adj.Contains(nodese))
                    {
                        nodese.Parent = currNodes;

                        openList.AddFirst(nodese);
                    }
                }
            }
        }
        if(_failCheck >= FailSafe) return null;
        //reconstruct path
        Path.AddFirst(_endNodes);
        Path.AddFirst(currNodes);
        Nodes nextNodes = currNodes.Parent;
        while (nextNodes != null)
        {
            Path.AddFirst(nextNodes);
            nextNodes = nextNodes.Parent;
        }
        return Path;
    }
}

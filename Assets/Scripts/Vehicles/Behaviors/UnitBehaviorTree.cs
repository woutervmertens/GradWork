using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Behaviors;

public class UnitBehaviorTree : BehaviorTree
{
    //------------------------------------------------------------------
    // CONTENT -- A lot of data is Dummy - You need to assign the correct data to the correct objects (eg Health in Enemy instead of DamageDealt)
    //------------------------------------------------------------------
    //General Data
    //RoadData
    public Connection PathToFollow;
    private int _currentWayPointId = 0;
    public float Speed = 2.0f;
    public float ReachDistance = 1.0f;
    public float RotationSpeed = 5.0f;
    private Vector3 current_pos;
    public int NextConnection;

    //LocalRoadData
    private float _laneWidth;
    private float _overallPreferredLane = 0;
    private float _turnProbability = 0;
    private float _detectionLength;
    private int _currentRoad = -1;
    private float _neighbourDistance;

    //CarData
    private float _width;
    private float _length;
    public int Lane = 0;
    public float LaneChangeSpeed;
    public float _breakSpeed;
    public float _bufferLength;
    private float _accelSpeed;

    //PathData
    private LinkedList<Nodes> PathFound = new LinkedList<Nodes>();
    public List<Nodes> Path = new List<Nodes>();
    public int PathNodeIndex = 1;
    private List<int> RoadPath = new List<int>();
    private int RoadNodeIndex = 0;
    private SpawnerNode startNode;
    private SpawnerNode endNode;

    //PathFinding Data
    private Nodes _startNodes, _endNodes;
    public int FailSafe = 10000; //cap nr of loops
    private int _failCheck = 0;

    //IntersectionData
    public bool IsOnIntersection = false;



    //------------------------------------------------------------------
    // AI BEHAVIOURS
    //------------------------------------------------------------------

    public BehaviorState CheckStartEndNodes()
    {
        if (startNode != null && endNode != null) return BehaviorState.Success;
        return BehaviorState.Running;
    }
    public BehaviorState FollowRoad()//Run parallel to hit detection on child
    {
        if (_currentWayPointId > PathToFollow.nodes.Count - 1)
        {
            _currentWayPointId = 0;
            return BehaviorState.Success;
        }
        if(_currentWayPointId == 0) NextConnection = RoadPath[RoadNodeIndex + 1];
        float distance = Vector3.Distance(PathToFollow.nodes[_currentWayPointId].position,
                transform.position);

        transform.position = Vector3.MoveTowards(transform.position, PathToFollow.nodes[_currentWayPointId].position,
            Time.deltaTime * Speed);


        Vector3 target = PathToFollow.nodes[_currentWayPointId].position;
        var rotation =
               Quaternion.LookRotation(target -
                                    transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * RotationSpeed);

        if (distance <= ReachDistance) ++_currentWayPointId;

        return BehaviorState.Running;
    }

    public BehaviorState PathFinding()//Run once, if failed pick different endnode
    {
        if(Path.Count > 0 && Path[0] == startNode.GetComponent<Nodes>() && Path[Path.Count-1] == endNode.GetComponent<Nodes>()) return BehaviorState.Success;
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
            for (var n = openList.First; n != null; n = n.Next)
            {
                if (n.Value.GetFScore(endNode) < lowestFScore)
                {
                    currNodes = n.Value;
                    lowestFScore = n.Value.GetFScore(endNode);
                }
            }
            //Pop current off the open list and push it to the closed
            openList.Remove(currNodes);
            closedList.AddFirst(currNodes);

            //retrieve the chosen nodes adjacent nodes
            List<Nodes> adj = new List<Nodes>();
            List<int> adjNrs = currNodes.GetConnections();
            foreach (int i in adjNrs)
            {
                adj.Add((MainManager.Main.GetCon(i).Val1 == currNodes) ? MainManager.Main.GetCon(i).Val2 : MainManager.Main.GetCon(i).Val1);
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
                    if (!openList.Contains(nodese))
                    {
                        nodese.Parent = currNodes;

                        openList.AddFirst(nodese);
                    }
                }
            }
        }
        if (_failCheck >= FailSafe) return BehaviorState.Failure;
        //reconstruct path
        PathFound.AddFirst(_endNodes);
        PathFound.AddFirst(currNodes);
        List<Nodes> parentList = new List<Nodes>();
        Nodes nextNodes = currNodes.Parent;
        while (nextNodes != null && !parentList.Contains(nextNodes)) //endless loop nextNodes doesn't run out of parents
        {
            parentList.Add(nextNodes);
            PathFound.AddFirst(nextNodes);
            nextNodes = nextNodes.Parent;
        }
        GetRoadPath();
        return BehaviorState.Success;
    }

    private void GetRoadPath()
    {
        for (var p = PathFound.First; p != null; p = p.Next)//Convert to List
        {
            Path.Add(p.Value);
        }
        for (int i = 0; i < Path.Count; i++)
        {
            foreach (var con in Path[i].GetComponent<Nodes>().GetConnections())
            {
                if (MainManager.Main.GetCon(con).Val1 == _endNodes || MainManager.Main.GetCon(con).Val2 == _endNodes ||
                    MainManager.Main.GetCon(con).Val1 == Path[i + 1] || MainManager.Main.GetCon(con).Val2 == Path[i + 1])
                {
                    RoadPath.Add(con);
                    break;
                }
            }
        }
        PathToFollow = MainManager.Main.GetCon(RoadPath[0]);
        PathToFollow.Vehicles.Add(this.GetComponent<Vehicle>());
        NextConnection = RoadPath[1];
    }
    public bool CheckHitDetection()//if true -> avoid
    {
        if (IsOnIntersection) return false;
        foreach (Vehicle neighbour in MainManager.Main.GetCon(RoadPath[RoadNodeIndex]).Vehicles)
        {
            if(neighbour == null || neighbour == this.GetComponent<Vehicle>()) continue;
            if ((Mathf.Abs(Vector3.Distance(neighbour.transform.position, this.transform.position)) < _detectionLength * Speed) 
                && (neighbour.GetComponent<UnitBehaviorTree>().Lane == Lane))
            {
                _neighbourDistance = Mathf.Abs(Vector3.Distance(neighbour.transform.position, this.transform.position));
                return GetComponent<Vehicle>().RayTest(_detectionLength * Speed);
            }
        }
        return false;
    }

    public bool CheckLeftLane()
    {
        if (Lane > 1)
        {
            Connection con = MainManager.Main.GetCon(RoadPath[RoadNodeIndex]);
            float vehLength = GetComponent<Vehicle>().GetVehicleLength();
            float laneWidth = con.LaneWidth;
            float dist = Mathf.Sqrt((vehLength * vehLength) + (laneWidth * laneWidth));
            List<Transform> nrNBors = new List<Transform>();
            foreach (Vehicle neighbour in con.Vehicles)
            {
                if (dist + _bufferLength > Vector3.Distance(transform.position, neighbour.transform.position))
                    nrNBors.Add(neighbour.transform);
            }
            if(nrNBors.Count > 0) return GetComponent<Vehicle>().CheckLeftLane(nrNBors, laneWidth);
            return true;
        }
        return false;
    }

    public BehaviorState ChangeLaneLeft()
    {
        //move child to lane
        _laneWidth = MainManager.Main.GetCon(RoadPath[RoadNodeIndex]).LaneWidth;
        if (transform.GetChild(0).localPosition.x > (_laneWidth* (Lane + 1) - _laneWidth*0.5))
        {
            Transform child = transform.GetChild(0);
            Vector3 nl = child.localPosition;
            nl.x -= LaneChangeSpeed*Time.deltaTime;
            child.localPosition = nl;
            return BehaviorState.Running;
        }
        Lane--;
        return BehaviorState.Success;
    }

    public bool CheckRightLane()
    {
        if (Lane < MainManager.Main.GetCon(RoadPath[RoadNodeIndex]).NrOfLanes)
        {
            Connection con = MainManager.Main.GetCon(RoadPath[RoadNodeIndex]);
            float vehLength = GetComponent<Vehicle>().GetVehicleLength();
            float laneWidth = con.LaneWidth;
            float dist = Mathf.Sqrt((vehLength * vehLength) + (laneWidth * laneWidth));
            List<Transform> nrNBors = new List<Transform>();
            foreach (Vehicle neighbour in con.Vehicles)
            {
                if (dist + _bufferLength > Vector3.Distance(transform.position, neighbour.transform.position))
                    nrNBors.Add(neighbour.transform);
            }
            if (nrNBors.Count > 0) return GetComponent<Vehicle>().CheckLeftLane(nrNBors, laneWidth);
            return true;
        }
        return false;
    }

    public BehaviorState ChangeLaneRight()
    {
        _laneWidth = MainManager.Main.GetCon(RoadPath[RoadNodeIndex]).LaneWidth;
        if (transform.GetChild(0).localPosition.x < (_laneWidth * (Lane +1) - _laneWidth * 0.5))
        {
            Transform child = transform.GetChild(0);
            Vector3 nl = child.localPosition;
            nl.x += LaneChangeSpeed * Time.deltaTime;
            child.localPosition = nl;
            return BehaviorState.Running;
        }
        Lane++;
        return BehaviorState.Success;
    }

    public BehaviorState SpeedUp()
    {
        if (Speed < (MainManager.Main.GetCon(RoadPath[RoadNodeIndex]).MaxSpeed)*((100-(5*(Lane-1)))/100))
        {
            Speed += _accelSpeed*Time.deltaTime;
            return BehaviorState.Running;
        }
        return BehaviorState.Success;
    }
    public BehaviorState SlowDown()
    {
        //slow down untill ray is false
        Speed -= (_breakSpeed*(_detectionLength*Speed - _neighbourDistance)*Time.deltaTime);
        if(GetComponent<Vehicle>().RayTest((_detectionLength * Speed) + _bufferLength)) return BehaviorState.Running;
        return BehaviorState.Success;
    }

    public BehaviorState Intersection()
    {
        if(IsOnIntersection) return BehaviorState.Running;
        if (Path[PathNodeIndex].GetComponent<IntersectionNode>() == null) return BehaviorState.Success;
        Path[PathNodeIndex].GetComponent<IntersectionNode>().Vehicles.Remove(this.GetComponent<Vehicle>());
        RoadNodeIndex++;
        PathToFollow = MainManager.Main.GetCon(RoadPath[RoadNodeIndex]);
        PathNodeIndex++;
        MainManager.Main.GetCon(RoadPath[RoadNodeIndex]).Vehicles.Add(this.GetComponent<Car>());
        IsOnIntersection = false;
        return BehaviorState.Success;
    }

    public BehaviorState Idle()
    {
        Debug.Log("Idle...");
        return BehaviorState.Running;
    }

    //------------------------------------------------------------------
    // AI SETUP
    //------------------------------------------------------------------

    public Nodes GetStartNode()
    {
        return _startNodes;
    }

    public bool IsNextNextNodeThisone(Nodes n)
    {
        if (Path.Count < PathNodeIndex + 2)
        {
            return false;
        }
        if (Path[PathNodeIndex + 1] == n || Path[PathNodeIndex + 2] == n)
        {
            return true;
        }
        return false;
    }

    public void SetStartAndEnd(SpawnerNode start, SpawnerNode end)
    {
        startNode = start;
        endNode = end;
    }
    void Start ()
    {
        //------------------------------------------------------------------
        // CONTEXT
        //------------------------------------------------------------------

        //Get Attributes
        Speed = transform.GetChild(0).GetComponent<Vehicle>().Speed;
        _accelSpeed = transform.GetChild(0).GetComponent<Vehicle>().AccelSpeed;
        _breakSpeed = transform.GetChild(0).GetComponent<Vehicle>().BreakSpeed;
        _bufferLength = transform.GetChild(0).GetComponent<Vehicle>().BufferLength;
        _length = transform.GetChild(0).GetComponent<Vehicle>().Lenght;
        _width = transform.GetChild(0).GetComponent<Vehicle>().Width;
        LaneChangeSpeed = transform.GetChild(0).GetComponent<Vehicle>().LaneChangeSpeed;

        //------------------------------------------------------------------
        // BEHAVIOUR TREES
        //------------------------------------------------------------------
        
        List <BehaviorComponent> trafficBehavior = new List<BehaviorComponent>()
        {
            new BehaviorAction(CheckStartEndNodes),
            new BehaviorAction(PathFinding),
            new Sequence(new List<BehaviorComponent>
            {
                new BehaviorAction(Intersection),
                new Parallel(new List<BehaviorComponent>
                {
                    new BehaviorAction(FollowRoad),
                    new BehaviorAction(SpeedUp),
                    new Selector(new List<BehaviorComponent>
                    {
                        new Sequence(new List<BehaviorComponent>
                        {
                            new BehaviorConditional(CheckHitDetection),
                            new BehaviorAction(ChangeLaneLeft)
                        }.ToArray()),
                        new BehaviorAction(SlowDown)
                    }.ToArray()),
                    new Sequence(new List<BehaviorComponent>
                    {
                        new BehaviorConditional(CheckRightLane),
                        new BehaviorAction(ChangeLaneRight)
                    }.ToArray())
                }.ToArray(),5)
            }.ToArray())
        };
        Sequence trafficSequence = new Sequence(trafficBehavior.ToArray());
        //Set default
        SetDefaultComposite(trafficSequence);
    }
}

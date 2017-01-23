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

    //PathData
    private LinkedList<Nodes> PathFound = new LinkedList<Nodes>();
    private List<Nodes> Path = new List<Nodes>();
    private int PathNodeIndex = 1;
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
                    if (!adj.Contains(nodese))
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
        Nodes nextNodes = currNodes.Parent;
        while (nextNodes != null)
        {
            PathFound.AddFirst(nextNodes);
            nextNodes = nextNodes.Parent;
        }
        GetRoadPath();
        return BehaviorState.Success;
    }

    private void GetRoadPath()
    {
        foreach (var p in Path)//Convert to List
        {
            Path.Add(p);
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
        NextConnection = RoadPath[1];
    }
    public bool CheckHitDetection()//if true -> avoid
    {
        foreach (Vehicle neighbour in MainManager.Main.GetCon(RoadPath[RoadNodeIndex]).Vehicles)
        {
            if(neighbour == this.GetComponent<Vehicle>()) continue;
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
        if (Lane > 0)
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
        Path[PathNodeIndex].GetComponent<IntersectionNode>().Vehicles.Remove(this.GetComponent<Vehicle>());
        RoadNodeIndex++;
        PathNodeIndex++;
        MainManager.Main.GetCon(RoadPath[RoadNodeIndex]).Vehicles.Add(this.GetComponent<Car>());
        IsOnIntersection = false;
        return BehaviorState.Success;
    }
    //public BehaviorState GoToClick()
    //{
    //    if ((!SelectionOnly || _selectableObj.IsSelected) && Input.GetMouseButtonUp(1))
    //    {
    //        var screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        RaycastHit hitInfo;
    //        if (Physics.Raycast(screenRay, out hitInfo))
    //        {

    //            Debug.Log("Target Selected");
    //            _agent.destination = hitInfo.point;
    //            return BehaviorState.Success;
    //        }
    //    }

    //    return BehaviorState.Running;
    //}

    //public BehaviorState ResetNavMeshContent()
    //{
    //    _agent.destination = transform.position;
    //    return BehaviorState.Success;
    //}

    //public bool FindClosestEnemyInRange()
    //{
    //    float currentDistanceEnemy = float.MaxValue;
    //    _target = null;

    //    foreach (var go in _enemies)
    //    {
    //        if(go == null)
    //            continue;

    //        //If enemy is in guard zone, check distance with current enemy (if any)
    //        if (Vector3.Distance(go.transform.position, GuardPosition.position) < GuardRange)
    //        {
    //            Debug.Log("A target is in our territory!!");
    //            float currentDistance = Vector3.Distance(go.transform.position,
    //                transform.position);
    //            if (_target != null && currentDistance > currentDistanceEnemy)
    //                continue;

    //            _target = go.transform;
    //            currentDistanceEnemy = currentDistance;
    //        }
    //    }

    //    return _target != null;
    //}

    //public BehaviorState GoToTarget()
    //{
    //    Debug.Log("Going to target");

    //    if(_target == null)
    //        return BehaviorState.Failure;

    //    if (Vector3.Distance(transform.position, _target.position) > AttackRange)
    //    {
    //        _agent.destination = _target.position;
    //        return BehaviorState.Running;
    //    }

    //    Debug.Log("Reached Target!!");
    //    return BehaviorState.Success;
    //}

    //public BehaviorState AttackEnemy()
    //{
    //    if (_damageDealt < _damageToDeal)
    //    {
    //        Debug.Log("Attacking Enemy!");
    //        _damageDealt += Damage;
    //        return BehaviorState.Running;
    //    }

    //    Debug.Log("Enemy Death!");
    //    return BehaviorState.Success;
    //}

    public BehaviorState Idle()
    {
        Debug.Log("Idle...");
        return BehaviorState.Running;
    }

    //public BehaviorState ReceivedClickTarget()
    //{
    //    if ((!SelectionOnly || _selectableObj.IsSelected) && Input.GetMouseButtonUp(1))
    //    {
    //        var screenRay = Camera.main.ScreenPointToRay(Input.mousePosition);
    //        RaycastHit hitInfo;
    //        if (Physics.Raycast(screenRay, out hitInfo))
    //        {

    //            Debug.Log("Target Selected");
    //            _clickTarget = hitInfo.point;
    //            return BehaviorState.Success;
    //        }
    //    }

    //    return BehaviorState.Failure;
    //}

    //public BehaviorState FlockToTarget()
    //{
    //    //Security check
    //    if(_clickTarget == null) return BehaviorState.Failure;

    //    //Variables
    //    var friends =
    //        SelectionManager.Instance.SelectableObjects.FindAll(
    //            x => x != this && x.GetComponent<SelectableObject>().IsSelected);
    //    Vector3 separationVector = Vector3.zero;
    //    Vector3 alignmentVector = Vector3.zero;
    //    Vector3 cohesionVector = Vector3.zero;
    //    Vector3 steerVector = Vector3.zero;
    //    float maximumSpeed = 5.0f;
    //    float maximumForce = 2.0f;
    //    float desiredSeperation = 1.5f;
    //    float neighbourDistance = 10.0f;
    //    float targetDistance = 5.0f;
    //    int countSeperation = 0;
    //    int countNeighbour = 0;

    ////--------------------------------------
    //// DATA CALCULATIONS
    ////--------------------------------------
    ////Steer away from all nearby friends
    //foreach (var fr in friends)
    //{
    //    //Calculate distance
    //    float d = Vector3.Distance(this.transform.position, fr.transform.position);

    //    //Check if we need to separate it --- SEPARATION
    //    if (d > 0 && d < desiredSeperation)
    //    {
    //        //Calculate vector pointing away from neighbour
    //        Vector3 diff = this.transform.position - fr.transform.position;
    //        diff.Normalize();
    //        diff /= d;
    //        separationVector += diff;
    //        ++countSeperation;
    //    }

    //    //ALIGNMENT + COHESION
    //    if (d > 0 && d < neighbourDistance)
    //    {
    //        alignmentVector += fr.GetComponent<UnityEngine.AI.NavMeshAgent>().velocity;
    //        cohesionVector += fr.transform.position;
    //        ++countNeighbour;
    //    }
    //}

    //    //Average all vectors if needed
    //    if (countSeperation > 0)
    //        separationVector /= countSeperation;
    //    if (countNeighbour > 0)
    //    {
    //        alignmentVector /= countNeighbour;
    //        cohesionVector /= countNeighbour;
    //    }

    //    //Craig Reynolds Formula
    //    //Seperation
    //    if (separationVector.magnitude > 0)
    //    {
    //        separationVector.Normalize();
    //        separationVector *= maximumSpeed;
    //        separationVector -= _agent.velocity;
    //        Vector3.ClampMagnitude(separationVector, maximumForce);
    //    }

    //    //Alignment
    //    alignmentVector.Normalize();
    //    alignmentVector *= maximumSpeed;
    //    alignmentVector -= _agent.velocity;
    //    Vector3.ClampMagnitude(alignmentVector, maximumForce);

    //    //Steering + Weighting towards target
    //    Vector3 dirVectorCohesion; //Do weighting only if we have cohesion
    //    if (cohesionVector == Vector3.zero)
    //        dirVectorCohesion = _clickTarget.Value - transform.position;
    //    else
    //    {
    //        dirVectorCohesion = _clickTarget.Value - cohesionVector;
    //    }

    //    steerVector = ((_clickTarget.Value - transform.position)*0.75f) + (dirVectorCohesion*0.25f);
    //    steerVector.Normalize();
    //    steerVector *= maximumSpeed;
    //    steerVector -= _agent.velocity;
    //    Vector3.ClampMagnitude(steerVector, maximumForce);

    //    //--------------------------------------
    //    // FINAL
    //    //--------------------------------------
    //    float randAroundZero = 15.0f;
    //    Vector3 randomize = new Vector3(
    //        (Random.value * (randAroundZero * 2)) - randAroundZero,
    //        (Random.value * (randAroundZero * 2)) - randAroundZero,
    //        (Random.value * (randAroundZero * 2)) - randAroundZero);

    //    Vector3 acc = (separationVector*1.5f)
    //                  + (alignmentVector*1.0f)
    //                  + (steerVector*1.0f)
    //                  + (randomize*1.0f);
    //    _agent.destination = _clickTarget.Value;
    //    _agent.velocity += acc*Time.deltaTime;

    //    //Exit condition
    //    if (Vector3.Distance(transform.position, _clickTarget.Value) < targetDistance)
    //    {
    //        Debug.Log("Reached Target");
    //        _clickTarget = null;
    //        return BehaviorState.Success;
    //    }

    //    return BehaviorState.Running;
    //}

    //------------------------------------------------------------------
    // AI SETUP
    //------------------------------------------------------------------

    public Nodes GetTargetNode()
    {
        return _endNodes;
    }
    void Start ()
    {
        //------------------------------------------------------------------
        // CONTEXT
        //------------------------------------------------------------------
        //_agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        //_selectableObj = GetComponent<SelectableObject>();
        //_clickTarget = new Vector3(-50,0,20);

        //Get Attributes


        //------------------------------------------------------------------
        // BEHAVIOUR TREES
        //------------------------------------------------------------------
        List<BehaviorComponent> goToBehaviours = new List<BehaviorComponent>
        {
           //new BehaviorAction(GoToClick) 
        };
        PartialSequence goToSequence = new PartialSequence(goToBehaviours.ToArray());

        List<BehaviorComponent> guardBehaviors = new List<BehaviorComponent>
        {
            //new BehaviorAction(ResetNavMeshContent),
            new Selector(new List<BehaviorComponent>
            {
                new Sequence(new List<BehaviorComponent>
                {
                    //new BehaviorConditional(FindClosestEnemyInRange),
                    //new BehaviorAction(GoToTarget),
                    //new BehaviorAction(AttackEnemy)
                }.ToArray()),
                new BehaviorAction(Idle)
            }.ToArray())
        };
        Sequence guardSequence = new Sequence(guardBehaviors.ToArray());

        //FLOCKING
        List<BehaviorComponent> flockingBehavior = new List<BehaviorComponent>()
        {
            //new BehaviorAction(ResetNavMeshContent),
            new Selector(new List<BehaviorComponent>
            {
                //new BehaviorAction(ReceivedClickTarget),
                //new BehaviorAction(FlockToTarget)
            }.ToArray())
        };
        Sequence flockSequence = new Sequence(flockingBehavior.ToArray());

        //COMBINED
        List<BehaviorComponent> combinedBehavior = new List<BehaviorComponent>()
        {
            //new BehaviorAction(ResetNavMeshContent),
            new Selector(new List<BehaviorComponent>
            {
                new Sequence(new List<BehaviorComponent>
                {
                    //new BehaviorConditional(FindClosestEnemyInRange),
                    //new BehaviorAction(AttackEnemy)
                }.ToArray()),
                //new BehaviorAction(ReceivedClickTarget),
                //new BehaviorAction(FlockToTarget),
                new BehaviorAction(Idle)
            }.ToArray())
        };
        Sequence combinedSequence = new Sequence(combinedBehavior.ToArray());

        //Set default
        SetDefaultComposite(combinedSequence);
    }
}

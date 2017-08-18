using System.Collections.Generic;
using System.Linq.Expressions;
using Assets.Scripts.Behaviors;
using UnityEngine;

namespace Assets.Scripts.Vehicles.Behaviors
{
    public class VehicleBehavior : BehaviorTree
    {
        //GENERAL DATA
        //------------
        //VehicleData
        public VehicleData Data;

        private Vehicle _vehicle = new Vehicle();
        public float Speed;
        public int CurrentLane = 0;

        //PathData
        private List<PathData> _path = new List<PathData>();
        private PathData _currentPath;
        private int _currentPathIndex = 0;
        private float _currentSplinePos = 0;
        private Node _startNode, _endNode;

        //BEHAVIORS
        //---------
        //Selectors
        public bool CheckHitDetection()
        {
            return CheckCloseByNeighbours(0);
        }

        public bool CheckLaneChangingNeed()
        {
            return false;
        }

        public bool CheckNeedLeft()
        {
            return false;
        }

        public bool CheckNeedRight()
        {
            return false;
        }

        public bool CheckLeftLane()
        {
            if (CurrentLane > 1)
            {
                //Connection con = MainManager.Main.GetCon(RoadPath[RoadNodeIndex]);
                //float vehLength = GetComponent<Vehicle>().GetVehicleLength();
                //float laneWidth = con.LaneWidth;
                //float dist = Mathf.Sqrt((vehLength * vehLength) + (laneWidth * laneWidth));
                //List<Transform> nrNBors = new List<Transform>();
                //foreach (Vehicle neighbour in con.Vehicles)
                //{
                //    if (neighbour == null) continue;
                //    if (dist + BufferLength > Vector3.Distance(transform.position, neighbour.transform.position))
                //        nrNBors.Add(neighbour.transform);
                //}
                //if (nrNBors.Count > 0) return GetComponent<Vehicle>().CheckLeftLane(nrNBors, laneWidth);
                return true;
            }
            return false;
        }

        public bool CheckRightLane()
        {
            //if (CurrentLane < MainManager.Main.GetCon(RoadPath[RoadNodeIndex]).NrOfLanes)
            //{
            //    Connection con = MainManager.Main.GetCon(RoadPath[RoadNodeIndex]);
            //    float vehLength = GetComponent<Vehicle>().GetVehicleLength();
            //    float laneWidth = con.LaneWidth;
            //    float dist = Mathf.Sqrt((vehLength * vehLength) + (laneWidth * laneWidth));
            //    List<Transform> nrNBors = new List<Transform>();
            //    foreach (Vehicle neighbour in con.Vehicles)
            //    {
            //        if (neighbour == null) continue;
            //        if (dist + BufferLength > Vector3.Distance(transform.position, neighbour.transform.position))
            //            nrNBors.Add(neighbour.transform);
            //    }
            //    if (nrNBors.Count > 0) return GetComponent<Vehicle>().CheckLeftLane(nrNBors, laneWidth);
            //    return true;
            //}
            return false;
        }

        private bool CheckCloseByNeighbours(float buffer)
        {
            //if (IsOnIntersection) return false;
            //foreach (Vehicle neighbour in MainManager.Main.GetCon(RoadPath[RoadNodeIndex]).Vehicles)
            //{
            //    if (neighbour == null || neighbour == this.GetComponent<Vehicle>()) continue;
            //    if ((Mathf.Abs(Vector3.Distance(neighbour.transform.position, this.transform.position)) < _detectionLength * Speed)
            //        && (neighbour.GetComponent<UnitBehaviorTree>().Lane == CurrentLane))
            //    {
            //        _neighbourDistance = Mathf.Abs(Vector3.Distance(neighbour.transform.position, this.transform.position));
            //        Vector3 fwdPos = transform.TransformPoint(Vector3.forward * (_detectionLength * Speed));
            //        float distFromHereToNB = Vector3.Distance(transform.position, neighbour.transform.position);
            //        float distFromFwdPosToNB = Vector3.Distance(fwdPos, neighbour.transform.position);
            //        return (distFromHereToNB < distFromFwdPosToNB && distFromFwdPosToNB < (_detectionLength * Speed) && Speed > neighbour.Speed);//NB is between this and fwdPos and is slower
            //    }
            //}
            return false;
        }

        public bool CheckIntersectionCommingUp()
        {
            if (_path[_currentPathIndex].EndNode.IsIntersection)
            {
                return true;
            }
            return false;
        }

        public bool NeedsPath()
        {
            if (_path.Count > 0) return false;
            Debug.Log("Path needed!");
            return true;
        }

        public bool IsCurrentPathFinished()
        {
            if ((_currentPath.Direction > 0 && _currentSplinePos >= _currentPath.EndF)
                || (_currentPath.Direction < 0 && _currentSplinePos <= _currentPath.EndF))
            {
                return true;
            }
            return false;
        }

        public bool IsNextPathIndexEnd()
        {
            if (_currentPathIndex >= _path.Count)
                return true;
            return false;
        }

        //Actions
        public BehaviorState GetPath()
        {
            Debug.Log("Getting the path.");
            if (_vehicle.Route.Count > 0)
            {
                if(_vehicle.Path.Count < 1) _vehicle.ConvertRouteToPath();
                _path = _vehicle.Path;
                _currentPath = _path[0];
                _currentPathIndex = 0;
                _currentSplinePos = _currentPath.StartF;
                return BehaviorState.Success;
            }
            else
            {
                return BehaviorState.Running;
            }
        }

        public BehaviorState SwitchToNextPathPart()
        {
            _currentPathIndex++;
            _currentPath = _path[_currentPathIndex];
            _currentSplinePos = _path[_currentPathIndex].StartF;
            return BehaviorState.Success;
        }

        public BehaviorState EndJourney()
        {
            //Destroy(this.gameObject);
            return BehaviorState.Success;
        }

        public BehaviorState FollowRoad()
        {
            Debug.Log("Following.");
            _currentSplinePos += Time.deltaTime*_currentPath.Direction;
            var normalizedSplinePos = _currentSplinePos / _currentPath.Spline.distance;
            transform.position = _currentPath.Spline.GetSplineValue((normalizedSplinePos * Speed) % 1);
            transform.LookAt(_currentPath.Spline.GetSplineValue((normalizedSplinePos * Speed + 0.01f) % 1)); //0.01f = lookat buffer
            return BehaviorState.Running;
        }

        public BehaviorState Intersection()
        {
            return BehaviorState.Running;
        }

        public BehaviorState SpeedUp()
        {
            //if (Speed < (MainManager.Main.GetCon(RoadPath[RoadNodeIndex]).MaxSpeed) * ((100 - (5 * (CurrentLane - 1))) / 100))
            //{
            //    Speed += Acceleration * Time.deltaTime;
            //    return BehaviorState.Running;
            //}
            return BehaviorState.Success;
        }

        public BehaviorState SlowDown()
        {
            //slow down untill ray is false
            //Speed -= (Deceleration * (_detectionLength * Speed - _neighbourDistance) * Time.deltaTime);
            if (CheckCloseByNeighbours(Data.BufferLength)) return BehaviorState.Running;
            return BehaviorState.Success;
        }

        public BehaviorState ChangeLane()
        {
            return BehaviorState.Running;
        }

        public BehaviorState Idle()
        {
            Debug.Log("Idle...");
            return BehaviorState.Running;
        }

        // Use this for initialization
        void Start () {
            //Get Attributes
            _vehicle = GetComponent<Vehicle>();

            //Behavior Tree
            List<BehaviorComponent> vehicleBehavior = new List<BehaviorComponent>()
            {
                new Selector(new List<BehaviorComponent>
                {
                    new Sequence(new List<BehaviorComponent>
                    {
                        new BehaviorConditional(NeedsPath),
                        new BehaviorAction(GetPath)
                    }.ToArray()),
                    new BehaviorAction(FollowRoad)
                }.ToArray())
            };
                
            //Set Default
            Sequence vehicleSequence = new Sequence(vehicleBehavior.ToArray());
            SetDefaultComposite(vehicleSequence);
        }
	
    }
}

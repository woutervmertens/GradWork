using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Assets.Scripts.Behaviors;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Vehicles.Behaviors
{
    public class VehicleBehavior : BehaviorTree
    {
        //GENERAL DATA
        //------------
        //VehicleData
        public VehicleData Data;
        private Vehicle _vehicle;
        public float Speed;
        public int CurrentLane = 0;
        public VehicleType Type = VehicleType.Car;
        private GameObject _vehicleChild;
        public float WantedLane = 0;

        //PathData
        private List<PathData> _path = new List<PathData>();
        private PathData _currentPath;
        private int _currentPathIndex = 0;
        private float _currentSplinePos = 0;
        private float _normalizedSplinePos = 0;
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
            if (RoadManager.DebubMode) Debug.Log("Path needed!");
            return true;
        }

        public bool IsCloseToPathEnd()
        {
            if (_normalizedSplinePos < 0.000000000001) return false;
            float endF = _currentPath.EndF / _currentPath.Spline.distance;
            float currF = (_currentSplinePos / _currentPath.Spline.distance) % 1;
            if (((_currentPath.Direction > 0 && currF >= endF - 0.05)
                 || (_currentPath.Direction < 0 && currF <= endF + 0.05)) && Vector3.Distance(transform.position, _path[_currentPathIndex].EndNode.Pos) < 2.0f)
            {
                return true;
            }
            return false;
        }

        public bool IsCurrentPathFinished()
        {
            if (Vector3.Distance(transform.position, _path[_currentPathIndex].EndNode.Pos) < 2.0f)
            {
                return true;
            }
            return false;
        }

        public bool IsNextPathIndexEnd()
        {
            if (_currentPathIndex >= _path.Count-1)
                return true;
            return false;
        }

        public bool IsRouteEndReached()
        {
            var d = Vector3.Distance(transform.position, _path[_path.Count-1].EndNode.Pos);
            if (d < 2.0f)
            {
                return true;
            }
            return false;
        }

        //Actions
        public BehaviorState SetUpModel()
        {
            if (Type == VehicleType.Car)
            {
                GameObject prefab = Resources.Load<GameObject>("Car").transform.GetChild(0).gameObject;
                _vehicleChild.GetComponent<MeshFilter>().sharedMesh = prefab.GetComponent<MeshFilter>().sharedMesh;
            }
            else if (Type == VehicleType.Jeep)
            {
                GameObject prefab = Resources.Load<GameObject>("Jeep").transform.GetChild(0).gameObject;
                _vehicleChild.GetComponent<MeshFilter>().sharedMesh = prefab.GetComponent<MeshFilter>().sharedMesh;
            }
            else if (Type == VehicleType.Van)
            {
                GameObject prefab = Resources.Load<GameObject>("Van").transform.GetChild(0).gameObject;
                _vehicleChild.GetComponent<MeshFilter>().sharedMesh = prefab.GetComponent<MeshFilter>().sharedMesh;
            }
            var c = new Color(Random.value, Random.value, Random.value, 1);
            _vehicleChild.GetComponent<Renderer>().material.color = c;
            return BehaviorState.Success;
        }

        public BehaviorState GetPath()
        {
            if (RoadManager.DebubMode) Debug.Log("Getting the path.");
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
            if (RoadManager.DebubMode) Debug.Log("Switching pathparts from: " + _path[_currentPathIndex-1].Spline.tRoad.name + " to " + _path[_currentPathIndex].Spline.tRoad.name);
            return BehaviorState.Success;
        }

        public BehaviorState EndJourney()
        {
            RoadManager.NumberOfVehicles--;
            Destroy(this.gameObject);
            return BehaviorState.Success;
        }

        public BehaviorState FollowRoad()
        {
            if (RoadManager.DebubMode) Debug.Log("Following : " + _currentPath.Direction);
            _currentSplinePos += (Time.deltaTime * Speed) * _currentPath.Direction;
             _normalizedSplinePos = _currentSplinePos / _currentPath.Length;
            transform.position = _currentPath.Spline.GetSplineValue(_currentSplinePos / _currentPath.Spline.distance);
            transform.LookAt(_currentPath.Spline.GetSplineValue(((_currentSplinePos / _currentPath.Spline.distance) + 0.01f*_currentPath.Direction))); //0.01f = lookat buffer
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
            if (RoadManager.DebubMode) Debug.Log("Idle...");
            return BehaviorState.Running;
        }

        // Use this for initialization
        void Start () {
            //Get Attributes
            _vehicle = GetComponent<Vehicle>();
            _vehicleChild = transform.GetChild(0).gameObject;
            Type = SelectType();

            //Behavior Tree
            List<BehaviorComponent> vehicleBehavior = new List<BehaviorComponent>()
            {
                new Selector(new List<BehaviorComponent>
                {
                    new Sequence(new List<BehaviorComponent>
                    {
                        new BehaviorConditional(NeedsPath),
                        new BehaviorAction(GetPath),
                        new BehaviorAction(SetUpModel)
                    }.ToArray()),
                    new Sequence(new List<BehaviorComponent>
                    {
                        new BehaviorConditional(IsCloseToPathEnd), //if close to path end, check if route end or just part end
                        new Selector(new List<BehaviorComponent>
                        {
                            new Sequence(new List<BehaviorComponent>
                            {
                                new BehaviorConditional(IsNextPathIndexEnd),
                                new BehaviorConditional(IsRouteEndReached), 
                                new BehaviorAction(EndJourney)
                            }.ToArray()),
                            new Sequence(new List<BehaviorComponent>
                            {
                                new BehaviorConditional(IsCurrentPathFinished),
                                new BehaviorAction(SwitchToNextPathPart)
                            }.ToArray())
                        }.ToArray())
                    }.ToArray()),
                    new BehaviorAction(FollowRoad)
                }.ToArray())
            };
                
            //Set Default
            Sequence vehicleSequence = new Sequence(vehicleBehavior.ToArray());
            SetDefaultComposite(vehicleSequence);
        }

        private VehicleType SelectType()
        {
            Array values = Enum.GetValues(typeof(VehicleType));
            return (VehicleType)values.GetValue(new System.Random().Next(values.Length));
        }
	
    }
}

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
        public float WantedLane = 0;
        private float CurrentLanePos = 2.5f;
        private float _laneWidth = 5.0f;
        public int CurrentLane = 0;
        public VehicleType Type = VehicleType.Car;
        private GameObject _vehicleChild;
        private bool _isChangingLanes = false;
        private bool _isOnIntersection = false;
        

        //PathData
        private List<PathData> _path = new List<PathData>();
        private PathData _currentPath;
        private int _currentPathIndex = 0;
        public float _currentSplinePos = 0;
        private float _normalizedSplinePos = 0;
        //private Node _startNode, _endNode;

        //BEHAVIORS
        //---------
        //Selectors
        public bool CheckHitDetection()
        {
            if (_isOnIntersection) return false;
            if (_currentPath.Direction > 0)
            {
                foreach (var n in _currentPath.RoadData.PosVehicles)
                {
                    if (n == _vehicle) continue;
                    var nF = n.GetFValue();
                    if (nF > _currentSplinePos &&
                        nF < _currentSplinePos + (Data.Length / 2 + Data.BufferLength) &&
                        n.GetCurrentLane() == CurrentLane) return true;
                }
            }
            if (_currentPath.Direction < 0)
            {
                foreach (var n in _currentPath.RoadData.NegVehicles)
                {
                    if (n == _vehicle) continue;
                    var nF = n.GetFValue();
                    if (nF < _currentSplinePos &&
                        nF > _currentSplinePos - (Data.Length / 2 + Data.BufferLength) &&
                        n.GetCurrentLane() == CurrentLane)
                        return true;
                }
            }
            return false;
        }

        public bool CheckSpeedLimitReached()
        {
            if (Speed < _currentPath.RoadData.MaxSpeed) return true;
            return false;
        }

        public bool CheckIsChangingLanes()
        {
            return _isChangingLanes;
        }

        public bool CheckLaneChangingNeed()
        {
            if (_currentPath.RoadData.NrOfLanes == 1) return false;
            if (WantedLane < 0)
            {
                WantedLane = 0;
                return false;
            }
            if (WantedLane > CurrentLane && CurrentLane == _currentPath.RoadData.NrOfLanes - 1)
            {
                WantedLane = CurrentLane;
                return false;
            }
            if (Mathf.Abs(WantedLane - CurrentLane) < _laneWidth/2) return false;
            return true;
        }

        public bool CheckNeedLeft()
        {
            if (WantedLane < CurrentLane) return true;
            return false;
        }

        public bool CheckNeedRight()
        {
            if (WantedLane > CurrentLane) return true;
            return false;
        }

        public bool CheckLeftLane()
        {
            if (_currentPath.Direction > 0)
            {
                foreach (var n in _currentPath.RoadData.PosVehicles)
                {
                    if (n == _vehicle) continue;
                    var nF = n.GetFValue();
                    if (nF > _currentSplinePos - (Data.Length / 2 + Data.BufferLength) &&
                        nF < _currentSplinePos + (Data.Length / 2 + Data.BufferLength) &&
                        n.GetCurrentLane() == CurrentLane - 1) return false;
                }
            }
            if (_currentPath.Direction < 0)
            {
                foreach (var n in _currentPath.RoadData.NegVehicles)
                {
                    if (n == _vehicle) continue;
                    var nF = n.GetFValue();
                    if (nF < _currentSplinePos + (Data.Length / 2 + Data.BufferLength) &&
                        nF > _currentSplinePos - (Data.Length / 2 + Data.BufferLength) &&
                        n.GetCurrentLane() == CurrentLane - 1)
                        return false;
                }
            }
            return true;
        }

        public bool CheckRightLane()
        {
            if (_currentPath.Direction > 0)
            {
                foreach (var n in _currentPath.RoadData.PosVehicles)
                {
                    if (n == _vehicle) continue;
                    var nF = n.GetFValue();
                    if (nF > _currentSplinePos - (Data.Length / 2 + Data.BufferLength) &&
                        nF < _currentSplinePos + (Data.Length / 2 + Data.BufferLength) &&
                        n.GetCurrentLane() == CurrentLane + 1) return false;
                }
            }
            if (_currentPath.Direction < 0)
            {
                foreach (var n in _currentPath.RoadData.NegVehicles)
                {
                    if (n == _vehicle) continue;
                    var nF = n.GetFValue();
                    if (nF < _currentSplinePos + (Data.Length / 2 + Data.BufferLength) &&
                        nF > _currentSplinePos - (Data.Length / 2 + Data.BufferLength) &&
                        n.GetCurrentLane() == CurrentLane + 1)
                        return false;
                }
            }
            return true;
        }

        //private bool CheckCloseByNeighbours(float buffer)
        //{
        //    //if (IsOnIntersection) return false;
        //    //foreach (Vehicle neighbour in MainManager.Main.GetCon(RoadPath[RoadNodeIndex]).Vehicles)
        //    //{
        //    //    if (neighbour == null || neighbour == this.GetComponent<Vehicle>()) continue;
        //    //    if ((Mathf.Abs(Vector3.Distance(neighbour.transform.position, this.transform.position)) < _detectionLength * Speed)
        //    //        && (neighbour.GetComponent<UnitBehaviorTree>().Lane == CurrentLane))
        //    //    {
        //    //        _neighbourDistance = Mathf.Abs(Vector3.Distance(neighbour.transform.position, this.transform.position));
        //    //        Vector3 fwdPos = transform.TransformPoint(Vector3.forward * (_detectionLength * Speed));
        //    //        float distFromHereToNB = Vector3.Distance(transform.position, neighbour.transform.position);
        //    //        float distFromFwdPosToNB = Vector3.Distance(fwdPos, neighbour.transform.position);
        //    //        return (distFromHereToNB < distFromFwdPosToNB && distFromFwdPosToNB < (_detectionLength * Speed) && Speed > neighbour.Speed);//NB is between this and fwdPos and is slower
        //    //    }
        //    //}
        //    return false;
        //}

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

            bool success = RoadManager.VehicleDictionary.TryGetValue(Type,out Data);
            if (!success && RoadManager.DebubMode) Debug.Log("VehicleData fetch failed!");

            CurrentLane = new System.Random().Next(_currentPath.RoadData.NrOfLanes - 1);
            CurrentLanePos = _laneWidth/2 + _laneWidth * CurrentLane;
            WantedLane = CurrentLane;

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
                //Add to vehicle list
                if (_currentPath.Direction > 0) _currentPath.RoadData.PosVehicles.Add(_vehicle);
                else _currentPath.RoadData.NegVehicles.Add(_vehicle);
                //Get lanewidth
                _laneWidth = _currentPath.Spline.RoadWidth / 2;
                return BehaviorState.Success;
            }
            else
            {
                return BehaviorState.Running;
            }
        }

        public BehaviorState SwitchToNextPathPart()
        {
            //Change Index
            _currentPathIndex++;
            //Get CurrentPath
            _currentPath = _path[_currentPathIndex];
            //Get F Pos
            _currentSplinePos = _path[_currentPathIndex].StartF;

            //Get lanewidth
            _laneWidth = _currentPath.Spline.RoadWidth/2;

            //Go out of previous vehicle list and into nextone
            if (_path[_currentPathIndex - 1].Direction > 0)
                _path[_currentPathIndex - 1].RoadData.PosVehicles.Remove(_vehicle);
            else _path[_currentPathIndex - 1].RoadData.NegVehicles.Remove(_vehicle);
            if(_currentPath.Direction > 0) _currentPath.RoadData.PosVehicles.Add(_vehicle);
            else _currentPath.RoadData.NegVehicles.Add(_vehicle);

            if (RoadManager.DebubMode) Debug.Log("Switching pathparts from: " + _path[_currentPathIndex-1].Spline.tRoad.name + " to " + _path[_currentPathIndex].Spline.tRoad.name);
            return BehaviorState.Success;
        }

        public BehaviorState EndJourney()
        {
            //Remove from lists
            RoadManager.NumberOfVehicles--;
            if (_currentPath.Direction > 0) _currentPath.RoadData.PosVehicles.Remove(_vehicle);
            else _currentPath.RoadData.NegVehicles.Remove(_vehicle);
            //Destroy
            Destroy(this.gameObject);
            return BehaviorState.Success;
        }

        public BehaviorState FollowRoad()
        {
            if(_currentPath == null) return BehaviorState.Failure;
            if (RoadManager.DebubMode) Debug.Log("Following : " + _currentPath.Direction);
            _currentSplinePos += (Time.deltaTime * Speed) * _currentPath.Direction;
             _normalizedSplinePos = _currentSplinePos / _currentPath.Length;
            transform.position = _currentPath.Spline.GetSplineValue(_currentSplinePos / _currentPath.Spline.distance);
            transform.LookAt(_currentPath.Spline.GetSplineValue(((_currentSplinePos / _currentPath.Spline.distance) + 0.01f*_currentPath.Direction))); //0.01f = lookat buffer
            Vector3 localModelPos = new Vector3(CurrentLanePos, 0.5f, 0.0f);
            _vehicleChild.transform.localPosition = localModelPos;
            return BehaviorState.Running;
        }

        public BehaviorState Intersection()
        {
            return BehaviorState.Running;
        }

        public BehaviorState SpeedUp()
        {
            if (Speed < Data.MaxSpeed)
            {
                Speed += Data.AccelerationSpeed * Time.deltaTime;
                return BehaviorState.Success;
            }
            return BehaviorState.Success;
        }

        public BehaviorState SlowDown()
        {
            if (Speed <= 0)
            {
                Speed = 0;
                return BehaviorState.Success;
            }
            Speed -= Data.BreakSpeed * Time.deltaTime;
            return BehaviorState.Success;
        }

        public BehaviorState ChangeLaneRight()
        {
            if (!_isChangingLanes)
            {
                CurrentLane++;
                _isChangingLanes = true;
            }
            if (CurrentLanePos >= (CurrentLane * _laneWidth) + _laneWidth/2)
            {
                CurrentLanePos = (CurrentLane * _laneWidth) + _laneWidth / 2;
                _isChangingLanes = false;
                return BehaviorState.Success;
            }
            CurrentLanePos += Data.ChangeSpeed * Time.deltaTime;
            return BehaviorState.Running;
        }

        public BehaviorState ChangeLaneLeft()
        {
            if (!_isChangingLanes)
            {
                CurrentLane--;
                _isChangingLanes = true;
            }
            if (CurrentLanePos <= (CurrentLane * _laneWidth) + _laneWidth / 2)
            {
                CurrentLanePos = (CurrentLane * _laneWidth) + _laneWidth / 2;
                _isChangingLanes = false;
                return BehaviorState.Success;
            }
            CurrentLanePos -= Data.ChangeSpeed * Time.deltaTime;
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
                new Parallel(new List<BehaviorComponent> {
                   new Selector(new List<BehaviorComponent>
                   {
                       new Sequence(new List<BehaviorComponent>//Startup
                       {
                           new BehaviorConditional(NeedsPath),
                           new BehaviorAction(GetPath),
                           new BehaviorAction(SetUpModel)
                       }.ToArray()),
                       new Sequence(new List<BehaviorComponent>//PathChanging and end
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
                       new Selector(new List<BehaviorComponent> //SpeedChanging
                       {
                           new Sequence(new List<BehaviorComponent>
                           {
                               new BehaviorConditional(CheckHitDetection),
                               new BehaviorAction(SlowDown)
                           }.ToArray()),
                           new Sequence(new List<BehaviorComponent>
                           {
                               new BehaviorConditional(CheckSpeedLimitReached),
                               new BehaviorAction(SpeedUp)
                           }.ToArray())
                       }.ToArray())
                   }.ToArray()),
                   new BehaviorAction(FollowRoad) //RoadFollowinConstant
                }.ToArray(),3)
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

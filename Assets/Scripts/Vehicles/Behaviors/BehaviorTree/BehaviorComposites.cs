using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Behaviors
{
    //SELECTOR
    public class Selector : BehaviorComponent
    {
        //Our children
        protected List<BehaviorComponent> ChildrenBehaviours;
        //CONSTRUCTOR
        public Selector(BehaviorComponent[] childrenBehaviours)
        {
            ChildrenBehaviours = new List<BehaviorComponent>(childrenBehaviours);
        }
        //METHODS
        public override BehaviorState Execute()
        {
            //Perform OR like behavior
            //If a behavior fails we continue down the path until one succeeds
            foreach (var child in ChildrenBehaviours)
            {
                CurrentState = child.Execute();
                switch (CurrentState)
                {
                    case BehaviorState.Failure:
                        continue;
                    case BehaviorState.Success:
                        return CurrentState;
                    case BehaviorState.Running:
                        return CurrentState;
                    default:
                        continue;
                }
            }

            return CurrentState = BehaviorState.Failure;
        }
    }

    //SEQUENCE
    public class Sequence : BehaviorComponent
    {
        //Our children
        protected List<BehaviorComponent> ChildrenBehaviours;
        //CONSTRUCTOR
        public Sequence(BehaviorComponent[] childrenBehaviours)
        {
            ChildrenBehaviours = new List<BehaviorComponent>(childrenBehaviours);
        }
        //METHODS
        public override BehaviorState Execute()
        {
            //Perform AND like behavior
            //If a behavior fails we return, if success we continue (until all succeeded), if running
            foreach (var child in ChildrenBehaviours)
            {
                CurrentState = child.Execute();
                switch (CurrentState)
                {
                    case BehaviorState.Failure:
                        return CurrentState;
                    case BehaviorState.Success:
                        continue;
                    case BehaviorState.Running:
                        return CurrentState;
                    default:
                        continue;
                }
            }

            return CurrentState = BehaviorState.Success;
        }
    }

    //PARTIAL SEQUENCE
    public class PartialSequence : BehaviorComponent
    {
        //Our children
        protected List<BehaviorComponent> ChildrenBehaviours;
        private int _currentBehaviorIndex = 0;
        //CONSTRUCTOR
        public PartialSequence(BehaviorComponent[] childrenBehaviours)
        {
            ChildrenBehaviours = new List<BehaviorComponent>(childrenBehaviours);
        }
        //METHODS
        public override BehaviorState Execute()
        {
            while (_currentBehaviorIndex < ChildrenBehaviours.Count)
            {
                CurrentState = ChildrenBehaviours[_currentBehaviorIndex].Execute();
                switch (CurrentState)
                {
                      case BehaviorState.Failure:
                        _currentBehaviorIndex = 0;
                        return CurrentState;
                      case BehaviorState.Success:
                        ++_currentBehaviorIndex;
                        return CurrentState = BehaviorState.Running; //Force continue running
                      case BehaviorState.Running:
                        return CurrentState;
                }
            }

            //If we went over all the behaviors, so all succeeded, sequence is completed
            _currentBehaviorIndex = 0;
            return CurrentState = BehaviorState.Success;
        }
    }
}
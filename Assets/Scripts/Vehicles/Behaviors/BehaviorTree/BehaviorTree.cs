using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Behaviors
{
    public enum BehaviorState
    {
        Failure,
        Success,
        Running
    }

    //Base BehaviorComponent
    public abstract class BehaviorComponent
    {
        protected BehaviorState CurrentState;
        public abstract BehaviorState Execute();
    }

    public class BehaviorTree : MonoBehaviour
    {
        //FIELDS
        public BehaviorState CurrentState { get; private set; }
        private List<BehaviorComponent> _composites;
        private BehaviorComponent _currentComposite;

        //METHODS
        public void SetDefaultComposite(BehaviorComponent composite)
        {
            _currentComposite = composite;
        }

        public void Initialize(BehaviorComponent[] composites)
        {
            _composites = new List<BehaviorComponent>(composites);
        }

        public void Update()
        {
            Execute();
        }
        public BehaviorState Execute()
        {
            if(_currentComposite == null)
                return BehaviorState.Failure;

            return CurrentState = _currentComposite.Execute();
        }
    }
}
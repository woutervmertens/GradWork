using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Behaviors
{
    public delegate bool ConditionalDelegate();
    public class BehaviorConditional : BehaviorComponent
    {
        //PROPERTY
        private ConditionalDelegate Condition { get; set; }
        //METHODS
        public BehaviorConditional(ConditionalDelegate condition)
        {
            Condition = condition;
        }
        public override BehaviorState Execute()
        {
            if(Condition == null)
                return BehaviorState.Failure;

            switch (Condition())
            {
                case true:
                    return BehaviorState.Success;
                case false:
                    return BehaviorState.Failure;
                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}
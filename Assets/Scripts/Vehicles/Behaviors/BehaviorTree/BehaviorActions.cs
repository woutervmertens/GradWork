using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Behaviors
{
    public delegate BehaviorState BehaviorActionDelegate();
    public class BehaviorAction : BehaviorComponent
    {
        //PROPERTIES
        private BehaviorActionDelegate Action { get; set; }

        //METHODS
        public BehaviorAction(BehaviorActionDelegate action)
        {
            Action = action;
        }
        public override BehaviorState Execute()
        {
            if(Action == null)
                return BehaviorState.Failure;

            return CurrentState = Action();
        }
    }
}
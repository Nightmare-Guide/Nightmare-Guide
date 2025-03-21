using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

namespace TheKiwiCoder
{
    public class MoveToPosition : ActionNode
    {
        public float speed = 5;
        public float stoppingDistance = 0.1f;
        public bool updateRotation = true;
        public float acceleration = 40.0f;
        public float tolerance = 1.0f;

        protected override void OnStart()
        {
            context.agent.stoppingDistance = stoppingDistance;
            context.agent.speed = speed;
            context.agent.destination = blackboard.moveToPosition;
            context.agent.updateRotation = updateRotation;
            context.agent.acceleration = acceleration;
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            // 실시간으로 isDetected 값을 갱신하여 처리
            bool isDetected = blackboard.Get<bool>("isDetected");

            if (isDetected)
            {
                // 만약 isDetected가 true라면, 행동을 중단하거나 다른 동작을 실행
                return State.Failure;
            }

            if (context.agent.pathPending)
            {
                return State.Running;
            }

            if (context.agent.remainingDistance < tolerance)
            {
                return State.Success;
            }

            if (context.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
            {
                return State.Failure;
            }

            return State.Running;
        }
    }
}

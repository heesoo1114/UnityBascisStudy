using UnityEngine;

namespace BTVisual.BasicNode
{
    public class DebugNode : ActionNode
    {
        [TextArea]
        public string message;

        protected override void OnStart()
        {
            Debug.Log($"OnStart {message}");
        }

        protected override void OnStop()
        {
            Debug.Log($"OnStop {message}");
        }

        protected override State OnUpdate()
        {
            Debug.Log($"OnUpdate {message}");
            return State.SUCCESS;
        }
    }
}

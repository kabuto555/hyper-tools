using UnityEngine;

namespace HyperTools
{
    public abstract class ControllerBehaviour : MonoBehaviour
    {
        protected abstract string ControllerKey { get; }

        public abstract void Initialize();

        protected virtual void Awake()
        {
            Game.AddController(ControllerKey, this);
        }

        protected virtual void OnDestroy()
        {
            Game.RemoveController(ControllerKey);
        }
    }
}

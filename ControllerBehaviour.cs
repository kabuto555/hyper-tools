using System;
using UnityEngine;

namespace HyperTools
{
    public abstract class ControllerBehaviour : MonoBehaviour
    {
        public abstract Type ControllerInterface { get; }

        public abstract void Initialize();

        protected virtual void Awake()
        {
            Game.AddController(this);
        }

        protected virtual void OnDestroy()
        {
            Game.RemoveController(this);
        }
    }
}

using System;
using UnityEngine;

namespace HyperTools
{
    public abstract class ServiceBehaviour : MonoBehaviour
    {
        public abstract Type ServiceInterface { get; }

        public abstract void Initialize();

        protected virtual void Awake()
        {
            Game.AddService(this);
        }

        protected virtual void OnDestroy()
        {
            Game.RemoveService(this);
        }
    }
}

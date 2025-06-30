using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace HyperTools
{
    public abstract class ServiceBehaviour : MonoBehaviour
    {
        public abstract Type ServiceInterface { get; }

        public abstract UniTask Initialize();

        protected virtual void Awake()
        {
            Game.AddService(this);
        }
    }
}

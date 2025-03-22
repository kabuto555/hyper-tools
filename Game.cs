using System;
using System.Collections.Generic;
using UnityEngine;

namespace HyperTools
{
    public class Game : MonoBehaviour
    {
        [Serializable]
        private class ControllerEntry
        {
            public Type Interface;
            public ControllerBehaviour Controller;
        }

        private static Game Instance { get; set; }
        private static readonly Queue<ControllerEntry> PendingControllers = new();

        [SerializeField] private List<ControllerEntry> Controllers;
        [SerializeField] private AudioController AudioController;

        private readonly Dictionary<Type, ControllerBehaviour> _controllerMappingByType = new();

        public static AudioController Audio => Instance.AudioController;

        private void Awake()
        {
            // If there is an instance, and it's not me, delete myself.

            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Initialize();
        }

        private void Initialize()
        {
            Instance = this;
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(this);
            }

            // Add any additional Systems initialization here:

            // Custom game Systems initializations
            Controllers.AddRange(PendingControllers);
            PendingControllers.Clear();

            foreach (var entry in Controllers)
            {
                AddControllerEntry(entry, false);
            }
        }

        public static void AddController(string key, ControllerBehaviour controller)
        {
            var entry = new ControllerEntry
            {
                Controller = controller,
                Interface = controller.ControllerInterface,
            };

            if (Instance == null)
            {
                PendingControllers.Enqueue(entry);
                return;
            }

            AddControllerEntry(entry, true);
        }

        private static void AddControllerEntry(ControllerEntry entry, bool addToMainControllerList)
        {
            Instance._controllerMappingByType[entry.Interface] = entry.Controller;
            if (addToMainControllerList)
            {
                Instance.Controllers.Add(entry);
            }
            
            entry.Controller.Initialize();
        }

        public static void RemoveController(ControllerBehaviour controller)
        {
            if (Instance == null)
            {
                return;
            }

            Instance._controllerMappingByType.Remove(controller.ControllerInterface);
            Instance.Controllers.RemoveAll(x => x.Interface == controller.ControllerInterface);
        }

        public static T GetController<T>() where T : ControllerBehaviour
        {
            return (T)Instance._controllerMappingByType[typeof(T)];
        }
    }
}

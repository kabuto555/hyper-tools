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
            public string Key;
            public ControllerBehaviour Controller;
        }

        private static Game Instance { get; set; }
        private static readonly Queue<ControllerEntry> PendingControllers = new();

        [SerializeField] private List<ControllerEntry> Controllers;
        [SerializeField] private AudioController AudioController;

        private readonly Dictionary<string, ControllerBehaviour> _controllerMapping = new();

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
                var controller = entry.Controller;
                Instance._controllerMapping[entry.Key] = controller;
                controller.Initialize();
            }
        }

        public static void AddController(string key, ControllerBehaviour controller)
        {
            var entry = new ControllerEntry
            {
                Key = key,
                Controller = controller
            };

            if (Instance == null)
            {
                PendingControllers.Enqueue(entry);
                return;
            }

            Instance._controllerMapping[key] = controller;
            Instance.Controllers.Add(entry);
            controller.Initialize();
        }

        public static void RemoveController(string key)
        {
            if (Instance == null)
            {
                return;
            }

            Instance._controllerMapping.Remove(key);
            Instance.Controllers.RemoveAll(x => x.Key == key);
        }

        public static ControllerBehaviour GetController(string key)
        {
            return Instance._controllerMapping[key];
        }
    }
}

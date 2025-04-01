using System;
using System.Collections.Generic;
using UnityEngine;

namespace HyperTools
{
    public class Game : MonoBehaviour
    {
        [Serializable]
        private class ServiceEntry
        {
            public Type Interface;
            public ServiceBehaviour Service;
        }

        private static Game Instance { get; set; }
        private static readonly Queue<ServiceEntry> PendingServices = new();

        [SerializeField] private List<ServiceEntry> Services;
        [SerializeField] private AudioController AudioController;

        private readonly Dictionary<Type, ServiceBehaviour> _serviceMappingByType = new();

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
            Services.AddRange(PendingServices);
            PendingServices.Clear();

            foreach (var entry in Services)
            {
                AddServiceEntry(entry, false);
            }
        }

        public static void AddService(ServiceBehaviour service)
        {
            var entry = new ServiceEntry
            {
                Service = service,
                Interface = service.ServiceInterface,
            };

            if (Instance == null)
            {
                PendingServices.Enqueue(entry);
                return;
            }

            AddServiceEntry(entry, true);
        }

        private static void AddServiceEntry(ServiceEntry entry, bool addToMainServiceList)
        {
            if (Instance._serviceMappingByType.ContainsKey(entry.Interface))
            {
                // Don't want to add an extra service we already have (let it destroy itself)
                return;
            }
            
            Instance._serviceMappingByType[entry.Interface] = entry.Service;
            if (addToMainServiceList)
            {
                Instance.Services.Add(entry);
            }
            
            entry.Service.Initialize();
        }

        public static void RemoveService(ServiceBehaviour service)
        {
            if (Instance == null)
            {
                return;
            }

            Instance._serviceMappingByType.Remove(service.ServiceInterface);
            Instance.Services.RemoveAll(x => x.Interface == service.ServiceInterface);
        }

        public static T GetService<T>() where T : ServiceBehaviour
        {
            return (T)Instance._serviceMappingByType[typeof(T)];
        }
    }
}

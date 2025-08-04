using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace HyperTools
{
    public class Game : MonoBehaviour
    {
        [Serializable]
        private class ServiceEntry
        {
            public Type Interface;
            [FormerlySerializedAs("Service")] public ServiceBehaviour service;
        }

        private static Game Instance { get; set; }
        private static readonly Queue<ServiceEntry> PendingServices = new();

        [FormerlySerializedAs("Services")] [SerializeField] private List<ServiceEntry> services;
        [FormerlySerializedAs("AudioController")] [SerializeField] private AudioController audioController;

        private readonly Dictionary<Type, ServiceBehaviour> _serviceMappingByType = new();
        private readonly Dictionary<Type, bool> _serviceInitializationMappingByType = new();

        public static AudioController Audio => Instance.audioController;

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
            services.AddRange(PendingServices);
            PendingServices.Clear();

            foreach (var entry in services)
            {
                AddServiceEntry(entry, false);
            }
        }

        public static void AddService(ServiceBehaviour service)
        {
            var entry = new ServiceEntry
            {
                service = service,
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
            
            Instance._serviceMappingByType[entry.Interface] = entry.service;
            if (addToMainServiceList)
            {
                Instance.services.Add(entry);
            }
            
            UniTask.Void(async () =>
            {
                await entry.service.Initialize();
                Instance._serviceInitializationMappingByType[entry.Interface] = true;
            });
        }

        public static void RemoveService(ServiceBehaviour service)
        {
            if (Instance == null)
            {
                return;
            }

            Instance._serviceMappingByType.Remove(service.ServiceInterface);
            Instance.services.RemoveAll(x => x.Interface == service.ServiceInterface);
        }

        public static T GetService<T>() where T : ServiceBehaviour
        {
            return (T)Instance._serviceMappingByType[typeof(T)];
        }

        public static async UniTask<T> WaitForServiceInitialization<T>() where T : ServiceBehaviour
        {
            await UniTask.WaitUntil(() => Instance._serviceInitializationMappingByType.GetValueOrDefault(typeof(T), false));
            
            return GetService<T>();
        }
    }
}

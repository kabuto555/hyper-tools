using System;
using System.Collections.Generic;
using Model;
using Newtonsoft.Json;
using UnityEngine;

namespace HyperTools
{
    public abstract class StaticDataRepository : ServiceBehaviour
    {
        private Dictionary<Type, object> _databasesByType;
        private Dictionary<Type, object> _catalogsByType;
    
        public override void Initialize()
        {
            _databasesByType = new Dictionary<Type, object>();
            _catalogsByType = new Dictionary<Type, object>();

            InitializeDictionariesFromJsonFiles();
        }

        protected virtual void InitializeDictionariesFromJsonFiles()
        {
            // Should be implemented by child classes by calling CreateDictionaryFromJsonFile as needed
        }
    
        protected void CreateDictionaryFromJsonFile<T>(string resourcePath) where T : IDataObject
        {
            var catalog = new List<T>();

            var db = new Dictionary<string, T>();
            var jsonFile = Resources.Load<TextAsset>(resourcePath);
            var items = JsonConvert.DeserializeObject<List<T>>(jsonFile.text);

            foreach (var item in items)
            {
                var key = item.Id;
                if (db.ContainsKey(key))
                {
                    Debug.LogWarning($"Data entry {key} already exists in {resourcePath}, overwriting previous entry");
                }
                else
                {
                    catalog.Add(item);
                }

                db[key] = item;
            }

            _databasesByType[typeof(T)] = db;
            _catalogsByType[typeof(T)] = catalog;
        }

        public T LookUp<T>(string id) where T : IDataObject
        {
            if (TryLookUp<T>(id, out var data))
            {
                return data;
            }

            Debug.LogError($"Unknown {typeof(T).Name} {id}");
            return default;
        }

        public bool TryLookUp<T>(string id, out T data) where T : IDataObject
        {
            data = default;

            if (id == null)
            {
                Debug.LogError("Programmer error: Using null is a stupid key");
                return false;
            }

            if (!_databasesByType.TryGetValue(typeof(T), out var db))
            {
                Debug.LogError($"Unknown DB for {typeof(T).Name}");
                return false;
            }

            if (db is not Dictionary<string, T> castedDb)
            {
                Debug.LogError($"Programmer error: Did not know how to store data");
                return false;
            }

            return castedDb.TryGetValue(id.Trim(), out data);
        }

        public List<T> GetCatalog<T>() where T : IDataObject
        {
            if (_catalogsByType.TryGetValue(typeof(T), out var catalog))
            {
                return (List<T>)catalog;
            }

            Debug.LogError($"Unknown catalog for {typeof(T).Name}");
            return new List<T>();
        }
    }
}
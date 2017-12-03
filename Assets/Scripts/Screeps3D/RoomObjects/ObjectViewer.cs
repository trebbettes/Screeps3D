﻿using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Screeps3D
{
    public class ObjectViewer : BaseSingleton<ObjectViewer>
    {
        [SerializeField] private GameObject[] _prefabs;
        private Dictionary<string, ObjectView> _prototypes = new Dictionary<string, ObjectView>();
        private Dictionary<string, Stack<ObjectView>> _pools = new Dictionary<string, Stack<ObjectView>>();

        private void Start()
        {
            foreach (var prefab in _prefabs)
            {
                var go = Instantiate(prefab);
                go.name = prefab.name;
                var view = go.GetComponent<ObjectView>();
                if (view == null) continue;
                _prototypes[prefab.name] = view;
                go.SetActive(false);
                go.transform.SetParent(transform);
            }
        }

        public ObjectView NewView(RoomObject roomObject)
        {
            var view = GetFromPool(roomObject.Type);
            if (!view)
            {
                view = NewInstance(roomObject.Type);
            }
            return view;
        }

        private ObjectView NewInstance(string type)
        {
            if (!_prototypes.ContainsKey(type))
                return null;

            var go = Instantiate(_prototypes[type].gameObject);
            go.SetActive(true);
            var view = go.GetComponent<ObjectView>();
            return view;
        }

        private ObjectView GetFromPool(string type)
        {
            var pool = GetPool(type);
            if (pool.Count > 0)
            {
                return pool.Pop();
            } else
            {
                return null;
            }
        }

        private Stack<ObjectView> GetPool(string type)
        {
            if (!_pools.ContainsKey(type))
            {
                _pools[type] = new Stack<ObjectView>();
            }
            return _pools[type];
        }

        public void AddToPool(ObjectView objectView)
        {
            var pool = GetPool(objectView.RoomObject.Type);
            pool.Push(objectView);
        }
    }
}
﻿using System;
using UnityEngine;

namespace Common
{
    public class BaseSingleton<T>: MonoBehaviour where T: Component
    {
        [SerializeField] private bool _keepAlive;
        private static T _instance;

        public static T Instance 
        {
            get {
                if (_instance != null) return _instance;
                _instance = FindObjectOfType<T>();
                
                if (_instance != null) return _instance;

                throw new Exception(string.Format("expecting singleton of type {0} in scene", typeof(T)));
            }
        }

        public virtual void Awake ()
        {
            _instance = this as T;
            if (_keepAlive)
                DontDestroyOnLoad(gameObject);
        }
    }
}
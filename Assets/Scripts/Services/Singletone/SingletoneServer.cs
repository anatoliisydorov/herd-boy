using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.Services
{
    public sealed class SingletoneServer
    {
        private static SingletoneServer instance;
        public static SingletoneServer Instance
        {
            get
            {
                if (instance == null)
                    instance = new SingletoneServer();
                return instance;
            }
        }

        private GameObject _singletonesObject;
        private Dictionary<System.Type, object> singletones = new Dictionary<System.Type, object>();

        public SingletoneServer()
        {
            _singletonesObject = new GameObject("Singletones");
            GameObject.DontDestroyOnLoad(_singletonesObject);
        }

        public void Set(object singletone)
        {
            var type = singletone.GetType();
            if (!singletones.ContainsKey(type))
                singletones.Add(type, singletone);
            else UnityEngine.Debug.LogWarning($"------------------------ SingletoneServe.Set -- Singletone with type: {type.ToString()} already exist;");
        }

        public T Get<T>()
        {
            var type = typeof(T);
            object result;

            if (!singletones.ContainsKey(type))
            {
                if ((typeof(MonoBehaviour)).IsAssignableFrom(type))
                    result = _singletonesObject.AddComponent(type);
                else
                    result = Activator.CreateInstance(type);

                singletones.Add(type, result);
            }
            else
            {
                result = singletones[type];
            }

            return (T)result;
        }
    }
}

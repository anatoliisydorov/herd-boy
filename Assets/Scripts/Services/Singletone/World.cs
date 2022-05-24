using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.Services
{
    public class World
    {
        public static World GetWorld()
        {
            return SingletoneServer.Instance.Get<World>();
        }

        public Dictionary<Type, Component> _worldSingleComponents = new Dictionary<Type, Component>();

        public bool AddSingleComponent<T>(T component) where T : Component
        {
            Type type = typeof(T);
            if (_worldSingleComponents.ContainsKey(type))
            {
                return false;
            }

            _worldSingleComponents.Add(type, component);
            return true;
        }

        public bool GetSingleComponent<T>(out T component) where T : Component
        {
            Type type = typeof(T);
            component = null;

            if (!_worldSingleComponents.ContainsKey(type))
            {
                return false;
            }

            component = (T)_worldSingleComponents[type];
            return true;
        }

        public bool RemoveSingleComponent<T>() where T : Component
        {
            Type type = typeof(T);
            if (_worldSingleComponents.ContainsKey(type))
            {
                _worldSingleComponents.Remove(type);
                return true;
            }

            return false;
        }

        public bool RemoveSingleComponent<T>(T component) where T : Component
        {
            Type type = typeof(T);
            if (_worldSingleComponents.ContainsKey(type))
            {
                _worldSingleComponents.Remove(type);
                return true;
            }

            return false;
        }
    }
}
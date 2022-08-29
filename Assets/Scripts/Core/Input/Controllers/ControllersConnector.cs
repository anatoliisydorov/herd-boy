using Dev.Core;
using Dev.Services;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Dev.Input
{
    public interface IController 
    {
        void InitializeController(ControllersConnector connector);
    }

    public class ControllersConnector: MonoBehaviour
    {
        [SerializeField] private List<IController> _controllers = new List<IController>();

        private void Awake()
        {
            if (World.GetWorld().AddSingleComponent(this))
            {
                InitializeControllers();
                return;
            }

            Destroy(gameObject);
        }

        private void InitializeControllers()
        {
            for (int i = 0; i < _controllers.Count; i++)
            {
                _controllers[i].InitializeController(this);
            }
        }

        private void OnDestroy()
        {
            World.GetWorld().RemoveSingleComponent(this);
        }

        public bool GetController<T>(out T controller) where T : IController
        {
            controller = default;

            for (int i = 0; i < _controllers.Count; i++)
            {
                if (_controllers[i] is T)
                {
                    controller = (T)_controllers[i];
                    return true;
                }
            }

            return false;
        }
    }
}
using Dev.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Dev.Movement
{
    public interface INavigatable
    {
        public void BuildPathAndMove();
    }

    public class NavigationManager
    {
        private List<INavigatable> _navigatables = new List<INavigatable>();

        public void AddNavigatable(INavigatable navigatable)
        {
            if (_navigatables.Contains(navigatable)) return;
            _navigatables.Add(navigatable);
        }

        public void RemoveNavigatable(INavigatable navigatable)
        {
            if (!_navigatables.Contains(navigatable)) return;
            _navigatables.Remove(navigatable);
        }

        public void RebuildPaths()
        {
            for (int i = 0; i < _navigatables.Count; i++)
                _navigatables[i].BuildPathAndMove();
        }
    }
}

using Dev.Core;
using UnityEngine;

namespace Dev.Character
{
    public interface ICharacterable
    {
        bool TakeDamage();
        bool Die();
    }

    public class BaseCharacter : SmartUpdate, ICharacterable
    {
        [SerializeField] protected int health;
        [SerializeField] protected Collider _collider;

        public Collider Collider { get => _collider; }

        public virtual bool TakeDamage()
        {
            if (health <= 0) return false;

            health--;
            if (health <= 0) Die();

            return true;
        }

        public virtual bool Die()
        {
            return true;
        }
    }
}
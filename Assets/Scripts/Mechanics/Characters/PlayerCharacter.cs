using Dev.AimableMechanics;
using Dev.Movement;
using Dev.Services;
using UnityEngine;

namespace Dev.Character
{
    [RequireComponent(typeof(BasicMovement))]
    public class PlayerCharacter: BaseCharacter
    {
        [SerializeField] private Sling _sling;

        private BasicMovement _basicMovement;

        public Sling Sling { get => _sling; }
        public BasicMovement BasicMovement { get => _basicMovement; }

        public override void OnAwake()
        {
            base.OnAwake();

            if (!World.GetWorld().AddSingleComponent(this))
            {
                Destroy(gameObject);
                return;
            }

            _basicMovement = GetComponent<BasicMovement>();
        }

        private void OnDestroy()
        {
            World.GetWorld().RemoveSingleComponent(this);
        }
    }
}
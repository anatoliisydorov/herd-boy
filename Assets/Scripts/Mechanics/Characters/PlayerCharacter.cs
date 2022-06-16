using Dev.AimableMechanics;
using Dev.Hands;
using Dev.Movement;
using Dev.Services;
using UnityEngine;

namespace Dev.Character
{
    [RequireComponent(typeof(BasicMovement))]
    public class PlayerCharacter: BaseCharacter
    {
        [SerializeField] private ThrowAndSling _throw;
        [SerializeField] private Carrying _carrying;

        private BasicMovement _basicMovement;

        public ThrowAndSling Throw { get => _throw; }
        public Carrying Carrying { get => _carrying; }
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
            _basicMovement.IsBlocked = false;
        }

        private void OnDestroy()
        {
            World.GetWorld().RemoveSingleComponent(this);
        }
    }
}
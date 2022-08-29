using Dev.Actions;
using Dev.AimableMechanics;
using Dev.Hands;
using Dev.Movement;
using Dev.Services;
using UnityEngine;

namespace Dev.Character
{
    [RequireComponent(typeof(BasicMovement))]
    [RequireComponent(typeof(MovementActions))]
    public class PlayerCharacter: BaseCharacter
    {
        [SerializeField] private ThrowAndSling _throw;
        [SerializeField] private Carring _carring;

        private IMovable _basicMovement;
        private MovementActions _onMoveActions;

        public ThrowAndSling Throw { get => _throw; }
        public Carring Carring { get => _carring; }
        public IMovable BasicMovement { get => _basicMovement; }
        public MovementActions OnMoveActions { get => _onMoveActions; }

        private void Awake()
        {
            if (!World.GetWorld().AddSingleComponent(this))
            {
                Destroy(gameObject);
                return;
            }

            _basicMovement = GetComponent<IMovable>();
            _basicMovement.IsBlocked = false;

            _onMoveActions = GetComponent<MovementActions>();
        }

        private void OnDestroy()
        {
            World.GetWorld().RemoveSingleComponent(this);
        }
    }
}
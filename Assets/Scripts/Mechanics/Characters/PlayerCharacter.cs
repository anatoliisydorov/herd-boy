using Dev.Services;

namespace Dev.Character
{
    public class PlayerCharacter: BaseCharacter
    {
        public override void OnAwake()
        {
            base.OnAwake();

            if (!World.GetWorld().AddSingleComponent(this))
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            World.GetWorld().RemoveSingleComponent(this);
        }
    }
}
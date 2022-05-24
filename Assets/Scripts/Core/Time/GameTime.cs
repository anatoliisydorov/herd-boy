using Dev.Core;
using UnityEngine;

namespace Dev.Time
{
    public class GameTime: SmartFixedUpdate
    {
        public float deltaTime { get; private set; }
        public float fixedDeltaTime { get; private set; }

        public float timeScale { get => UnityEngine.Time.timeScale; set => UnityEngine.Time.timeScale = value; }


        public static float DeltaTime { get => Services.SingletoneServer.Instance.Get<GameTime>().deltaTime; }
        public static float FixedDeltaTime { get => Services.SingletoneServer.Instance.Get<GameTime>().fixedDeltaTime; }

        public static float TimeScale 
        { 
            get => Services.SingletoneServer.Instance.Get<GameTime>().timeScale; 
            set => Services.SingletoneServer.Instance.Get<GameTime>().timeScale = value;
        }

        public override void OnAwake()
        {
            base.OnAwake();
            forceUpdate = true;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            deltaTime = UnityEngine.Time.deltaTime;
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();
            fixedDeltaTime = UnityEngine.Time.fixedDeltaTime;
        }
    }
}
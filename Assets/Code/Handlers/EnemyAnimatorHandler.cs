using Droids.Facade.Animation;
using UnityEngine;
using UnityEngine.U2D;

namespace Droids.Handlers
{
    public sealed class EnemyAnimatorHandler
    {
        public AnimationHandler Idle { get; private set; }
        public AnimationHandler Attack { get; private set; }
        public AnimationHandler Defend { get; private set; }

        public EnemyAnimatorHandler(Animator sprite)
        {
            const string prefix = "Enemy_";

            Idle = new($"{prefix}Idle", sprite);
            Attack = new($"{prefix}Attack", sprite);
            Defend = new($"{prefix}Defend", sprite);
        }
    }
}

using Droids.Facade.Animation;
using UnityEngine;

namespace Droids.Handlers
{
    public sealed class PlayerAnimatorHandler
    {
        public AnimationHandler Idle { get; private set; }
        public AnimationHandler Move { get; private set; }
        public AnimationHandler Fall { get; private set; }
        public AnimationHandler Jump { get; private set; }
        public AnimationHandler WallGrip { get; private set; }
        public AnimationHandler Hung { get; private set; }
        public AnimationHandler Climb { get; private set; }
        public AnimationHandler Slash1 { get; private set; }
        public AnimationHandler Slash2 { get; private set; }
        public AnimationHandler Slash3 { get; private set; }
        public AnimationHandler AirSlash { get; private set; }

        public TriggerParameter Finish { get; private set; }

        public PlayerAnimatorHandler(Animator sprite)
        {
            const string prefix = "Player_";

            Idle = new($"{prefix}Idle", sprite);
            Move = new($"{prefix}Move", sprite);
            Fall = new($"{prefix}Fall", sprite);
            Jump = new($"{prefix}Jump", sprite);
            WallGrip = new($"{prefix}WallGrip", sprite);
            Hung = new($"{prefix}Hung", sprite);
            Climb = new($"{prefix}Climb", sprite);
            Slash1 = new($"{prefix}Slash1", sprite);
            Slash2 = new($"{prefix}Slash2", sprite);
            Slash3 = new($"{prefix}Slash3", sprite);
            AirSlash = new($"{prefix}AirSlash", sprite);

            Finish = new(sprite, "Finish");
        }
    }
}

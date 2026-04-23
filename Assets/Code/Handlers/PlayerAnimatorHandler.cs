using Droids.Facade.Animation;
using UnityEngine;

namespace Droids.Handlers
{
    public sealed class PlayerAnimatorHandler
    {
        public BoolParameter Grounded { get; private set; }
        public BoolParameter Moving { get; private set; }
        public BoolParameter Jumping { get; private set; }
        public BoolParameter WallGrip { get; private set; }
        public BoolParameter Hung { get; private set; }
        public TriggerParameter Climb { get; private set; }

        public IntParameter SkillId { get; private set; }
        public IntParameter Iteration { get; private set; }
        public TriggerParameter Execute { get; private set; }
        public TriggerParameter Finish { get; private set; }

        public PlayerAnimatorHandler(Animator sprite)
        {
            Animator[] animators = new Animator[] { sprite };

            Grounded = new(animators, "Grounded");
            Moving = new(animators, "Moving");
            Jumping = new(animators, "Jump");
            WallGrip = new(animators, "WallGrip");
            Hung = new(animators, "Hung");
            Climb = new(animators, "Climb");

            SkillId = new(animators, "SkillId");
            Iteration = new(animators, "Iteration");
            Execute = new(animators, "Execute");
            Finish = new(animators, "Finish");
        }
    }
}

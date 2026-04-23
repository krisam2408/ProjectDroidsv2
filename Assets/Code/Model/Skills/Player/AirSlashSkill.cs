using Droids.Behaviour;
using Droids.Handlers;
using Droids.Model.DataTransfer;
using System;
using System.Collections;
using UnityEngine;

namespace Droids.Model.Skills.Player
{
    [CreateAssetMenu(fileName = "airSlash.asset", menuName = "Player Skills/Air Slash")]
    public sealed class AirSlashSkill : BasePlayerSkill
    {
        [SerializeField] private int m_damage = 3;
        [SerializeField] private float m_animationTime;

        public override int AnimationId => 2;

        public override bool IsUsable(AttacksState state)
        {
            bool locked = !Context.Skills.Locked;
            bool grounded = !Context.Controller.Collisions.HasFlag(CollisionChecker.Bottom);
            bool input = state.Slash.IsPressed;

            return locked && grounded && input;
        }

        public override IEnumerator Execute()
        {
            Context.Skills.Locked = true;
            Context.Animator.SkillId.Value = AnimationId;
            Context.Animator.Execute.Value = true;

            yield return new WaitForSeconds(m_animationTime * 0.5f);

            DoDamage();
            
            yield return new WaitForSeconds(m_animationTime * 0.5f);

            Context.Skills.Locked = false;
            Context.Controller.CanMove = true;
            Context.Animator.Finish.Value = true;
            Context.Skills.ResetSkill();
        }

        private void DoDamage()
        {
            Square damageBox = EffectArea(Context.transform.position, Context.XFlipped);
            Collider2D[] targets = Physics2D.OverlapAreaAll(damageBox.TopLeft, damageBox.BottomRight, TargetMask);
            foreach(Collider2D target in targets)
            {
                if(target.transform.TryGetComponent(out ITargetBehaviour tar))
                {
                    tar.ReceiveDamage(m_damage, false);
                }
            }
        }
    }
}

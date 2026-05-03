using Droids.Behaviour;
using Droids.Handlers;
using Droids.Model;
using Droids.Model.DataTransfer;
using System;
using System.Collections;
using UnityEngine;

namespace Droids.Skills.Player
{
    [CreateAssetMenu(fileName = "downStab.asset", menuName = "Player Skills/Down Stab")]
    public sealed class DownStabSkill : BasePlayerSkill
    {
        [SerializeField] private int m_damage = 5;
        [SerializeField] private float m_animationTime = 0.25f;

        public override bool IsUsable(AttacksState state)
        {
            bool locked = !Context.Skills.Locked;
            bool grounded = !Context.Controller.Collisions.HasFlag(CollisionChecker.Bottom);
            bool input = state.Slash.IsPressed;
            bool down = Context.Input.Move.Y < 0;

            return locked && grounded && down && input;
        }

        public override IEnumerator Execute()
        {
            Context.Skills.Locked = true;
            Context.Animator.DownStab.Play();

            yield return new WaitForSeconds(m_animationTime * 0.5f);

            DoDamage();
            
            yield return new WaitForSeconds(m_animationTime * 0.5f);

            Context.Skills.Locked = false;
            Context.Controller.CanMove = true;
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

using Droids.Behaviour;
using Droids.Handlers;
using Droids.Model;
using Droids.Model.DataTransfer;
using System;
using System.Collections;
using UnityEngine;

namespace Droids.Skills.Player
{
    [CreateAssetMenu(fileName = "upSlash.asset", menuName = "Player Skills/Up Slash")]
    public sealed class UpSlashSkill : BasePlayerSkill
    {
        [SerializeField] private int m_damage = 3;
        [SerializeField] private float m_delayTime;
        [SerializeField] private float m_animationTime;
        [SerializeField] private float m_height = 1f;

        public override bool IsUsable(AttacksState state)
        {
            bool locked = !Context.Skills.Locked;
            bool grounded = Context.Controller.Collisions.HasFlag(CollisionChecker.Bottom);
            bool up = Context.Input.Move.Y > 0f;
            bool input = state.Slash.IsPressed;

            return locked && grounded && up && input;
        }

        private float m_elapsedTime = 0f;

        public override IEnumerator Execute()
        {
            Context.Skills.Locked = true;
            Context.Animator.UpSlash.Play();
            m_elapsedTime = 0f;

            while(m_elapsedTime < m_delayTime)
            {
                m_elapsedTime += Time.deltaTime;
                yield return null;
            }

            DoDamage();

            while(m_elapsedTime < m_animationTime)
            {
                Context.Controller.Move(m_height * Time.deltaTime * Vector2.up, true);
                m_elapsedTime += Time.deltaTime;
                yield return null;
            }

            Context.AppliedY = 0f;
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

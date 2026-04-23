using Droids.Behaviour;
using Droids.Model.DataTransfer;
using System;
using System.Collections;
using UnityEngine;

namespace Droids.Model.Skills.Player
{
    [CreateAssetMenu(fileName = "tripleSlash.asset", menuName = "Player Skills/Triple Slash")]
    public sealed class TripleSlashSkill : BasePlayerSkill
    {
        [SerializeField] private int m_damage;
        [SerializeField] private float m_animationTime;
        [SerializeField] private float m_resetTime;

        public override int AnimationId => 1;

        private int m_iteration = 1;
        private int Iteration
        {
            get => m_iteration;
            set
            {
                int v = value;
                if (v < 1 || v > 3)
                    v = 1;
                m_iteration = v;
            }
        }

        public override bool IsUsable
        {
            get
            {
                bool locked = !Context.Skills.Locked;
                bool grounded = Context.Controller.Collisions.HasFlag(CollisionChecker.Bottom);
                bool input = Context.Input.Slash.IsPressed;

                return locked && grounded && input;
            }
        }

        private Coroutine m_resetRoutine;

        public override IEnumerator Execute()
        {
            if (m_resetRoutine != null)
                Context.StopCoroutine(m_resetRoutine);

            Context.Skills.Locked = true;
            Context.Controller.CanMove = false;
            Context.Animator.SkillId.Value = AnimationId;
            Context.Animator.Iteration.Value = Iteration;
            Context.Animator.Execute.Value = true;

            Iteration++;

            yield return new WaitForSeconds(m_animationTime * 0.5f);

            DoDamage();
            
            yield return new WaitForSeconds(m_animationTime * 0.5f);

            Context.Skills.Locked = false;
            Context.Controller.CanMove = true;
            Context.Animator.Finish.Value = true;

            m_resetRoutine = Context.StartCoroutine(ResetIteration());
            Context.Skills.ResetSkill();
        }

        private void DoDamage()
        {
            int damage = m_damage * Iteration;

            Square damageBox = EffectArea(Context.transform.position, Context.XFlipped);
            Collider2D[] targets = Physics2D.OverlapAreaAll(damageBox.TopLeft, damageBox.BottomRight, TargetMask);
            foreach(Collider2D target in targets)
            {
                if(target.transform.TryGetComponent(out ITargetBehaviour tar))
                {
                    tar.ReceiveDamage(damage, false);
                }
            }
        }

        private IEnumerator ResetIteration()
        {
            yield return new WaitForSeconds(m_resetTime);
            Iteration = 1;
        }
    }
}

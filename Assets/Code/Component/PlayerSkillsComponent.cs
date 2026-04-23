using Droids.Behaviour;
using Droids.Model.DataTransfer;
using Droids.Model.Skills.Player;
using System.Collections;
using UnityEngine;

namespace Droids.Component
{
    public sealed class PlayerSkillsComponent : MonoBehaviour
    {
        [SerializeField] private BasePlayerSkill[] m_skills;

        private PlayerBehaviour m_context;
        private Coroutine m_resetRoutine;
        private WaitForSeconds m_resetWait = new(0.5f);

        public bool Locked { get; set; }

        public void Initialize(PlayerBehaviour context)
        {
            m_context = context;

            foreach (BasePlayerSkill skill in m_skills)
                skill.Initialize(context);
        }

        public BasePlayerSkill CheckSkills()
        {
            foreach(BasePlayerSkill skill in m_skills)
            {
                if (skill.IsUsable)
                {
                    if (m_resetRoutine != null)
                        StopCoroutine(m_resetRoutine);
                    return skill;
                }
            }

            return null;
        }

        public void ResetSkill() => m_resetRoutine = StartCoroutine(ResetRoutine());
        
        private IEnumerator ResetRoutine()
        {
            yield return m_resetWait;
            m_context.Animator.SkillId.Value = 0;
            m_context.Animator.Iteration.Value = 0;
            m_context.Animator.Execute.Value = false;
            m_context.Animator.Finish.Value = false;
        }

#if UNITY_EDITOR
        [SerializeField] private bool m_showGizmos;
        [SerializeField] private int m_skill;

        private void OnDrawGizmos()
        {
            if (!m_showGizmos)
                return;

            if (m_skill >= m_skills.Length)
                return;

            Gizmos.color = Color.red;

            Square area = m_skills[m_skill].EffectArea(transform.position, false);
            area.DrawGizmo();
        }
#endif
    }
}

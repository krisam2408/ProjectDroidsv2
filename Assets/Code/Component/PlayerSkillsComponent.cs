using Droids.Skills.Player;
using Droids.Behaviour;
using Droids.Handlers;
using Droids.Model.DataTransfer;
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
            AttacksState state = m_context.Input.GetState();

            foreach(BasePlayerSkill skill in m_skills)
            {
                if (skill.IsUsable(state))
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

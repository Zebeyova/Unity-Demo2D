using UnityEngine;

namespace Script.Enemy
{
    public class EnemyAnimationController : MonoBehaviour
    {
        public Animator animator;

        private void Awake()
        {
            CheckComponent();
        }

        private void CheckComponent()
        {
            if (!animator) animator = GetComponent<Animator>();
        }

        public void IdleAnimation()
        {
            animator.SetBool(_idleWalk, false);
        }

        public void WalkAnimation()
        {
            animator.SetBool(_idleWalk, true);
        }

        #region 哈希表

        private readonly int _idleWalk = Animator.StringToHash("IdleWalk");

        #endregion
    }
}
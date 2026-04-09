using UnityEngine;

namespace Script.Enemy
{
    public class EnemyAnimationController : MonoBehaviour
    {
        public Animator animator;
        private bool _isAttacking;

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

        public void AttackAnimation()
        {
            switch (_isAttacking)
            {
                case true:
                    return;
                case false:
                    animator.SetBool(_idleWalk, false);
                    animator.SetBool(_isCompleted, false);
                    animator.SetTrigger(_anyAttack);
                    _isAttacking = true;
                    break;
            }
        }

        #region 动画事件

        public void AttackCompleted()
        {
            _isAttacking = false;
            animator.SetBool(_isCompleted, true);
        }

        #endregion

        #region 哈希表

        private readonly int _isCompleted = Animator.StringToHash("IsCompleted");
        private readonly int _idleWalk = Animator.StringToHash("IdleWalk");
        private readonly int _anyAttack = Animator.StringToHash("AnyAttack");
        private readonly int _anyHurt= Animator.StringToHash("AnyHurt");
        private readonly int _anyDie = Animator.StringToHash("AnyDie");

        #endregion
    }
}
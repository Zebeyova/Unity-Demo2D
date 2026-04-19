using System.Collections;
using UnityEngine;

namespace Script.Enemy
{
    public class EnemyAnimationController : MonoBehaviour
    {
        public Animator animator;
        private Health _enemyHealth;
        private bool _isAttacking;
        private bool _isDestroy; //销毁敌人

        private void Awake() => CheckComponent();

        private void Start()
        {
            _enemyHealth.onTakeDamage.AddListener(HurtAnimation);
            _enemyHealth.onDeath.AddListener(() => DieAnimation(0));
        }

        private void OnDestroy()
        {
            _enemyHealth.onTakeDamage.RemoveListener(HurtAnimation);
            _enemyHealth.onDeath.RemoveListener(() => DieAnimation(0));
        }

        private void CheckComponent()
        {
            animator = GetComponent<Animator>();
            _enemyHealth = GetComponent<Health>();
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

        private void HurtAnimation(float damage, float currentHealth)
        {
            animator.SetBool(_isCompleted, false);
            animator.SetTrigger(_anyHurt);
        }
        private void DieAnimation(float currentHealth)
        {
            if (_isDestroy) return;
            _isDestroy = currentHealth == 0;
            animator.SetBool(_isCompleted, false);
            animator.SetTrigger(_anyDie);
            animator.Update(0f);
            StartCoroutine(DestroyEnemy());
        }
        private IEnumerator DestroyEnemy()
        {
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Die"));
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(stateInfo.length);
            Destroy(gameObject);
        }

        #region 动画事件

        public void AttackCompleted()
        {
            _isAttacking = false;
            animator.SetBool(_isCompleted, true);
        }

        public void HurtCompleted()
        {
            _isAttacking = false;
            animator.SetBool(_isCompleted, true);
        }

        #endregion

        #region 哈希表

        private readonly int _isCompleted = Animator.StringToHash("IsCompleted");
        private readonly int _idleWalk = Animator.StringToHash("IdleWalk");
        private readonly int _anyAttack = Animator.StringToHash("AnyAttack");
        private readonly int _anyHurt = Animator.StringToHash("AnyHurt");
        private readonly int _anyDie = Animator.StringToHash("AnyDie");

        #endregion
    }
}
using System;
using UnityEngine;

namespace Script.Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        private void Awake()
        {
            CheckComponent();
        }

        private void Start()
        {
            _playerHealth.onTakeDamage.AddListener(HurtAnimation);
            _playerHealth.onDeath.AddListener(() => HurtAnimation(0, 0));
        }

        private void OnDestroy()
        {
            _playerHealth.onTakeDamage.RemoveListener(HurtAnimation);
            _playerHealth.onDeath.RemoveListener(() => HurtAnimation(0, 0));
        }

        private void CheckComponent()
        {
            animator = GetComponent<Animator>();
            _playerController = GetComponent<PlayerController>();
            _playerHealth = GetComponent<Health>();
        }

        public void UpdateState(bool isWalking, bool isRunning)
        {
            if (TurnState()) return;
        }

        public void RunAnimation(bool isSliding)
        {
        }

        public void JumpAnimation(bool isJumping)
        {
        }

        public void AttackAnimation(int count)
        {
            if (GetState(_attack1) || GetState(_attack2) || GetState(_skills)) return;
        }

        private void HurtAnimation(float damage, float currentHealth)
        {
            if (_isDestroy) return;
            _isDestroy = currentHealth == 0;
            _playerController.comboCount = 0; //受伤时重置连击数
        }

        public void StartTurn(bool isRunning, Action turnComplete)
        {
            if (GetState(_jump) || GetState(_fall) || GetState(_attack1) || GetState(_attack2)) return;
            _onComplete = turnComplete;

            if (isRunning)
            {
                RunAnimation(false);
            }
            else
            {
            }
        }

        private bool GetState(int stateHash)
        {
            if (!animator) return false;
            return animator.GetCurrentAnimatorStateInfo(0).shortNameHash == stateHash;
        }

        public bool TurnState()
        {
            return GetState(_walkToTurn) || GetState(_runToTurn);
        }

        #region 成员

        private bool _comboRequested; //连击请求
        private Action _onComplete;
        public Animator animator;
        private PlayerController _playerController;
        private Health _playerHealth;
        private bool _isDestroy; //销毁玩家

        #endregion

        #region 哈希表

        private readonly int _walkToTurn = Animator.StringToHash("Walk_Turn");
        private readonly int _runToTurn = Animator.StringToHash("run_Turn");
        private readonly int _jump = Animator.StringToHash("Jump");
        private readonly int _fall = Animator.StringToHash("Fall");
        private readonly int _attack1 = Animator.StringToHash("Attack1");
        private readonly int _attack2 = Animator.StringToHash("Attack2");
        private readonly int _skills = Animator.StringToHash("Skills");

        #endregion
    }
}
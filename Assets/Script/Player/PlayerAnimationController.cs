using System;
using UnityEngine;

namespace Script.Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        public Animator animator;

        private Action _onComplete;
        private PlayerController _playerController;
        private int _attackAnimStartCount;
        private int _attackAnimEndCount;

        private void Awake()
        {
            CheckComponent();
            animator.SetBool(_idleWalk, false);
        }


        private void CheckComponent()
        {
            if (!animator) animator = GetComponent<Animator>();
            if (!_playerController) _playerController = GetComponent<PlayerController>();
        }

        public void UpdateState(bool isWalking, bool isRunning)
        {
            if (InTurnState()) return;
            animator.SetBool(_idleWalk, isWalking && !isRunning);
            animator.SetBool(_walkRun, !isWalking && isRunning);
            animator.SetBool(_idleRun, !isWalking && isRunning);
        }

        public void RunAnimation(bool isSliding)
        {
            if (GetState(_runSlide)) return;
            animator.SetBool(_idleWalk, false);
            animator.SetBool(_walkRun, true);
            animator.SetBool(_idleRun, true);
            animator.SetTrigger(isSliding ? _runSlide : _runTurn);
        }

        public void JumpAnimation(bool isJumping)
        {
            if (isJumping && !GetState(_anyJump) && !GetState(_jumpFall))
                animator.SetBool(_anyJump, true);
            else if (!isJumping) animator.SetBool(_anyJump, false);
        }

        public void AttackAnimation(bool isAttacking, int count)
        {
            if (GetState(_attack1) || GetState(_attack2))
            {
                // 正在攻击，改变count
                animator.SetInteger(_attackCount, count);
            }
            else
            {
                animator.SetBool(_anyAttack, isAttacking);
                animator.SetInteger(_attackCount, count);
            }

            _attackAnimStartCount = count;
        }

        public void StartTurn(bool isRunning, Action turnComplete)
        {
            if (GetState(_anyJump) || GetState(_jumpFall)) return;
            _onComplete = turnComplete;
            animator.SetBool(_isCompleted, false);

            animator.SetBool(_idleWalk, false);
            animator.SetBool(_walkRun, false);
            animator.SetBool(_idleRun, false);

            if (isRunning)
            {
                RunAnimation(false);
            }
            else
            {
                animator.SetBool(_idleWalk, true);
                animator.SetTrigger(_walkTurn);
            }
        }

        private bool GetState(int stateHash)
        {
            if (!animator) return false;
            return animator.GetCurrentAnimatorStateInfo(0).shortNameHash == stateHash;
        }

        public bool InTurnState()
        {
            return GetState(_walkToTurn) || GetState(_runToTurn);
        }

        #region 哈希表

        private readonly int _isCompleted = Animator.StringToHash("IsCompleted");
        private readonly int _idleWalk = Animator.StringToHash("IdleWalk");
        private readonly int _idleRun = Animator.StringToHash("IdleRun");
        private readonly int _walkRun = Animator.StringToHash("WalkRun");
        private readonly int _walkTurn = Animator.StringToHash("WalkTurn");
        private readonly int _runTurn = Animator.StringToHash("RunTurn");
        private readonly int _runSlide = Animator.StringToHash("RunSlide");
        private readonly int _anyJump = Animator.StringToHash("AnyJump");
        private readonly int _jumpFall = Animator.StringToHash("JumpFall");
        private readonly int _anyAttack = Animator.StringToHash("AnyAttack");
        private readonly int _attackCount = Animator.StringToHash("AttackCount");

        //状态表
        private readonly int _walkToTurn = Animator.StringToHash("Walk_Turn");
        private readonly int _runToTurn = Animator.StringToHash("run_Turn");
        private readonly int _attack1 = Animator.StringToHash("Attack1");
        private readonly int _attack2 = Animator.StringToHash("Attack2");

        #endregion

        #region 动画事件注册

        public void TurnComplete()
        {
            _onComplete?.Invoke();
            _onComplete = null;
            animator.SetBool(_isCompleted, true);
        }

        public void JumpComplete()
        {
            animator.SetBool(_anyJump, false);
            animator.SetTrigger(_jumpFall);
        }

        public void FallComplete()
        {
            animator.SetBool(_isCompleted, true);
        }

        public void AttackStart()
        {
            animator.SetBool(_anyAttack, false);
            _attackAnimEndCount = _attackAnimStartCount;
        }

        public void AttackEnd()
        {
            if (_attackAnimEndCount == _attackAnimStartCount)
            {
                animator.SetBool(_isCompleted, true);
                _playerController.inAttacking = false;
                _playerController.comboCount = 0;
            }
            else
            {
                animator.SetBool(_isCompleted, false);
            }
        }

        #endregion
    }
}
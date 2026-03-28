using System;
using UnityEngine;

namespace Script.Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        public Animator animator;
        private Action _onComplete;

        private void Awake()
        {
            CheckComponent();
            animator.SetBool(_isCompleted, true);
            animator.SetBool(_idleWalk, false);
        }

        private void CheckComponent()
        {
            if (!animator) animator = GetComponent<Animator>();
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
            if (GetCurrentState(_runSlide)) return;
            animator.SetBool(_idleWalk, false);
            animator.SetBool(_walkRun, true);
            animator.SetBool(_idleRun, true);
            animator.SetTrigger(isSliding ? _runSlide : _runTurn);
        }

        public void JumpAnimation(bool isJumping)
        {
            if (isJumping && !GetCurrentState(_anyJump) && !GetCurrentState(_jumpFall))
            {
                animator.SetBool(_anyJump, true);
            }
            else if (!isJumping)
            {
                animator.SetBool(_anyJump, false);
            }
        }

        public void StartTurn(bool isRunning, Action turnComplete)
        {
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

        private bool GetCurrentState(int stateHash)
        {
            if (!animator) return false;
            return animator.GetCurrentAnimatorStateInfo(0).shortNameHash == stateHash;
        }

        public bool InTurnState()
        {
            return GetCurrentState(_idleTurn) || GetCurrentState(_walkTurn) || GetCurrentState(_runTurn);
        }

        #region 哈希表

        private readonly int _isCompleted = Animator.StringToHash("IsCompleted");
        private readonly int _idleWalk = Animator.StringToHash("IdleWalk");
        private readonly int _idleRun = Animator.StringToHash("IdleRun");
        private readonly int _walkRun = Animator.StringToHash("WalkRun");
        private readonly int _idleTurn = Animator.StringToHash("IdleTurn");
        private readonly int _walkTurn = Animator.StringToHash("WalkTurn");
        private readonly int _runTurn = Animator.StringToHash("RunTurn");
        private readonly int _runSlide = Animator.StringToHash("RunSlide");
        private readonly int _anyJump = Animator.StringToHash("AnyJump");
        private readonly int _jumpFall = Animator.StringToHash("JumpFall");

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
            animator.SetTrigger(_jumpFall);
            animator.SetBool(_isCompleted, true);
            animator.SetBool(_anyJump, false);
        }

        #endregion
    }
}
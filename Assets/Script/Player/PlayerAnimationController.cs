using UnityEngine;

namespace Script.Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        public Animator animator;
        private SpriteRenderer _spriteRenderer;
        private System.Action _onTurnComplete;

        #region 哈希表

        public int isCompleted = Animator.StringToHash("IsCompleted");

        public int idleWalk = Animator.StringToHash("IdleWalk");
        public int walkRun = Animator.StringToHash("WalkRun");
        public int idleRun = Animator.StringToHash("IdleRun");

        public int idleTurn = Animator.StringToHash("IdleTurn");
        public int walkTurn = Animator.StringToHash("WalkTurn");
        public int runTurn = Animator.StringToHash("RunTurn");
        public int runSlide = Animator.StringToHash("RunSlide");

        #endregion

        private void Awake()
        {
            CheckComponent();
            animator.SetBool(isCompleted, true);
            animator.SetBool(idleWalk, false);
        }

        private void CheckComponent()
        {
            if (!animator) animator = GetComponent<Animator>();
        }

        public void UpdateState(bool isWalking, bool isRunning)
        {
            if (InTurnState()) return;
            animator.SetBool(idleWalk, isWalking && !isRunning);
            animator.SetBool(walkRun, isRunning && isWalking);
            animator.SetBool(idleRun, !isWalking && isRunning);
        }

        public void WalkAnimation()
        {
            animator.SetBool(idleWalk, true);
            animator.SetTrigger(walkTurn);
        }

        public void RunAnimation(bool isSliding)
        {
            if (GetCurrentState(runSlide)) return;
            if (isSliding)
            {
                animator.SetBool(idleWalk, false);
                animator.SetBool(idleRun, true);
                animator.SetBool(walkRun, true);
                animator.SetTrigger(runSlide);
            }
            else
            {
                animator.SetBool(idleWalk, false);
                animator.SetBool(idleRun, true);
                animator.SetBool(walkRun, true);
                animator.SetTrigger(runTurn);
            }
        }

        public void StartTurn(bool isRunning, bool isSliding, System.Action turnComplete)
        {
            _onTurnComplete = turnComplete;
            animator.SetBool(isCompleted, false);

            animator.SetBool(idleWalk, false);
            animator.SetBool(walkRun, false);
            animator.SetBool(idleRun, false);

            if (isRunning) RunAnimation(isSliding);
            else WalkAnimation();
        }

        private bool GetCurrentState(int stateHash)
        {
            if (!animator) return false;
            return animator.GetCurrentAnimatorStateInfo(0).shortNameHash == stateHash;
        }

        private bool InTurnState()
        {
            return (GetCurrentState(idleTurn) || GetCurrentState(walkTurn) || GetCurrentState(runTurn));
        }

        #region 动画事件注册

        public void TurnComplete()
        {
            _onTurnComplete?.Invoke();
            _onTurnComplete = null;
            animator.SetBool(isCompleted, true);
        }

        #endregion
    }
}
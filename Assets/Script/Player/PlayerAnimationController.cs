using UnityEngine;

namespace Script.Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        public Animator animator;
        private SpriteRenderer _spriteRenderer;
        private System.Action _onTurnComplete;

        #region 哈希表

        public int isTurnComplete = Animator.StringToHash("IsTurnComplete");

        public int idleWalk = Animator.StringToHash("IdleWalk");
        public int walkRun = Animator.StringToHash("WalkRun");
        public int idleRun = Animator.StringToHash("IdleRun");

        public int idleTurn = Animator.StringToHash("IdleTurn");
        public int walkTurn = Animator.StringToHash("WalkTurn");
        public int runTurn = Animator.StringToHash("RunTurn");

        #endregion

        private void Awake()
        {
            CheckComponent();
            animator.SetBool(isTurnComplete, true);
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

        public void StartTurn(bool isRunning, System.Action turnComplete)
        {
            _onTurnComplete = turnComplete;
            animator.SetBool(isTurnComplete, false);
            
            animator.SetBool(idleWalk, false);
            animator.SetBool(walkRun, false);
            animator.SetBool(idleRun, false);

            if (isRunning)
            {
                animator.SetBool(idleWalk, false);
                animator.SetBool(idleRun, true);
                animator.SetBool(walkRun, true);
                animator.SetTrigger(runTurn);
            }
            else
            {
                animator.SetBool(idleWalk, true);
                animator.SetTrigger(walkTurn);
            }
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
            animator.SetBool(isTurnComplete, true);
        }

        #endregion
    }
}
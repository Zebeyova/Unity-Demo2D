using UnityEngine;

namespace Script.Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        public Animator animator;
        private SpriteRenderer _spriteRenderer;
        private System.Action _onTurnComplete;

        #region 哈希表

        public int idleTurn = Animator.StringToHash("IdleTurn");
        public int isTurnComplete = Animator.StringToHash("IsTurnComplete");
        public int idleWalk = Animator.StringToHash("IdleWalk");
        public int walkTurn = Animator.StringToHash("WalkTurn");
        public int walkRun = Animator.StringToHash("WalkRun");
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
            animator.SetBool(idleWalk, isWalking && !isRunning);
            animator.SetBool(walkRun, isRunning);
            if (!IsInState(walkTurn) && !IsInState(runTurn))
            {
                if (isWalking)
                {
                    animator.SetBool(idleWalk, true);
                    animator.SetBool(walkRun, false);
                }

                if (isRunning)
                {
                    animator.SetBool(idleWalk, false);
                    animator.SetBool(walkRun, true);
                }
            }
        }

        public void StartTurn(bool isWalking, bool isRunning, System.Action turnComplete)
        {
            _onTurnComplete = turnComplete;
            animator.SetBool(isTurnComplete, false);
            animator.SetBool(idleWalk, true);
            animator.SetTrigger(walkTurn);
        }

        private bool IsInState(int stateHash)
        {
            if (!animator) return false;
            return animator.GetCurrentAnimatorStateInfo(0).shortNameHash == stateHash;
        }

        private bool IsInTurnState()
        {
            return IsInState(idleTurn) || IsInState(walkTurn) || IsInState(runTurn);
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
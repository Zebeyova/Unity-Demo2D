using UnityEngine;

namespace Script.Player
{
    public class PlayerAnimationController : MonoBehaviour
    {
        public System.Action OnTurnComplete;
        public Animator animator;

        #region 哈希表

        public int idleTurn = Animator.StringToHash("IdleTurn");
        public int isTurnComplete = Animator.StringToHash("IsTurnComplete");
        public int idleWalk = Animator.StringToHash("IdleWalk");
        public int walkTurn = Animator.StringToHash("WalkTurn");
        public int walkRun = Animator.StringToHash("WalkRun");

        public int idleState = Animator.StringToHash("Idle");
        public int walkState = Animator.StringToHash("Walk");

        #endregion

        private void Awake()
        {
            CheckComponent();
        }

        private void CheckComponent()
        {
            if (!animator) animator = GetComponent<Animator>();
        }

        public bool IsInState(int stateHash)
        {
            if (!animator) return false;
            return animator.GetCurrentAnimatorStateInfo(0).shortNameHash == stateHash;
        }

        public void SetBool(int id, bool value)
        {
            animator.SetBool(id, value);
        }

        public void SetTrigger(int id)
        {
            animator.SetTrigger(id);
        }

        #region 动画事件注册

        public void TurnComplete()
        {
            OnTurnComplete?.Invoke();
        }

        #endregion
    }
}
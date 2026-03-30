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
    }
}
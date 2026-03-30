using UnityEngine;

namespace Script
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
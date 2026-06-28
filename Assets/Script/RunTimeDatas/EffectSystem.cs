using Script.Views;
using UnityEngine;

namespace Script.RunTimeDatas
{
    public class EffectSystem : MonoBehaviour
    {
        public ParticleSystem enemyExpParticle;
        public GameObject player;

        private void OnEnable()
        {
            Events.EventCenter.OnEnemyDeath += CreateEnemyExpParticle;
        }

        private void Start()
        {
            player = GameObject.FindWithTag("Player");
        }

        private void OnDisable()
        {
            Events.EventCenter.OnEnemyDeath -= CreateEnemyExpParticle;
        }

        private void CreateEnemyExpParticle(Events.EnemyDeathEventArgs args)
        {
            if (args == null) return;
            if (!enemyExpParticle || !player) return;

            var expParticle = Instantiate(enemyExpParticle, args.Position, Quaternion.Euler(-90f, 0f, 0f));
            var particleView = expParticle.GetComponent<ParticleView>() ??
                               expParticle.gameObject.AddComponent<ParticleView>();
            particleView.Initialize(player.transform, args.Experience);
        }
    }
}
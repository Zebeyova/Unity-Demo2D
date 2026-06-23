using UnityEngine;

namespace Script.RunTimeDatas
{
    public class EffectSystem : MonoBehaviour
    {
        public ParticleSystem enemyExpParticle;

        private void Start()
        {
            EnemyRunTimeData.OnEnemyDeath += PlayEnemyExpParticle;
        }

        private void PlayEnemyExpParticle(Vector3 pos)
        {
            if (!enemyExpParticle) return;
            var EnemyExp = Instantiate(enemyExpParticle, transform);
            EnemyExp.transform.position = pos;
            EnemyExp.Play();
        }
    }
}
using UnityEngine;

namespace Script.Views
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleView : MonoBehaviour
    {
        [SerializeField] private float followSpeed = 0.5f;

        private Transform _player;
        private float _experience;
        private bool _initialized;

        public void Initialize(Transform player, float experience)
        {
            _player = player;
            _experience = experience;
            _initialized = true;
        }

        private void Update()
        {
            if (!_initialized) return;
            if (!_player)
            {
                Destroy(gameObject);
                return;
            }

            transform.position = Vector3.MoveTowards(
                transform.position, _player.position + new Vector3(0, 0.3f, 0), followSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_initialized || !other.CompareTag("Player")) return;
            Events.EventCenter.TriggerExperienceCollected(new Events.ExperienceEventArgs
            {
                experience = _experience
            });
            Destroy(gameObject);
        }
    }
}
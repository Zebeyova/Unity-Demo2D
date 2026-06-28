using UnityEngine;
using System.Collections;

namespace Script.Views
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleView : MonoBehaviour
    {
        [SerializeField] private float baseSpeed = 3f;
        [SerializeField] private float maxSpeed = 10f;
        [SerializeField] private float stayTime = 0.25f;
        [SerializeField] private float speedRampDuration = 2f;
        [SerializeField] private AnimationCurve speedCurve;

        private Transform _player;
        private float _experience;
        private float _currentSpeed;
        private bool _isFollowing;

        public void Initialize(Transform player, float experience)
        {
            _player = player;
            _experience = experience;
            _isFollowing = false;
            _currentSpeed = 0f;
            speedCurve ??= AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
            if (_player) StartCoroutine(DelayedStart());
        }

        private void Update()
        {
            if (!_player)
            {
                Destroy(gameObject);
                return;
            }

            if (!_isFollowing) return;
            transform.position = Vector3.MoveTowards(
                transform.position, _player.position + new Vector3(0, 0.3f, 0), _currentSpeed * Time.deltaTime);
        }

        private IEnumerator DelayedStart()
        {
            yield return new WaitForSeconds(stayTime);
            if (!_player) yield break;
            var targetSpeed = baseSpeed * Vector3.Distance(transform.position, _player.position);

            _isFollowing = true;
            if (speedRampDuration <= 0f)
            {
                _currentSpeed = Mathf.Min(targetSpeed, maxSpeed);
                yield break;
            }

            var elapsed = 0f;
            while (elapsed < speedRampDuration)
            {
                if (!_player) yield break;
                var curveVal = speedCurve.Evaluate(Mathf.Clamp01(elapsed / speedRampDuration));
                _currentSpeed = Mathf.Min(targetSpeed * curveVal, maxSpeed);
                elapsed += Time.deltaTime;
                yield return null;
            }

            _currentSpeed = Mathf.Min(targetSpeed, maxSpeed);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            Events.EventCenter.TriggerExperienceCollected(new Events.ExperienceEventArgs
            {
                Experience = _experience
            });
            Destroy(gameObject);
        }
    }
}
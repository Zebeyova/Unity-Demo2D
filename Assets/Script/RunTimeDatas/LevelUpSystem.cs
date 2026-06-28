using UnityEngine;

namespace Script.RunTimeDatas
{
    public class LevelUpSystem : MonoBehaviour
    {
        public static LevelUpSystem Instance { get; private set; }

        [Header("经验曲线")] [SerializeField] [Min(1)]
        private int startingLevel = 1;

        [SerializeField] [Min(0f)] private float startingExperience = 0.5f;
        [SerializeField] [Min(1f)] private float baseExperienceToNextLevel = 10f;
        [SerializeField] [Min(1f)] private float experienceGrowth = 1.35f;

        [Header("敌人强化")] [SerializeField] [Min(0f)]
        private float enemyStatGrowthPerPlayerLevel = 0.12f;

        private PlayerRunTimeData _playerData;
        private int _currentLevel;
        private float _currentExperience;
        public int CurrentLevel => _playerData ? _playerData.Level : _currentLevel;

        private void Awake()
        {
            Instance = this;
            _currentLevel = startingLevel;
            _currentExperience = startingExperience;
        }

        private void OnEnable()
        {
            Events.EventCenter.OnExperienceCollected += HandleExperienceCollected;
        }

        private void Start()
        {
            EnsurePlayerBound();
        }

        private void OnDisable()
        {
            Events.EventCenter.OnExperienceCollected -= HandleExperienceCollected;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public float CalculateExperienceToNextLevel(int level)
        {
            var safeLevel = Mathf.Max(startingLevel, level);
            return baseExperienceToNextLevel * Mathf.Pow(experienceGrowth, safeLevel - startingLevel);
        }

        public float CalculateEnemyStatMultiplier(int playerLevel)
        {
            var safeLevel = Mathf.Max(startingLevel, playerLevel);
            return 1f + Mathf.Max(0, safeLevel - startingLevel) * enemyStatGrowthPerPlayerLevel;
        }

        private void HandleExperienceCollected(Events.ExperienceEventArgs args)
        {
            if (args == null || args.experience <= 0f) return;
            EnsurePlayerBound();
            if (!_playerData) return;

            _currentExperience += args.experience;
            while (_currentExperience >= CalculateExperienceToNextLevel(_currentLevel))
            {
                _currentExperience -= CalculateExperienceToNextLevel(_currentLevel);
                _currentLevel++;
                _playerData.ApplyLevel(_currentLevel);
            }

            _playerData.ApplyExperience(_currentExperience);
        }

        private void EnsurePlayerBound()
        {
            if (_playerData) return;

            _playerData = FindObjectOfType<PlayerRunTimeData>();
            if (!_playerData) return;

            _currentLevel = Mathf.Max(startingLevel, _playerData.Level > 0 ? _playerData.Level : startingLevel);
            _currentExperience = Mathf.Max(startingExperience, _playerData.Experience);
            _playerData.ApplyProgress(_currentLevel, _currentExperience);
        }
    }
}
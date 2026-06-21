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

        private void Start()
        {
            TryBindPlayer();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void RegisterPlayer(PlayerRunTimeData player)
        {
            if (!player) return;

            _playerData = player;
            _currentLevel = Mathf.Max(startingLevel, player.Level > 0 ? player.Level : startingLevel);
            _currentExperience = Mathf.Max(startingExperience, player.Experience);
            _playerData.SetProgress(_currentLevel, _currentExperience);
        }

        public void AddExperience(float amount)
        {
            if (amount <= 0f) return;

            TryBindPlayer();
            if (!_playerData) return;

            _currentExperience += amount;

            while (_currentExperience >= GetExperienceToNextLevel(_currentLevel))
            {
                _currentExperience -= GetExperienceToNextLevel(_currentLevel);
                _currentLevel++;
                _playerData.SetLevel(_currentLevel);
            }

            _playerData.SetExperience(_currentExperience);
        }

        public float GetExperienceToNextLevel(int level)
        {
            var safeLevel = Mathf.Max(startingLevel, level);
            return baseExperienceToNextLevel * Mathf.Pow(experienceGrowth, safeLevel - startingLevel);
        }

        public float GetEnemyStatMultiplier(int playerLevel)
        {
            var safeLevel = Mathf.Max(startingLevel, playerLevel);
            return 1f + Mathf.Max(0, safeLevel - startingLevel) * enemyStatGrowthPerPlayerLevel;
        }

        private void TryBindPlayer()
        {
            if (_playerData) return;

            _playerData = FindObjectOfType<PlayerRunTimeData>();
            if (!_playerData) return;

            _currentLevel = Mathf.Max(startingLevel, _playerData.Level > 0 ? _playerData.Level : startingLevel);
            _currentExperience = Mathf.Max(startingExperience, _playerData.Experience);
            _playerData.SetProgress(_currentLevel, _currentExperience);
        }
    }
}
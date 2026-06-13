using System.Collections;
using Script.RunTimeDatas;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script.Views
{
    public class BlackMaskView : MonoBehaviour
    {
        private static readonly int Radius = Shader.PropertyToID("_Radius");
        private static readonly int Center = Shader.PropertyToID("_Center");
        private static bool _needRespawnFadeOut;

        public Material blackMaskMaterial;
        [Range(0f, 2f)] public float radius = 2f;

        private Camera _camera;
        private Coroutine _changeRadiusCoroutine;
        private PlayerRunTimeData _playerData;

        private void Awake()
        {
            if (blackMaskMaterial) blackMaskMaterial = new Material(blackMaskMaterial);
            _camera = Camera.main;
            _playerData = FindObjectOfType<PlayerRunTimeData>();
        }

        private void Start()
        {
            _playerData.OnPlayerDeath += OnPlayerDeath;
            if (_needRespawnFadeOut) StartCoroutine(ChangeRadiusCoroutine(true));
        }

        private void OnDestroy()
        {
            if (_playerData) _playerData.OnPlayerDeath -= OnPlayerDeath;
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            if (blackMaskMaterial == null) Graphics.Blit(src, dst);
            else Graphics.Blit(src, dst, blackMaskMaterial);
        }

        private void Update()
        {
            if (!_playerData) return;
            Vector2 viewPos = _camera.WorldToViewportPoint(_playerData.transform.position);
            blackMaskMaterial.SetVector(Center, new Vector4(viewPos.x, viewPos.y, 0, 0));
        }

        private void OnPlayerDeath()
        {
            if (_changeRadiusCoroutine != null) StopCoroutine(_changeRadiusCoroutine);
            _changeRadiusCoroutine = StartCoroutine(ChangeRadiusCoroutine(false));
        }

        private IEnumerator ChangeRadiusCoroutine(bool respawn)
        {
            if (respawn)
            {
                radius = 0f;
                while (radius < 2f)
                {
                    radius += Time.deltaTime * 1.5f;
                    blackMaskMaterial.SetFloat(Radius, radius);
                    yield return null;
                }

                _needRespawnFadeOut = false;
            }
            else
            {
                radius = 2f;
                while (radius > 0f)
                {
                    radius -= Time.deltaTime * 1.5f;
                    blackMaskMaterial.SetFloat(Radius, radius);
                    yield return null;
                }

                yield return new WaitForSeconds(0.5f);
                _needRespawnFadeOut = true;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }
}
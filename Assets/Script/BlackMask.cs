using System;
using System.Collections;
using Script.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Script
{
    public class BlackMask : MonoBehaviour
    {
        private static readonly int Radius = Shader.PropertyToID("_Radius");
        private static readonly int Center = Shader.PropertyToID("_Center");
        private static bool _needRespawnFadeOut;

        public Material blackMaskMaterial;
        [Range(0f, 2f)] public float radius = 2f;
        private Camera _camera;
        private Coroutine _changeRadiusCoroutine;
        private PlayerController _playerController;
        private Health _playerHealth;
        private void Awake()
        {
            CheckComponent();
            if (blackMaskMaterial != null) blackMaskMaterial = new Material(blackMaskMaterial);
        }
        private void Start()
        {
            _playerHealth.onDeath.AddListener(ChangeRadius);
            if (_needRespawnFadeOut)
            {
                if (_changeRadiusCoroutine != null) StopCoroutine(_changeRadiusCoroutine);
                _changeRadiusCoroutine = StartCoroutine(ChangeRadiusCoroutine(true));
            }
        }
        private void Update()
        {
            if (!_playerController) return;
            Vector2 _pPos = _camera.WorldToViewportPoint(_playerController.transform.position);
            blackMaskMaterial.SetVector(Center, new Vector4(_pPos.x, _pPos.y, 0, 0));
        }
        private void OnDestroy() => _playerHealth.onDeath.RemoveListener(ChangeRadius);

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (blackMaskMaterial == null)
            {
                Graphics.Blit(source, destination);
                return;
            }

            Graphics.Blit(source, destination, blackMaskMaterial);
        }
        private void CheckComponent()
        {
            _playerController = FindObjectOfType<PlayerController>();
            _camera = Camera.main;
            _playerHealth = _playerController.GetComponent<Health>();
        }
        private void ChangeRadius()
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
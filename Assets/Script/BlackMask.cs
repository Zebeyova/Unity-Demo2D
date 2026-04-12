using Script.Player;
using UnityEngine;

namespace Script
{
    public class BlackMask : MonoBehaviour
    {
        private static readonly int Radius = Shader.PropertyToID("_Radius");
        private static readonly int Center = Shader.PropertyToID("_Center");

        public Material blackMaskMaterial;
        [Range(0f, 2f)] public float radius = 2f;
        private Coroutine _changeRadiusCoroutine;
        private PlayerController _playerController;
        private Camera _camera;
        private Health _playerHealth;
        private void Awake()
        {
            CheckComponent();
            if (blackMaskMaterial != null) blackMaskMaterial = new Material(blackMaskMaterial);
        }
        private void CheckComponent()
        {
            if (!_playerController) _playerController = FindObjectOfType<PlayerController>();
            if (!_camera) _camera = Camera.main;
            if (!_playerHealth) _playerHealth = _playerController.GetComponent<Health>();
        }
        private void Start()
        {
            _playerHealth.onDeath.AddListener(ChangeRadius);
        }
        private void Update()
        {
            if (!_playerController) return;
            Vector2 _pPos = _camera.WorldToViewportPoint(_playerController.transform.position);
            blackMaskMaterial.SetVector(Center, new Vector4(_pPos.x, _pPos.y, 0, 0));
        }
        private void ChangeRadius()
        {
            if (_changeRadiusCoroutine != null) StopCoroutine(_changeRadiusCoroutine);
            _changeRadiusCoroutine = StartCoroutine(ChangeRadiusCoroutine());
        }
        System.Collections.IEnumerator ChangeRadiusCoroutine()
        {
            while (radius > 0f)
            {
                radius -= Time.deltaTime * 1.5f;
                blackMaskMaterial.SetFloat(Radius, radius);
                yield return null;
            }

            radius = 0f;
            blackMaskMaterial.SetFloat(Radius, radius);
        }
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (blackMaskMaterial == null)
            {
                Graphics.Blit(source, destination);
                return;
            }

            Graphics.Blit(source, destination, blackMaskMaterial);
        }
    }
}
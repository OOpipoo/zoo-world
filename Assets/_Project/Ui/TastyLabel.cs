using TMPro;
using UnityEngine;

namespace _Project.Ui
{
    public class TastyLabel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Vector2 _screenOffset = new Vector2(0f, 40f);

        private RectTransform _rectTransform;
        private RectTransform _canvasRect;
        private Camera _camera;

        private Transform _anchor;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _camera = Camera.main;
            _canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            gameObject.SetActive(false);
        }

        public void Show(Transform anchor)
        {
            if (anchor == null) return;
            _anchor = anchor;
            gameObject.SetActive(true);
            UpdatePosition();
        }

        public void Hide()
        {
            _anchor = null;
            gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            if (_anchor == null) return;
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            if (_camera == null) _camera = Camera.main;
            if (_camera == null || _canvasRect == null || _anchor == null) return;

            var screenPos = _camera.WorldToScreenPoint(_anchor.position);

            if (screenPos.z < 0f) { _text.enabled = false; return; }

            _text.enabled = true;

            screenPos.x += _screenOffset.x;
            screenPos.y += _screenOffset.y;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvasRect, screenPos, null, out var localPoint);
            _rectTransform.anchoredPosition = localPoint;
        }
    }
}

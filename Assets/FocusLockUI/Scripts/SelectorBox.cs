using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FocusLockable
{
    public class SelectorBox : MonoBehaviour
    {
        [SerializeField]
        private int _ignoreLayer = 2;

        [SerializeField]
        private Transform _scaleTransform;

        [SerializeField]
        [Range(0.0f, 0.5f)]
        private float _scalePadding = 0.05f;

        private Vector3 _targetBoundsWorldCenter = Vector3.zero;
        private Vector3 _targetBoundsLocalScale = Vector3.zero;

        private Vector3[] _corners = null;
        private Vector3[] _rectTransformCorners = new Vector3[4];
        private Bounds _localTargetBounds = new Bounds();
        private List<Vector3> _boundsPoints = new List<Vector3>();


        public void UpdateSelectorBox()
        {
            RefreshTargetBounds();
            UpdateGizmoPosition();
        }

        private void RefreshTargetBounds()
        {
            if(FocusLockManager.Instance.Target == null)
            {
                _targetBoundsWorldCenter = Vector3.zero;
                _targetBoundsLocalScale = Vector3.zero;
                return;
            }

            _boundsPoints.Clear();

            var target = FocusLockManager.Instance.Target;

            Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
            for(int i=0; i < renderers.Length; ++i)
            {
                var rendererObj = renderers[i];
                if (rendererObj.gameObject.layer == _ignoreLayer)
                    continue;

                rendererObj.bounds.GetCornerPositionsFromRendererBounds(ref _corners);
                _boundsPoints.AddRange(_corners);
            }

            RectTransform[] rectTransforms = target.GetComponentsInChildren<RectTransform>();
            for(int i=0; i < rectTransforms.Length; ++i)
            {
                rectTransforms[i].GetWorldCorners(_rectTransformCorners);
                _boundsPoints.AddRange(_rectTransformCorners);
            }

            if(_boundsPoints.Count > 0)
            {
                for(int i=0; i < _boundsPoints.Count; ++i)
                {
                    _boundsPoints[i] = target.transform.InverseTransformPoint(_boundsPoints[i]);
                }

                _localTargetBounds.center = _boundsPoints[0];
                _localTargetBounds.size = Vector3.zero;
                foreach(var point in _boundsPoints)
                {
                    _localTargetBounds.Encapsulate(point);
                }
            }

            _targetBoundsWorldCenter = target.transform.TransformPoint(_localTargetBounds.center);
            _targetBoundsLocalScale = _localTargetBounds.size;
            _targetBoundsLocalScale.Scale(target.transform.localScale);
        }

        private void UpdateGizmoPosition()
        {
            if (FocusLockManager.Instance.Target == null)
            {
                transform.position = Vector3.zero;
                _scaleTransform.localScale = Vector3.zero;
                return;
            }

            transform.position = _targetBoundsWorldCenter;
            var scale = _targetBoundsLocalScale;
            var largestDimension = Mathf.Max(Mathf.Max(scale.x, scale.y), scale.z);

            scale.x += largestDimension * _scalePadding;
            scale.y += largestDimension * _scalePadding;
            scale.z += largestDimension * _scalePadding;

            _scaleTransform.localScale = scale;
            transform.eulerAngles = FocusLockManager.Instance.Target.transform.eulerAngles;
        }
    }
}
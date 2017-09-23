using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.VR.WSA.Input;

namespace FocusLockable
{
    public class FocusLockManager : Singleton<FocusLockManager> {

        [Header("Sound effect settings")]
        [SerializeField]
        private bool _isEnabledClickSound;

        [SerializeField]
        private AudioClip _clickSE;

        [Header("Lock release timer settings")]
        [SerializeField]
        private bool _isEnabledAutoRelease;

        [SerializeField]
        [Range(0.0f, 5.0f)]
        private float _autoReleaseTime;

        [Header("Box settings")]
        [SerializeField]
        private Color _colorWhenFingerUp;

        [SerializeField]
        private Color _colorWhenFingerDown;

        [SerializeField]
        private GameObject _selectorBoxPrefab;


        public GameObject Target => _target;
        public bool IsEnabledClickSound => _isEnabledClickSound;
        public bool IsEnabledAutoRelease => _isEnabledAutoRelease;

        private SelectorBox _selectorBox;
        private Material _selectorBoxMaterial;
        private AudioSource _clickSoundSource;
        private bool _isTimerActive;
        private float _lockStartTime;
        private GameObject _target;
        

        private void OnEnable()
        {
            StartCoroutine(Initialize());
        }

        private void OnDisable()
        {
            GazeManager.Instance.FocusedObjectChanged -= OnFocusedObjectChanged;
            InteractionManager.SourcePressed -= OnSourcePressed;
            InteractionManager.SourceReleased -= OnSourceReleased;
            InteractionManager.SourceLost -= OnSourceLost;
        }

        private void Update()
        {
            if(_isEnabledAutoRelease)
            {
                if(_isTimerActive && Time.unscaledTime - _lockStartTime > _autoReleaseTime)
                {
                    ExecuteEvents.Execute(_target, null, OnFocusLockReleasedEventHandler);
                    _target = null;
                    InputManager.Instance.OverrideFocusedObject = null;
                    _selectorBox.UpdateSelectorBox();
                }
            }

            if (_target != null) _selectorBox.UpdateSelectorBox();
        }

        private IEnumerator Initialize()
        {
            while(true)
            {
                if (GazeManager.Initialized) break;
                else yield return null;
            }

            GazeManager.Instance.FocusedObjectChanged += OnFocusedObjectChanged;
            InteractionManager.SourcePressed += OnSourcePressed;
            InteractionManager.SourceReleased += OnSourceReleased;
            InteractionManager.SourceLost += OnSourceLost;

            var go = Instantiate(_selectorBoxPrefab);
            _selectorBox = go.GetComponent<SelectorBox>();
            _selectorBoxMaterial = go.GetComponentInChildren<MeshRenderer>().material;
            _selectorBox.UpdateSelectorBox();

            _clickSoundSource = gameObject.EnsureComponent<AudioSource>();
            _clickSoundSource.playOnAwake = false;
            _clickSoundSource.clip = _clickSE;
        }


        private void OnSourcePressed(InteractionSourceState state)
        {
            _selectorBoxMaterial.color = _colorWhenFingerDown;
        }
        

        private void OnSourceLost(InteractionSourceState state)
        {
            _selectorBoxMaterial.color = _colorWhenFingerUp;
        }

        private void OnSourceReleased(InteractionSourceState state)
        {
            if (_isEnabledClickSound && _clickSE != null && _target != null && _selectorBoxMaterial.color == _colorWhenFingerDown)
                _clickSoundSource.Play();

            _selectorBoxMaterial.color = _colorWhenFingerUp;
        }


        private void OnFocusedObjectChanged(GameObject previousObject, GameObject newObject)
        {
            if(newObject != null)
            {
                var go = newObject;
                while(true)
                {
                    if (go.GetComponent<IFocusLockable>() != null) break;
                    else
                    {
                        if (go.transform.parent == null) return;
                        else go = go.transform.parent.gameObject;
                    }
                }

                if (_isEnabledAutoRelease) _isTimerActive = false;

                if(_target != go)
                {
                    ExecuteEvents.Execute(_target, null, OnFocusLockReleasedEventHandler);

                    _target = go;
                    _selectorBoxMaterial.color = _colorWhenFingerUp;
                    InputManager.Instance.OverrideFocusedObject = _target;
                    ExecuteEvents.Execute(_target, null, OnFocusLockedEventHandler);
                }
            }

            if(newObject == null && _isEnabledAutoRelease)
            {
                _lockStartTime = Time.unscaledTime;
                _isTimerActive = true;
            }
        }

        private static readonly ExecuteEvents.EventFunction<IFocusLockable> OnFocusLockedEventHandler =
            delegate (IFocusLockable handler, BaseEventData eventData)
            {
                handler.OnFocusLocked();
            };

        private static readonly ExecuteEvents.EventFunction<IFocusLockable> OnFocusLockReleasedEventHandler =
            delegate (IFocusLockable handler, BaseEventData eventData)
            {
                handler.OnFocusReleased();
            };
    }
}
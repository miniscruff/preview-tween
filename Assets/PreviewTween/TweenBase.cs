namespace PreviewTween
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;

    public enum PlayMode
    {
        None,
        Start,
        OnEnable
    }

    public enum WrapMode
    {
        Once,
        Loop,
        PingPong
    }

    public enum EasingMode
    {
        Linear,
        QuadraticIn,
        QuadraticOut,
        QuadraticInOut,
        CustomCurve
    }

    public abstract class TweenBase : MonoBehaviour
    {
        // tween values
        private float _progress;
        private bool _isPlaying;
        private int _direction = 1;

        // settings
        [SerializeField] private float _delay;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private PlayMode _playMode;
        [SerializeField] private WrapMode _wrapMode;
        [SerializeField] private EasingMode _easingMode;
        [SerializeField] private AnimationCurve _customCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private UnityEvent _onComplete = new UnityEvent();

        public float progress
        {
            get { return _progress; }
            set { _progress = value; }
        }

        public bool isPlaying
        {
            get { return _isPlaying; }
        }

        public float delay
        {
            get { return _delay; }
            set { _delay = value; }
        }

        public float duration
        {
            get { return _duration; }
            set { _duration = value; }
        }

        public PlayMode playMode
        {
            get { return _playMode; }
            set { _playMode = value; }
        }

        public WrapMode wrapMode
        {
            get { return _wrapMode; }
            set { _wrapMode = value; }
        }

        public EasingMode easingMode
        {
            get { return _easingMode; }
            set { _easingMode = value; }
        }

        public AnimationCurve customCurve
        {
            get { return _customCurve; }
            set { _customCurve = value; }
        }

        // Kinda lame but UnityEvents arent considered events so resharper thinks it should just be complete
        // ReSharper disable once InconsistentNaming
        public UnityEvent onComplete
        {
            get { return _onComplete; }
        }

        private void Start()
        {
            if (_playMode == PlayMode.Start)
            {
                Play();
            }
        }

        private void OnEnable()
        {
            if (_playMode == PlayMode.OnEnable)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            Stop();
        }

        public void Apply()
        {
            UpdateValue(GetSmoothTime());
        }

        public void Play()
        {
            if (!CanPlay())
            {
                return;
            }

            Apply();
            if (!_isPlaying)
            {
                _isPlaying = true;
                StartCoroutine(RunTween());
            }
        }

        public void Replay()
        {
            if (!CanPlay())
            {
                return;
            }

            _progress = 0f;
            _direction = 1;
            Play();
        }

        public void Toggle()
        {
            if (!CanPlay())
            {
                return;
            }

            if (progress <= 0f)
            {
                _direction = 1;
            }
            else if (progress >= 1f)
            {
                _direction = -1;
            }
            else
            {
                _direction *= -1;
            }

            Play();
        }

        public void Stop()
        {
            _isPlaying = false;
        }

        private IEnumerator RunTween()
        {
            if (_delay > 0f)
            {
                yield return new WaitForSeconds(_delay);
            }

            while (_isPlaying)
            {
                yield return null;

                _progress += Time.deltaTime / _duration * _direction;
                if (_progress <= 0f || _progress >= 1f)
                {
                    HandleWrapping();
                    Apply();

                    // we want to call our onComplete AFTER we apply
                    _onComplete.Invoke();
                }
                else
                {
                    Apply();
                }
            }

            _isPlaying = false;
        }

        private void HandleWrapping()
        {
            if (wrapMode == WrapMode.Once)
            {
                _isPlaying = false;
                _progress = Mathf.Clamp01(_progress);
            }
            else if (wrapMode == WrapMode.Loop)
            {
                // we add one incase we are < 0
                _progress = (_progress + 1f) % 1f;
            }
            else if (wrapMode == WrapMode.PingPong)
            {
                // ping pong bounces progress and flips direction
                if (_progress < 0f)
                {
                    _progress *= -1;
                    _direction = 1;
                }
                else if (_progress > 1f)
                {
                    _progress = 2f - _progress;
                    _direction = -1;
                }
            }
        }

        private bool CanPlay()
        {
            return enabled;
        }

        private float GetSmoothTime()
        {
            switch (_easingMode)
            {
                case EasingMode.Linear:
                    return Easings.Linear(_progress);
                case EasingMode.QuadraticIn:
                    return Easings.QuadraticIn(_progress);
                case EasingMode.QuadraticOut:
                    return Easings.QuadraticOut(_progress);
                case EasingMode.QuadraticInOut:
                    return Easings.QuadraticInOut(_progress);
                case EasingMode.CustomCurve:
                    return _customCurve.Evaluate(_progress);
            }
            throw new NotImplementedException("Easing [" + _easingMode + "] not yet implemented");
        }

        protected abstract void UpdateValue(float smoothTime);
    }
}
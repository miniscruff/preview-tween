namespace PreviewTween
{
    using System.Collections;
    using UnityEngine;

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

    public abstract class TweenBase<T> : MonoBehaviour
    {
        // tween values
        [SerializeField] private T _start;
        [SerializeField] private T _end;
        private float _progress;
        private bool _isPlaying;
        private int _direction = 1;

        // settings
        [SerializeField] private float _delay;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private PlayMode _playMode;
        [SerializeField] private WrapMode _wrapMode;

        public T start
        {
            get { return _start; }
            set { _start = value; }
        }

        public T end
        {
            get { return _end; }
            set { _end = value; }
        }

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

        public void Sample()
        {
            UpdateValue(_progress);
        }

        public void Play()
        {
            if (!CanPlay())
            {
                return;
            }

            _isPlaying = true;
            StartCoroutine(RunTween());
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
            // sample before starting our loop as well
            Sample();

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
                }
                Sample();
            }

            _isPlaying = false;
        }

        private void HandleWrapping()
        {
            if (wrapMode == WrapMode.Once)
            {
                // we are now done
                _isPlaying = false;
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

        protected abstract void UpdateValue(float time);
    }
}
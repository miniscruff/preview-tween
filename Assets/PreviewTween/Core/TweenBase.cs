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
        private int _pingPongDirection = 1;

        // settings
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
            // do nothing if we arent on...
            if (!enabled)
            {
                return;
            }

            _isPlaying = true;
            StartCoroutine(RunTween());
        }

        public void Stop()
        {
            _isPlaying = false;
        }

        private IEnumerator RunTween()
        {
            // sample before starting our loop as well
            Sample();

            while (_isPlaying)
            {
                if (_progress >= 1f && wrapMode == WrapMode.Once)
                {
                    break;
                }
                yield return null;

                if (wrapMode == WrapMode.Once)
                {
                    _progress = Mathf.Clamp01(_progress + Time.deltaTime / _duration);
                }
                else if (wrapMode == WrapMode.Loop)
                {
                    _progress = (_progress + Time.deltaTime / _duration) % 1f;
                }
                else if (wrapMode == WrapMode.PingPong)
                {
                    _progress += Time.deltaTime / _duration * _pingPongDirection;
                    if (_progress < 0f)
                    {
                        _progress *= -1;
                        _pingPongDirection = 1;
                    }
                    else if(_progress > 1f)
                    {
                        _progress = 2f - _progress;
                        _pingPongDirection = -1;
                    }
                }
                Sample();
            }

            _isPlaying = false;
        }

        protected abstract void UpdateValue(float time);
    }
}
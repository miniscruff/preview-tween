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

    public abstract class TweenBase<T> : MonoBehaviour
    {
        // tween values
        [SerializeField] private T _start;
        [SerializeField] private T _end;
        private float _progress;
        [SerializeField] private float _duration = 1f;
        private bool _isPlaying;

        // settings
        [SerializeField] private PlayMode _playMode;

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

        public float duration
        {
            get { return _duration; }
            set { _duration = value; }
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

        public PlayMode playMode
        {
            get { return _playMode; }
            set { _playMode = value; }
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

            while (_isPlaying && (_progress < 1f))
            {
                yield return null;

                _progress = Mathf.Clamp01(_progress + Time.deltaTime / _duration);
                Sample();
            }

            _isPlaying = false;
        }

        protected abstract void UpdateValue(float time);
    }
}
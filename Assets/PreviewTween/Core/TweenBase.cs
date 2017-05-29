namespace PreviewTween
{
    using System.Collections;
    using UnityEngine;

    public abstract class TweenBase<T> : MonoBehaviour
    {
        [SerializeField] private T _start;
        [SerializeField] private T _end;
        private float _progress;
        private float _duration = 1f;

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

        private void Start()
        {
            // default to playing on start for now
            Play();
        }

        public void Sample()
        {
            UpdateValue(_progress);
        }

        public void Play()
        {
            StartCoroutine(RunTween());
        }

        private IEnumerator RunTween()
        {
            // sample before starting our loop as well
            Sample();

            while (_progress < 1f)
            {
                yield return null;

                _progress = Mathf.Clamp01(_progress + Time.deltaTime / _duration);
                Sample();
            }
        }

        protected abstract void UpdateValue(float time);
    }
}
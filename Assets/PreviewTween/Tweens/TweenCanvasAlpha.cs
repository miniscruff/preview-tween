namespace PreviewTween
{
    using UnityEngine;

    public sealed class TweenCanvasAlpha : TweenBase
    {
        [SerializeField] private CanvasGroup _target;
        [SerializeField] private float _start;
        [SerializeField] private float _end = 1f;

        public CanvasGroup target
        {
            get { return _target; }
            set { _target = value; }
        }

        public float start
        {
            get { return _start; }
            set { _start = value; }
        }

        public float end
        {
            get { return _end; }
            set { _end = value; }
        }

        private void Reset()
        {
            _target = GetComponent<CanvasGroup>();
            if (_target != null)
            {
                _start = _target.alpha;
                _end = _target.alpha;
            }
        }

        public override void RecordStart()
        {
            _start = _target.alpha;
        }

        public override void RecordEnd()
        {
            _end = _target.alpha;
        }

        protected override void UpdateValue(float smoothTime)
        {
            target.alpha = Mathf.Lerp(start, end, smoothTime);
        }
    }
}
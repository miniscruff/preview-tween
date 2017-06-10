namespace PreviewTween
{
    using UnityEngine;

    public sealed class TweenScale : TweenBase
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _start;
        [SerializeField] private Vector3 _end;

        public Transform target
        {
            get { return _target; }
            set { _target = value; }
        }

        public Vector3 start
        {
            get { return _start; }
            set { _start = value; }
        }

        public Vector3 end
        {
            get { return _end; }
            set { _end = value; }
        }

        private void Reset()
        {
            _target = transform;
            _start = transform.localScale;
            _end = transform.localScale;
        }

        public override void RecordStart()
        {
            _start = _target.localScale;
        }

        public override void RecordEnd()
        {
            _end = _target.localScale;
        }

        protected override void UpdateValue(float smoothTime)
        {
            _target.localScale = Vector3.LerpUnclamped(_start, _end, smoothTime);
        }
    }
}
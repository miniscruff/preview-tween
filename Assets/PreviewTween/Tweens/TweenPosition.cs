namespace PreviewTween
{
    using UnityEngine;

    public sealed class TweenPosition : TweenBase
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _start;
        [SerializeField] private Vector3 _end;
        [SerializeField] private bool _worldSpace = true;

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

        public bool worldSpace
        {
            get { return _worldSpace; }
            set { _worldSpace = value; }
        }

        private void Reset()
        {
            _target = transform;
            _start = transform.position;
            _end = transform.position;
        }

        public override void RecordStart()
        {
            _start = _worldSpace ? target.position : target.localPosition;
        }

        public override void RecordEnd()
        {
            _end = _worldSpace ? target.position : target.localPosition;
        }

        protected override void UpdateValue(float smoothTime)
        {
            if (_worldSpace)
            {
                _target.position = Vector3.LerpUnclamped(_start, _end, smoothTime);
            }
            else
            {
                _target.localPosition = Vector3.LerpUnclamped(_start, _end, smoothTime);
            }
        }
    }
}
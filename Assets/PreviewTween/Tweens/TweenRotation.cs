namespace PreviewTween
{
    using UnityEngine;

    public sealed class TweenRotation : TweenBase
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Quaternion _start;
        [SerializeField] private Quaternion _end;
        [SerializeField] private bool _worldSpace = true;

        public Transform target
        {
            get { return _target; }
            set { _target = value; }
        }

        public Quaternion start
        {
            get { return _start; }
            set { _start = value; }
        }

        public Quaternion end
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
            _start = transform.rotation;
            _end = transform.rotation;
        }

#if UNITY_EDITOR
        public override void RecordStart()
        {
            _start = _worldSpace ? transform.rotation : transform.localRotation;
        }

        public override void RecordEnd()
        {
            _end = _worldSpace ? transform.rotation : transform.localRotation;
        }
#endif

        protected override void UpdateValue(float smoothTime)
        {
            if (_worldSpace)
            {
                _target.rotation = Quaternion.LerpUnclamped(_start, _end, smoothTime);
            }
            else
            {
                _target.localRotation = Quaternion.LerpUnclamped(_start, _end, smoothTime);
            }
        }
    }
}
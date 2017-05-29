namespace PreviewTween
{
    using UnityEngine;

    public sealed class TweenPosition : TweenBase
    {
        [SerializeField] private Vector3 _start;
        [SerializeField] private Vector3 _end;

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

        protected override void UpdateValue(float smoothTime)
        {
            transform.position = Vector3.LerpUnclamped(_start, _end, smoothTime);
        }
    }
}
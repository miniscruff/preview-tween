namespace PreviewTween
{
    using UnityEngine;

    public sealed class TweenPosition : TweenBase
    {
        [SerializeField] private Vector3 _start;
        [SerializeField] private Vector3 _end;

        protected override void UpdateValue(float smoothTime)
        {
            transform.position = Vector3.LerpUnclamped(_start, _end, smoothTime);
        }
    }
}
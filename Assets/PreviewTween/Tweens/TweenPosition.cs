namespace PreviewTween
{
    using UnityEngine;

    public sealed class TweenPosition : TweenBase<Vector3>
    {
        protected override void UpdateValue(float smoothTime)
        {
            transform.position = Vector3.LerpUnclamped(start, end, smoothTime);
        }
    }
}
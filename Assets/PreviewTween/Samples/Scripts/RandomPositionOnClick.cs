namespace PreviewTween.Samples
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// Simple demo showing how you can use external scripts to control a tween during runtime
    /// </summary>
    public sealed class RandomPositionOnClick : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TweenPosition _tween;
        [SerializeField] private Vector3 _boundsMin;
        [SerializeField] private Vector3 _boundsMax;

        public void OnPointerClick(PointerEventData eventData)
        {
            _tween.start = _tween.target.position;
            _tween.end = new Vector3(
                Random.Range(_boundsMin.x, _boundsMax.x),
                Random.Range(_boundsMin.y, _boundsMax.y),
                Random.Range(_boundsMin.z, _boundsMax.z)
            );
            _tween.Replay();
        }
    }
}
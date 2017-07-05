namespace PreviewTween
{
    using NUnit.Framework;
    using UnityEngine;

    public abstract class SetupTweenTests<T> where T : TweenBase
    {
        private GameObject _gameObject;
        private T _tween;

        protected GameObject gameObject
        {
            get { return _gameObject; }
        }

        protected T tween
        {
            get { return _tween; }
        }

        [SetUp]
        public virtual void SetUp()
        {
            SetUp(new GameObject("Tween"));
        }

        protected void SetUp(GameObject precreatedObject)
        {
            _gameObject = precreatedObject;
            _tween = _gameObject.AddComponent<T>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_gameObject != null)
            {
                Object.DestroyImmediate(_gameObject);
            }
        }
    }
}
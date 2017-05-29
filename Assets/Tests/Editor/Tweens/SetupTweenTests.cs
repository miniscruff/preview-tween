namespace PreviewTween
{
    using NUnit.Framework;
    using UnityEngine;

    public abstract class SetupTweenTests<T> where T : TweenBase
    {
        private T _tween;
        protected T tween
        {
            get { return _tween; }
        }

        [SetUp]
        public virtual void SetUp()
        {
            GameObject go = new GameObject("Tween");
            _tween = go.AddComponent<T>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_tween.gameObject);
        }
    }
}
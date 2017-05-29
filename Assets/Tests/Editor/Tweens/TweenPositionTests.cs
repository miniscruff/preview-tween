namespace PreviewTween
{
    using NUnit.Framework;
    using UnityEngine;

    public sealed class TweenPositionTests
    {
        private TweenPosition _tweenPosition;

        [SetUp]
        public void SetUp()
        {
            GameObject go = new GameObject("TweenPosition");
            _tweenPosition = go.AddComponent<TweenPosition>();

            _tweenPosition.start = new Vector3(10, 0, 0);
            _tweenPosition.end = new Vector3(20, 0, 0);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_tweenPosition.gameObject);
        }

        [Test]
        public void ValueAtStart()
        {
            _tweenPosition.progress = 0f;
            _tweenPosition.Apply();

            Assert.AreEqual(new Vector3(10, 0, 0), _tweenPosition.transform.position);
        }

        [Test]
        public void ValueAtEnd()
        {
            _tweenPosition.progress = 1f;
            _tweenPosition.Apply();

            Assert.AreEqual(new Vector3(20, 0, 0), _tweenPosition.transform.position);
        }

        [Test]
        public void ValueAtMiddle()
        {
            _tweenPosition.progress = 0.5f;
            _tweenPosition.Apply();

            Assert.AreEqual(new Vector3(15, 0, 0), _tweenPosition.transform.position);
        }
    }
}
namespace PreviewTween
{
    using System.Collections;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;

    public sealed class TestTween : TweenBase<float>
    {
        protected override float UpdateValue(float time)
        {
            return Mathf.Lerp(start, end, time);
        }
    }

    public sealed class TweenBaseTests
    {
        private GameObject _tweenObject;
        private TestTween _tween;

        [SetUp]
        public void SetUp()
        {
            _tweenObject = new GameObject("Tween");
            _tween = _tweenObject.AddComponent<TestTween>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_tweenObject);
        }

        [Test]
        public void Sample_ProgressZero_HasStartValue()
        {
            _tween.start = 10f;
            _tween.end = 25f;
            _tween.Sample();

            Assert.AreEqual(10f, _tween.value);
        }

        [Test]
        public void Sample_ProgressHalf_HasMiddleValue()
        {
            _tween.start = 10f;
            _tween.end = 20f;
            _tween.progress = 0.5f;
            _tween.Sample();

            Assert.AreEqual(15f, _tween.value);
        }

        [Test]
        public void Sample_ProgressOne_HasEndValue()
        {
            _tween.start = 10f;
            _tween.end = 25f;
            _tween.progress = 1f;
            _tween.Sample();

            Assert.AreEqual(25f, _tween.value);
        }

        [Test]
        public void Play_ImmediatelyHasStartValue()
        {
            _tween.start = 10f;
            _tween.end = 20f;

            _tween.Play();

            Assert.AreEqual(10f, _tween.value);
        }

        [UnityTest]
        public IEnumerator Play_DefaultDuration_SetsValueToEnd()
        {
            _tween.start = 10f;
            _tween.end = 20f;

            _tween.Play();
            yield return new WaitForSeconds(1f);

            Assert.AreEqual(20f, _tween.value);
        }

        [UnityTest]
        public IEnumerator PlayTwoSecondDuration_SetsValueToEnd()
        {
            _tween.start = 10f;
            _tween.end = 20f;
            _tween.duration = 2f;

            _tween.Play();
            yield return new WaitForSeconds(2f);

            Assert.AreEqual(20f, _tween.value);
        }
    }
}
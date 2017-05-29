namespace PreviewTween
{
    using System.Collections;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;

    public sealed class TestTween : TweenBase<float>
    {
        private float _value;
        public float value { get { return _value; } }

        protected override void UpdateValue(float time)
        {
            _value = Mathf.Lerp(start, end, time);
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
            Assert.IsTrue(_tween.isPlaying);
            yield return new WaitForSeconds(1f);

            Assert.IsFalse(_tween.isPlaying);

            Assert.AreEqual(20f, _tween.value);
        }

        [Test]
        public void DisabledComponent_DoesntPlay()
        {
            _tween.start = 10f;
            _tween.end = 20f;

            _tween.enabled = false;

            _tween.Play();
            Assert.IsFalse(_tween.isPlaying);
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

        [UnityTest]
        public IEnumerator NonePlayMode_OnlyPlaysIfWeCallPlay()
        {
            _tween.playMode = PlayMode.None;
            _tweenObject.SetActive(false);

            yield return null;
            Assert.IsFalse(_tween.isPlaying);

            yield return null;
            _tweenObject.SetActive(true);

            yield return null;
            Assert.IsFalse(_tween.isPlaying);

            yield return null;
            _tween.Play();
            Assert.IsTrue(_tween.isPlaying);
        }

        [UnityTest]
        public IEnumerator StartPlayMode_PlaysImmediately()
        {
            // teardown and rebuild our game object so it starts off
            // this is cause we need start to be called AFTER we have changed a setting
            TearDown();

            _tweenObject = new GameObject("Tween");
            _tweenObject.SetActive(false);

            _tween = _tweenObject.AddComponent<TestTween>();
            _tween.playMode = PlayMode.Start;

            // check to make sure we dont start until we are turned on
            yield return null;
            Assert.IsFalse(_tween.isPlaying);

            yield return null;
            _tweenObject.SetActive(true);

            yield return null;
            Assert.IsTrue(_tween.isPlaying);
        }

        [UnityTest]
        public IEnumerator OnEnablePlayMode_PlaysIfWeAreTurnedOn()
        {
            _tween.playMode = PlayMode.OnEnable;

            yield return null;
            Assert.IsFalse(_tween.isPlaying);

            _tweenObject.SetActive(false);
            _tweenObject.SetActive(true);

            yield return null;
            Assert.IsTrue(_tween.isPlaying);
        }

        [UnityTest]
        public IEnumerator Stop_PreventsAnyMoreProgress()
        {
            _tween.start = 10f;
            _tween.end = 20f;

            _tween.Play();
            Assert.IsTrue(_tween.isPlaying);
            yield return new WaitForSeconds(0.5f);

            // stop the tween halfway through and check our values
            _tween.Stop();
            yield return new WaitForSeconds(0.5f);

            // we wont be exact due to coroutines and editor stuff
            Assert.LessOrEqual(_tween.progress, 0.55f);
            // do the same lerp we would normally do to make sure it was updated with the exact value
            Assert.AreEqual(Mathf.Lerp(10f, 20f, _tween.progress), _tween.value);
            Assert.IsFalse(_tween.isPlaying);

            // go back to playing for the remainder
            _tween.Play();

            yield return new WaitForSeconds(0.5f);

            Assert.IsFalse(_tween.isPlaying);
            Assert.AreEqual(20f, _tween.value);
        }

        [UnityTest]
        public IEnumerator DisablingComponent_StopsTween()
        {
            _tween.start = 10f;
            _tween.end = 20f;

            _tween.Play();
            Assert.IsTrue(_tween.isPlaying);
            yield return new WaitForSeconds(0.5f);

            // disable the tween halfway through and check our values
            _tween.enabled = false;
            yield return new WaitForSeconds(0.5f);

            // we wont be exact due to coroutines and editor stuff
            Assert.LessOrEqual(_tween.progress, 0.55f);
            // do the same lerp we would normally do to make sure it was updated with the exact value
            Assert.AreEqual(Mathf.Lerp(10f, 20f, _tween.progress), _tween.value);
            Assert.IsFalse(_tween.isPlaying);

            // go back to playing for the remainder
            _tween.enabled = true;
            _tween.Play();
            Assert.IsTrue(_tween.isPlaying);

            yield return new WaitForSeconds(0.5f);

            Assert.IsFalse(_tween.isPlaying);
            Assert.AreEqual(20f, _tween.value);
        }

        [UnityTest]
        public IEnumerator DisablingObject_StopsTween()
        {
            _tween.start = 10f;
            _tween.end = 20f;

            _tween.Play();
            Assert.IsTrue(_tween.isPlaying);
            yield return new WaitForSeconds(0.5f);

            // disable the tween halfway through and check our values
            _tweenObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);

            // we wont be exact due to coroutines and editor stuff
            Assert.LessOrEqual(_tween.progress, 0.55f);
            // do the same lerp we would normally do to make sure it was updated with the exact value
            Assert.AreEqual(Mathf.Lerp(10f, 20f, _tween.progress), _tween.value);
            Assert.IsFalse(_tween.isPlaying);

            // go back to playing for the remainder
            _tweenObject.SetActive(true);
            _tween.Play();

            yield return new WaitForSeconds(0.5f);

            Assert.IsFalse(_tween.isPlaying);
            Assert.AreEqual(20f, _tween.value);
        }

        // test pausing
        // test stopping
    }
}
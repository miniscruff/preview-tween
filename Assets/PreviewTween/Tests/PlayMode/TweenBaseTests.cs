namespace PreviewTween
{
    using System;
    using System.Collections;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;
    using Object = UnityEngine.Object;

    public sealed class TestTween : TweenBase
    {
        private float _start;
        private float _end;
        private float _value;

        public float start
        {
            get { return _start; }
            set { _start = value; }
        }

        public float end
        {
            get { return _end; }
            set { _end = value; }
        }

        public float value { get { return _value; } }

        public override void RecordStart()
        {
            // Used by the editor so it doesnt matter here
        }

        public override void RecordEnd()
        {
            // Used by the editor so it doesnt matter here
        }

        protected override void UpdateValue(float smoothTime)
        {
            _value = Mathf.LerpUnclamped(start, end, smoothTime);
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
        public void Apply_ProgressZero_HasStartValue()
        {
            _tween.start = 10f;
            _tween.end = 25f;
            _tween.Apply();

            Assert.AreEqual(10f, _tween.value);
        }

        [Test]
        public void Apply_ProgressHalf_HasMiddleValue()
        {
            _tween.start = 10f;
            _tween.end = 20f;
            _tween.progress = 0.5f;
            _tween.Apply();

            Assert.AreEqual(15f, _tween.value);
        }

        [Test]
        public void Apply_ProgressOne_HasEndValue()
        {
            _tween.start = 10f;
            _tween.end = 25f;
            _tween.progress = 1f;
            _tween.Apply();

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

        [UnityTest]
        public IEnumerator Play_NegativeDirection_GoesToStart()
        {
            _tween.start = 10f;
            _tween.end = 20f;

            _tween.progress = 0.25f;
            _tween.direction = -1;

            _tween.Play();
            Assert.IsTrue(_tween.isPlaying);
            yield return new WaitForSeconds(0.25f);

            Assert.IsFalse(_tween.isPlaying);

            Assert.AreEqual(10f, _tween.value);
            Assert.AreEqual(0f, _tween.progress);
        }

        [UnityTest]
        public IEnumerator Play_CallingMoreThanOnce_DoesntEndFaster()
        {
            _tween.start = 10f;
            _tween.end = 20f;

            _tween.Play();
            _tween.Play();
            _tween.Play();
            _tween.Play();

            Assert.IsTrue(_tween.isPlaying);
            yield return new WaitForSeconds(0.5f);
            Assert.IsTrue(_tween.isPlaying);
            yield return new WaitForSeconds(0.5f);

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
        public IEnumerator PlayWithDelay_BeforeDelayStart_ThenNormal()
        {
            _tween.start = 10f;
            _tween.end = 20f;
            _tween.delay = 0.5f;

            _tween.Play();
            Assert.IsTrue(_tween.isPlaying);
            yield return new WaitForSeconds(0.25f);

            Assert.IsTrue(_tween.isPlaying);
            Assert.AreEqual(0f, _tween.progress);

            yield return new WaitForSeconds(0.3f);
            Assert.AreNotEqual(0f, _tween.progress);

            yield return new WaitForSeconds(1f);

            Assert.IsFalse(_tween.isPlaying);
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

        [UnityTest]
        public IEnumerator PlayWithLooping_RestartsProgressWhenCompleting()
        {
            _tween.start = 10f;
            _tween.end = 20f;
            _tween.wrapMode = WrapMode.Loop;

            _tween.Play();
            Assert.IsTrue(_tween.isPlaying);

            float lastProgress = 0f;
            while (true)
            {
                float newProgress = _tween.progress;
                if (newProgress > 1f)
                {
                    Assert.Fail();
                }
                if (newProgress < lastProgress)
                {
                    Assert.IsTrue(newProgress <= 0.1f);
                    break;
                }
                lastProgress = newProgress;
                yield return null;
            }

            Assert.IsTrue(_tween.isPlaying);
        }

        [UnityTest]
        public IEnumerator PlayWithPingPong_BouncesAtOneAndZero()
        {
            _tween.start = 10f;
            _tween.end = 20f;
            _tween.wrapMode = WrapMode.PingPong;

            _tween.Play();
            Assert.IsTrue(_tween.isPlaying);

            float lastProgress = 0f;
            // first check we start to go back down
            while (true)
            {
                float newProgress = _tween.progress;
                if (newProgress > 1f)
                {
                    Assert.Fail();
                }
                if (newProgress < lastProgress)
                {
                    Assert.IsTrue(newProgress >= 0.9f);
                    break;
                }
                lastProgress = newProgress;
                yield return null;
            }

            // then check we go back up
            lastProgress = 1f;
            while (true)
            {
                float newProgress = _tween.progress;
                if (newProgress < 0f)
                {
                    Assert.Fail();
                }
                if (newProgress > lastProgress)
                {
                    Assert.IsTrue(newProgress <= 0.1f);
                    break;
                }
                lastProgress = newProgress;
                yield return null;
            }

            Assert.IsTrue(_tween.isPlaying);
        }

        [UnityTest]
        public IEnumerator Toggle_GoesBackToStart()
        {
            _tween.start = 10f;
            _tween.end = 20f;

            _tween.Play();
            Assert.IsTrue(_tween.isPlaying);
            yield return new WaitForSeconds(0.25f);
            _tween.Toggle();
            yield return new WaitForSeconds(0.5f);

            Assert.IsFalse(_tween.isPlaying);

            Assert.AreEqual(10f, _tween.value);
        }

        [UnityTest]
        public IEnumerator Toggle_DoubleCall_GoesToEnd()
        {
            _tween.start = 10f;
            _tween.end = 20f;

            _tween.Play();
            Assert.IsTrue(_tween.isPlaying);
            yield return new WaitForSeconds(0.25f);
            _tween.Toggle();
            yield return new WaitForSeconds(0.1f);
            _tween.Toggle();
            yield return new WaitForSeconds(1f);

            Assert.IsFalse(_tween.isPlaying);

            Assert.AreEqual(20f, _tween.value);
        }

        [UnityTest]
        public IEnumerator Replay_StartsOverAfterDone()
        {
            _tween.start = 10f;
            _tween.end = 20f;
            _tween.duration = 0.25f;

            _tween.Play();
            Assert.IsTrue(_tween.isPlaying);
            yield return new WaitForSeconds(_tween.duration);

            Assert.IsFalse(_tween.isPlaying);
            Assert.AreEqual(20f, _tween.value);

            // now replay it
            _tween.Replay();
            Assert.IsTrue(_tween.isPlaying);
            Assert.AreEqual(10f, _tween.value);

            yield return new WaitForSeconds(1f);

            Assert.IsFalse(_tween.isPlaying);
            Assert.AreEqual(20f, _tween.value);
        }

        [UnityTest]
        public IEnumerator Replay_StartsOverInMiddle()
        {
            _tween.start = 10f;
            _tween.end = 20f;
            _tween.duration = 0.25f;

            _tween.Play();
            Assert.IsTrue(_tween.isPlaying);
            yield return new WaitForSeconds(_tween.duration / 2f);

            // now replay it
            _tween.Replay();
            Assert.IsTrue(_tween.isPlaying);
            Assert.AreEqual(10f, _tween.value);

            yield return new WaitForSeconds(_tween.duration);

            Assert.IsFalse(_tween.isPlaying);
            Assert.AreEqual(20f, _tween.value);
        }

        [Test]
        public void Apply_ProgressHalfWithEasing_HasMiddleValue()
        {
            _tween.start = 10f;
            _tween.end = 20f;
            _tween.progress = 0.5f;
            _tween.easingMode = EasingMode.QuadraticIn;
            _tween.Apply();

            Assert.AreEqual(12.5f, _tween.value);
        }

        [UnityTest]
        public IEnumerator Play_ExecutesEventOnComplete()
        {
            _tween.duration = 0.25f;

            bool wasCalled = false;
            _tween.onComplete.AddListener(() => wasCalled = true);

            _tween.Play();
            yield return new WaitForSeconds(_tween.duration);

            Assert.IsTrue(wasCalled);
        }

        [UnityTest]
        public IEnumerator Play_OnCompleteCalledForEachLoop()
        {
            _tween.duration = 0.25f;
            _tween.wrapMode = WrapMode.Loop;

            int calledCount = 0;
            _tween.onComplete.AddListener(() => calledCount++);

            _tween.Play();
            yield return new WaitForSeconds(_tween.duration * 5);

            Assert.AreEqual(5, calledCount);
        }

        [Test]
        public void Curve_SettingEasingToCustomUsesCurve()
        {
            AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
            curve.AddKey(0.25f, 0.4f); // add a random key so its not a simple ease in out

            _tween.start = 0f;
            _tween.end = 1f;
            _tween.progress = 0.6f;

            _tween.easingMode = EasingMode.CustomCurve;
            _tween.customCurve = curve;
            _tween.Apply();

            Assert.AreEqual(curve.Evaluate(0.6f), _tween.value);
        }

        [Test]
        public void CheckAllEasings()
        {
            const float progress = 0.25f;
            _tween.start = 0f;
            _tween.end = 1f;
            _tween.progress = progress;

            _tween.easingMode = EasingMode.Linear;
            _tween.Apply();
            Assert.IsTrue(Mathf.Approximately(Easings.Linear(progress), _tween.value));

            _tween.easingMode = EasingMode.QuadraticIn;
            _tween.Apply();
            Assert.IsTrue(Mathf.Approximately(Easings.Quadratic.In(progress), _tween.value));

            _tween.easingMode = EasingMode.QuadraticOut;
            _tween.Apply();
            Assert.IsTrue(Mathf.Approximately(Easings.Quadratic.Out(progress), _tween.value));

            _tween.easingMode = EasingMode.QuadraticInOut;
            _tween.Apply();
            Assert.IsTrue(Mathf.Approximately(Easings.Quadratic.InOut(progress), _tween.value));

            _tween.easingMode = EasingMode.CubicIn;
            _tween.Apply();
            Assert.IsTrue(Mathf.Approximately(Easings.Cubic.In(progress), _tween.value));

            _tween.easingMode = EasingMode.CubicOut;
            _tween.Apply();
            Assert.IsTrue(Mathf.Approximately(Easings.Cubic.Out(progress), _tween.value));

            _tween.easingMode = EasingMode.CubicInOut;
            _tween.Apply();
            Assert.IsTrue(Mathf.Approximately(Easings.Cubic.InOut(progress), _tween.value));
        }

        [Test]
        public void Delay_LessThanZero_ThrowsError()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _tween.delay = -2f);
            Assert.AreEqual(0f, _tween.delay);
        }

        [Test]
        public void Duration_LessThanMinimum_ThrowsError()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _tween.duration = TweenBase.minimum_duration - 1f);
            Assert.AreEqual(1f, _tween.duration);
        }

        [Test]
        public void Direction_NotOneOrNegativeOne_ThrowsError()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _tween.direction = 4);
            Assert.AreEqual(1, _tween.direction);
        }

        [Test]
        public void Tick_WithZeroDeltaTime_ThrowsError()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _tween.Tick(0f));
        }

        [Test]
        public void Tick_WithNegativeDeltaTime_ThrowsError()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _tween.Tick(-6f));
        }
    }
}
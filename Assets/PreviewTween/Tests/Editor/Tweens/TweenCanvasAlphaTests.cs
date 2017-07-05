namespace PreviewTween
{
    using NUnit.Framework;
    using UnityEngine;

    public sealed class TweenCanvasAlphaTests : SetupTweenTests<TweenCanvasAlpha>
    {
        public override void SetUp()
        {
            GameObject go = new GameObject("Tween");
            CanvasGroup group = go.AddComponent<CanvasGroup>();

            SetUp(go);

            tween.start = 0f;
            tween.end = 1f;
            tween.progress = 0.5f;

            Assert.AreEqual(group, tween.target);
        }

        [Test]
        public void SetsAlpha()
        {
            tween.Apply();
            Assert.AreEqual(0.5f, tween.target.alpha);
        }

        [Test]
        public void SeparateTarget()
        {
            GameObject target = new GameObject("Target");
            CanvasGroup group = target.AddComponent<CanvasGroup>();

            tween.target = group;
            tween.Apply();

            Assert.AreEqual(0.5f, group.alpha);

            Object.DestroyImmediate(target);
        }

        [Test]
        public void RecordStart()
        {
            tween.target.alpha = 0.2f;
            tween.RecordStart();

            Assert.AreEqual(0.2f, tween.start);
        }

        [Test]
        public void RecordEnd()
        {
            tween.target.alpha = 0.2f;
            tween.RecordEnd();

            Assert.AreEqual(0.2f, tween.end);
        }

        [Test]
        public void RecordStart_WithTarget()
        {
            GameObject target = new GameObject("Target");
            CanvasGroup group = target.AddComponent<CanvasGroup>();
            group.alpha = 0.75f;

            tween.target = group;
            tween.RecordStart();

            Assert.AreEqual(0.75f, tween.start);
            Object.DestroyImmediate(target);
        }

        [Test]
        public void RecordEnd_WithTarget()
        {
            GameObject target = new GameObject("Target");
            CanvasGroup group = target.AddComponent<CanvasGroup>();
            group.alpha = 0.555f;

            tween.target = group;
            tween.RecordStart();

            Assert.AreEqual(0.555f, tween.start);
            Object.DestroyImmediate(target);
        }

        [Test]
        public void ResetSetsStartAndEndToCurrentValue()
        {
            CanvasGroup group = tween.target;
            Object.DestroyImmediate(tween);

            group.alpha = 0.35f;
            TweenCanvasAlpha tempTween = gameObject.AddComponent<TweenCanvasAlpha>();

            Assert.AreEqual(0.35f, tempTween.start);
            Assert.AreEqual(0.35f, tempTween.end);
        }

        [Test]
        public void ResetWithNoConnectedCanvasGroup_UsesZeroAndOneForDefaults()
        {
            Object.DestroyImmediate(tween.target);
            Object.DestroyImmediate(tween);

            TweenCanvasAlpha tempTween = gameObject.AddComponent<TweenCanvasAlpha>();

            Assert.AreEqual(0f, tempTween.start);
            Assert.AreEqual(1f, tempTween.end);
        }
    }
}
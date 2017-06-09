namespace PreviewTween
{
    using NUnit.Framework;
    using UnityEngine;

    public sealed class TweenScaleTests : SetupTweenTests<TweenScale>
    {
        public override void SetUp()
        {
            base.SetUp();

            tween.start = new Vector3(10, 0, 0);
            tween.end = new Vector3(20, 0, 0);
            tween.progress = 0.5f;

            Assert.AreEqual(tween.transform, tween.target);
        }

        [Test]
        public void LocalScale()
        {
            tween.Apply();
            Assert.AreEqual(new Vector3(15, 0, 0), tween.transform.localScale);
        }

        [Test]
        public void SeperateTarget()
        {
            GameObject target = new GameObject("Target");
            tween.target = target.transform;

            tween.Apply();
            Assert.AreEqual(Vector3.one, tween.transform.localScale);
            Assert.AreEqual(new Vector3(15, 0, 0), target.transform.localScale);

            Object.DestroyImmediate(target);
        }


        [Test]
        public void RecordStart()
        {
            tween.transform.localScale = new Vector3(75f, 0f, 25f);
            tween.RecordStart();

            Assert.AreEqual(new Vector3(75f, 0f, 25f), tween.start);
        }

        [Test]
        public void RecordEnd()
        {
            tween.transform.localScale = new Vector3(2f, 4f, 6f);
            tween.RecordEnd();

            Assert.AreEqual(new Vector3(2f, 4f, 6f), tween.end);
        }

        [Test]
        public void RecordStart_WithTarget()
        {
            GameObject target = new GameObject("Target");
            target.transform.localScale = new Vector3(75f, 0f, 25f);

            tween.target = target.transform;
            tween.RecordStart();

            Assert.AreEqual(new Vector3(75f, 0f, 25f), tween.start);
            Object.DestroyImmediate(target);
        }

        [Test]
        public void RecordEnd_WithTarget()
        {
            GameObject target = new GameObject("Target");
            target.transform.localScale = new Vector3(2f, 4f, 6f);

            tween.target = target.transform;
            tween.RecordEnd();

            Assert.AreEqual(new Vector3(2f, 4f, 6f), tween.end);
            Object.DestroyImmediate(target);
        }

        [Test]
        public void ResetSetsStartAndEndToCurrentValue()
        {
            Object.DestroyImmediate(tween);

            gameObject.transform.localScale = new Vector3(12f, 14f, 16f);
            TweenScale tempTween = gameObject.AddComponent<TweenScale>();

            Assert.AreEqual(new Vector3(12f, 14f, 16f), tempTween.start);
            Assert.AreEqual(new Vector3(12f, 14f, 16f), tempTween.end);
        }
    }
}
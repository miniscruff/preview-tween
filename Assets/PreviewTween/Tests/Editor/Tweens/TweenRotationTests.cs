namespace PreviewTween
{
    using NUnit.Framework;
    using UnityEngine;

    public sealed class TweenRotationTests : SetupTweenTests<TweenRotation>
    {
        public override void SetUp()
        {
            base.SetUp();

            tween.start = Quaternion.Euler(90f, 0f, 0f);
            tween.end = Quaternion.Euler(180f, 0f, 0f);
            tween.progress = 0.5f;

            Assert.AreEqual(tween.gameObject.transform, tween.target);
        }

        [Test]
        public void WorldRotation()
        {
            tween.Apply();
            Assert.IsTrue(Quaternion.Euler(135f, 0f, 0f) == tween.transform.rotation);
        }

        [Test]
        public void SeperateTarget()
        {
            GameObject target = new GameObject("Target");
            tween.target = target.transform;

            tween.Apply();
            Assert.AreEqual(Quaternion.identity, tween.gameObject.transform.rotation);
            Assert.IsTrue(Quaternion.Euler(135f, 0f, 0f) == target.transform.rotation);

            Object.DestroyImmediate(target);
        }

        [Test]
        public void LocalSpace()
        {
            GameObject parent = new GameObject("Parent");
            parent.transform.rotation = Quaternion.Euler(45f, 0f, 45f);

            GameObject target = new GameObject("Target");
            target.transform.SetParent(parent.transform);

            tween.target = target.transform;
            tween.worldSpace = false;

            tween.Apply();
            Assert.AreEqual(Quaternion.identity, tween.gameObject.transform.rotation);
            Assert.IsTrue(Quaternion.Euler(135f, 0f, 0f) == target.transform.localRotation);
            Assert.IsFalse(Quaternion.Euler(135f, 0f, 0f) == target.transform.rotation);

            Object.DestroyImmediate(target);
            Object.DestroyImmediate(parent);
        }

        [Test]
        public void RecordStart()
        {
            tween.transform.rotation = Quaternion.Euler(15f, 0f, 15f);
            tween.RecordStart();

            Assert.AreEqual(Quaternion.Euler(15f, 0f, 15f), tween.start);
        }

        [Test]
        public void RecordEnd()
        {
            tween.transform.rotation = Quaternion.Euler(15f, 0f, 15f);
            tween.RecordEnd();

            Assert.AreEqual(Quaternion.Euler(15f, 0f, 15f), tween.end);
        }

        [Test]
        public void RecordStart_InLocalMode()
        {
            GameObject parent = new GameObject("Parent");
            parent.transform.rotation = Quaternion.Euler(15f, 0f, 15f);

            tween.transform.SetParent(parent.transform);
            tween.worldSpace = false;
            tween.transform.localRotation = Quaternion.Euler(-90f, 0f, 45f);
            tween.RecordStart();

            Assert.IsTrue(Quaternion.Euler(-90f, 0f, 45f) == tween.start);
            Assert.AreNotEqual(tween.transform.rotation, tween.start);

            Object.DestroyImmediate(parent);
        }

        [Test]
        public void RecordEnd_InLocalMode()
        {
            GameObject parent = new GameObject("Parent");
            parent.transform.rotation = Quaternion.Euler(15f, 0f, 15f);

            tween.transform.SetParent(parent.transform);
            tween.worldSpace = false;
            tween.transform.localRotation = Quaternion.Euler(-90f, 0f, 45f);
            tween.RecordEnd();

            Assert.IsTrue(Quaternion.Euler(-90f, 0f, 45f) == tween.end);
            Assert.AreNotEqual(tween.transform.rotation, tween.end);

            Object.DestroyImmediate(parent);
        }

        [Test]
        public void ResetSetsStartAndEndToCurrentValue()
        {
            Object.DestroyImmediate(tween);

            gameObject.transform.rotation = Quaternion.Euler(0f, 45f, -45f);
            TweenRotation tempTween = gameObject.AddComponent<TweenRotation>();

            Assert.AreEqual(Quaternion.Euler(0f, 45f, -45f), tempTween.start);
            Assert.AreEqual(Quaternion.Euler(0f, 45f, -45f), tempTween.end);
        }
    }
}
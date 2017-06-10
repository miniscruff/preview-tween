namespace PreviewTween
{
    using NUnit.Framework;
    using UnityEngine;

    public sealed class TweenRotationTests : SetupTweenTests<TweenRotation>
    {
        public override void SetUp()
        {
            base.SetUp();

            tween.start = new Vector3(90f, 0f, 0f);
            tween.end = new Vector3(180f, 0f, 0f);
            tween.progress = 0.5f;

            Assert.AreEqual(tween.transform, tween.target);
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
            Assert.AreEqual(Quaternion.identity, tween.transform.rotation);
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
            Assert.AreEqual(Quaternion.identity, tween.transform.rotation);
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

            Assert.IsTrue(new Vector3(15f, 0f, 15f) == tween.start);
        }

        [Test]
        public void RecordEnd()
        {
            tween.transform.rotation = Quaternion.Euler(15f, 0f, 15f);
            tween.RecordEnd();

            Assert.IsTrue(new Vector3(15f, 0f, 15f) == tween.end);
        }

        [Test]
        public void RecordStart_WithTarget()
        {
            GameObject target = new GameObject("Target");
            target.transform.rotation = Quaternion.Euler(15f, 0f, 15f);

            tween.target = target.transform;
            tween.RecordStart();

            Assert.IsTrue(new Vector3(15f, 0f, 15f) == tween.start);
            Object.DestroyImmediate(target);
        }

        [Test]
        public void RecordEnd_WithTarget()
        {
            GameObject target = new GameObject("Target");
            target.transform.rotation = Quaternion.Euler(15f, 0f, 15f);

            tween.target = target.transform;
            tween.RecordEnd();

            Assert.IsTrue(new Vector3(15f, 0f, 15f) == tween.end);
            Object.DestroyImmediate(target);
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

            Assert.IsTrue(Quaternion.Euler(-90f, 0f, 45f).eulerAngles == tween.start);
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

            Assert.IsTrue(Quaternion.Euler(-90f, 0f, 45f).eulerAngles == tween.end);
            Assert.AreNotEqual(tween.transform.rotation, tween.end);

            Object.DestroyImmediate(parent);
        }

        [Test]
        public void ResetSetsStartAndEndToCurrentValue()
        {
            Object.DestroyImmediate(tween);

            gameObject.transform.rotation = Quaternion.Euler(0f, 45f, 45f);
            TweenRotation tempTween = gameObject.AddComponent<TweenRotation>();

            Assert.IsTrue(new Vector3(0f, 45f, 45f) == tempTween.start);
            Assert.IsTrue(new Vector3(0f, 45f, 45f) == tempTween.end);
        }
    }
}
namespace PreviewTween
{
    using NUnit.Framework;
    using UnityEngine;

    public sealed class TweenPositionTests : SetupTweenTests<TweenPosition>
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
        public void WorldPosition()
        {
            tween.Apply();
            Assert.AreEqual(new Vector3(15, 0, 0), tween.transform.position);
        }

        [Test]
        public void SeperateTarget()
        {
            GameObject target = new GameObject("Target");
            tween.target = target.transform;

            tween.Apply();
            Assert.AreEqual(Vector3.zero, tween.transform.position);
            Assert.AreEqual(new Vector3(15, 0, 0), target.transform.position);

            Object.DestroyImmediate(target);
        }

        [Test]
        public void LocalSpace()
        {
            GameObject parent = new GameObject("Parent");
            parent.transform.position = new Vector3(-10, 10, -10);

            GameObject target = new GameObject("Target");
            target.transform.SetParent(parent.transform);

            tween.target = target.transform;
            tween.worldSpace = false;

            tween.Apply();
            Assert.AreEqual(Vector3.zero, tween.transform.position);
            Assert.AreEqual(new Vector3(15, 0, 0), target.transform.localPosition);
            Assert.AreNotEqual(new Vector3(15, 0, 0), target.transform.position);

            Object.DestroyImmediate(target);
            Object.DestroyImmediate(parent);
        }

        [Test]
        public void RecordStart()
        {
            tween.transform.position = new Vector3(75f, 0f, 25f);
            tween.RecordStart();

            Assert.AreEqual(new Vector3(75f, 0f, 25f), tween.start);
        }

        [Test]
        public void RecordEnd()
        {
            tween.transform.position = new Vector3(2f, 4f, 6f);
            tween.RecordEnd();

            Assert.AreEqual(new Vector3(2f, 4f, 6f), tween.end);
        }

        [Test]
        public void RecordStart_WithTarget()
        {
            GameObject target = new GameObject("Target");
            target.transform.position = new Vector3(75f, 0f, 25f);

            tween.target = target.transform;
            tween.RecordStart();

            Assert.AreEqual(new Vector3(75f, 0f, 25f), tween.start);
            Object.DestroyImmediate(target);
        }

        [Test]
        public void RecordEnd_WithTarget()
        {
            GameObject target = new GameObject("Target");
            target.transform.position = new Vector3(2f, 4f, 6f);

            tween.target = target.transform;
            tween.RecordEnd();

            Assert.AreEqual(new Vector3(2f, 4f, 6f), tween.end);
            Object.DestroyImmediate(target);
        }

        [Test]
        public void RecordStart_InLocalMode()
        {
            GameObject parent = new GameObject("Parent");
            parent.transform.position = new Vector3(-10, 10, -10);

            tween.transform.SetParent(parent.transform);
            tween.worldSpace = false;
            tween.transform.localPosition = new Vector3(6f, 2f, 4f);
            tween.RecordStart();

            Assert.AreEqual(new Vector3(6f, 2f, 4f), tween.start);
            Assert.AreNotEqual(tween.transform.position, tween.start);

            Object.DestroyImmediate(parent);
        }

        [Test]
        public void RecordEnd_InLocalMode()
        {
            GameObject parent = new GameObject("Parent");
            parent.transform.position = new Vector3(-10, 10, -10);

            tween.transform.SetParent(parent.transform);
            tween.worldSpace = false;
            tween.transform.localPosition = new Vector3(6f, 2f, 4f);
            tween.RecordEnd();

            Assert.AreEqual(new Vector3(6f, 2f, 4f), tween.end);
            Assert.AreNotEqual(tween.transform.position, tween.end);

            Object.DestroyImmediate(parent);
        }

        [Test]
        public void ResetSetsStartAndEndToCurrentValue()
        {
            Object.DestroyImmediate(tween);

            gameObject.transform.position = new Vector3(12f, 14f, 16f);
            TweenPosition tempTween = gameObject.AddComponent<TweenPosition>();

            Assert.AreEqual(new Vector3(12f, 14f, 16f), tempTween.start);
            Assert.AreEqual(new Vector3(12f, 14f, 16f), tempTween.end);
        }
    }
}
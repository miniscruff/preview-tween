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

            Assert.AreEqual(tween.gameObject.transform, tween.target);
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
            Assert.AreEqual(Vector3.zero, tween.gameObject.transform.position);
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
            Assert.AreEqual(Vector3.zero, tween.gameObject.transform.position);
            Assert.AreEqual(new Vector3(15, 0, 0), target.transform.localPosition);
            Assert.AreNotEqual(new Vector3(15, 0, 0), target.transform.position);

            Object.DestroyImmediate(target);
        }
    }
}
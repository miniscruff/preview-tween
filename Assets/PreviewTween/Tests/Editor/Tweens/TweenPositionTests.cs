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
        }

        [Test]
        public void WorldPosition()
        {
            tween.Apply();
            Assert.AreEqual(new Vector3(15, 0, 0), tween.transform.position);
        }
    }
}
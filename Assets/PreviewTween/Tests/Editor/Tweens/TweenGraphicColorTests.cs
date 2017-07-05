namespace PreviewTween
{
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.UI;

    public sealed class TweenGraphicColorTests : SetupTweenTests<TweenGraphicColor>
    {
        public override void SetUp()
        {
            GameObject go = new GameObject("Tween");
            // use Image as our graphic as graphic is abstract
            Graphic graphic = go.AddComponent<Image>();

            SetUp(go);

            tween.gradient = new Gradient
            {
                colorKeys = new [] {
                    new GradientColorKey(Color.white, 0f),
                    new GradientColorKey(Color.black, 1f)
                },
                alphaKeys = new[] {
                    new GradientAlphaKey(0f, 0f),
                    new GradientAlphaKey(1f, 1f)
                },
                mode = GradientMode.Blend
            };
            tween.progress = 0.5f;

            Assert.AreEqual(graphic, tween.target);
        }

        [Test]
        public void SetsColor()
        {
            tween.Apply();
            Assert.AreEqual(new Color(0.5f, 0.5f, 0.5f, 0.5f), tween.target.color);
        }

        [Test]
        public void SeparateTarget()
        {
            GameObject target = new GameObject("Target");
            Graphic graphic = target.AddComponent<Image>();

            tween.target = graphic;
            tween.Apply();

            Assert.AreEqual(new Color(0.5f, 0.5f, 0.5f, 0.5f), graphic.color);

            Object.DestroyImmediate(target);
        }

        [Test]
        public void RecordStart()
        {
            tween.target.color = Color.blue;
            tween.RecordStart();

            Assert.AreEqual(Color.blue, tween.gradient.Evaluate(0f));
        }

        [Test]
        public void RecordEnd()
        {
            tween.target.color = Color.red;
            tween.RecordEnd();

            Assert.AreEqual(Color.red, tween.gradient.Evaluate(1f));
        }

        [Test]
        public void RecordStart_WithTarget()
        {
            GameObject target = new GameObject("Target");
            Graphic graphic = target.AddComponent<Image>();
            graphic.color = Color.green;

            tween.target = graphic;
            tween.RecordStart();

            Assert.AreEqual(Color.green, tween.gradient.Evaluate(0f));
            Object.DestroyImmediate(target);
        }

        [Test]
        public void RecordEnd_WithTarget()
        {
            GameObject target = new GameObject("Target");
            Graphic graphic = target.AddComponent<Image>();
            graphic.color = Color.white;

            tween.target = graphic;
            tween.RecordEnd();

            Assert.AreEqual(Color.white, tween.gradient.Evaluate(1f));
            Object.DestroyImmediate(target);
        }

        [Test]
        public void ResetSetsStartAndEndToCurrentValue()
        {
            Graphic graphic = tween.target;
            Object.DestroyImmediate(tween);

            graphic.color = Color.magenta;
            TweenGraphicColor tempTween = gameObject.AddComponent<TweenGraphicColor>();

            Assert.AreEqual(Color.magenta, tempTween.gradient.Evaluate(0f));
            Assert.AreEqual(Color.magenta, tempTween.gradient.Evaluate(1f));
        }

        [Test]
        public void ResetWithNoConnectedCanvasGroup_UsesWhiteAndBlackForDefault()
        {
            Object.DestroyImmediate(tween.target);
            Object.DestroyImmediate(tween);

            TweenGraphicColor tempTween = gameObject.AddComponent<TweenGraphicColor>();

            Assert.AreEqual(Color.white, tempTween.gradient.Evaluate(0f));
            Assert.AreEqual(Color.black, tempTween.gradient.Evaluate(1f));
        }
    }
}
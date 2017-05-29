namespace PreviewTween
{
    using System;
    using System.IO;
    using NUnit.Framework;
    using UnityEngine;

    public sealed class EasingsTests
    {
        private static void CompareEasingToCache(Func<float, float> easingFunc, string filePath)
        {
            const int width = 100;
            const int height = 100;

            Texture2D texture = new Texture2D(width, height * 2, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point,
            };

            for (float x = 0; x < width; x += 0.1f)
            {
                int y = Mathf.RoundToInt(easingFunc(x / width) * 100) + height / 2;
                texture.SetPixel((int)x, y, Color.black);
            }

            byte[] pngBytes = texture.EncodeToPNG();
            string fullPath = "Assets/Tests/Editor/EasingComparisons/" + filePath + ".png";
            if (File.Exists(fullPath))
            {
                byte[] existingImage = File.ReadAllBytes(fullPath);
                CollectionAssert.AreEqual(existingImage, pngBytes);
            }
            else
            {
                // if we dont have the image already, this will create it
                // it is up to the creator to verify the image is accurate after making it
                // if its not accurate, edit the algorithm and delete the image to remake the comparison image
                File.WriteAllBytes(fullPath, pngBytes);
            }
        }

        [Test]
        public void Linear()
        {
            CompareEasingToCache(Easings.Linear, "Linear");
        }

        [Test]
        public void QuadraticIn()
        {
            CompareEasingToCache(Easings.QuadraticIn, "QuadraticIn");
        }

        [Test]
        public void QuadraticOut()
        {
            CompareEasingToCache(Easings.QuadraticOut, "QuadraticOut");
        }

        [Test]
        public void QuadraticInOut()
        {
            CompareEasingToCache(Easings.QuadraticInOut, "QuadraticInOut");
        }
    }
}
namespace PreviewTween
{
    using System;
    using System.IO;
    using NUnit.Framework;
    using UnityEditor;
    using UnityEngine;

    public sealed class EasingsTests
    {
        private static void CompareEasingToCache(Func<float, float> easingFunc, string filePath)
        {
            CompareEasingToCache(easingFunc, filePath, 100, 0);
        }

        private static void CompareEasingToCache(Func<float, float> easingFunc, string filePath, int height)
        {
            CompareEasingToCache(easingFunc, filePath, height, 0);
        }

        private static void CompareEasingToCache(Func<float, float> easingFunc, string filePath, int height, int startingHeight)
        {
            const int width = 100;
            const int width_buffer = 5;

            // we need to subtract 1 so that a value of 1 on our easing is height - 1 on our texture
            float lerpHeight = height - startingHeight - 1f;

            // add a few pixels of width so we can use a value of 1 and not go out of range
            Texture2D texture = new Texture2D(width + width_buffer * 2, height, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point
            };

            for (float x = 0; x <= width; x += 0.25f)
            {
                float progress = x / width;
                float factor = easingFunc(progress);

                // our easing function should always be between 0 and 1
                Assert.IsTrue(factor >= 0f);
                Assert.IsTrue(factor <= 1f);

                int y = Mathf.RoundToInt(Mathf.Lerp(0f, lerpHeight, factor) + startingHeight);
                Assert.IsTrue(y < height);

                // set the value to black on our texture
                texture.SetPixel((int)x + width_buffer, y, Color.black);
            }

            byte[] pngBytes = texture.EncodeToPNG();
            Texture2D existingTexture = Resources.Load<Texture2D>("Easings/" + filePath);
            if (existingTexture != null)
            {
                byte[] existingTextureBytes = existingTexture.EncodeToPNG();
                CollectionAssert.AreEqual(existingTextureBytes, pngBytes);
            }
            else
            {
                // if we dont have the image already, this will create it
                // it is up to the creator to verify the image is accurate after making it
                // if its not accurate, edit the algorithm and delete the image to remake the comparison image
                string fullPath = "Assets/PreviewTween/Editor/Resources/Easings/" + filePath + ".png";
                File.WriteAllBytes(fullPath, pngBytes);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
                // Note: The path is hard coded so if you move the preview tween folder inside another one it will fail currently

                // we have to setup our import settings so we can use it for our easings dialog and to read it in future tests
                TextureImporter textureImporter = AssetImporter.GetAtPath(fullPath) as TextureImporter;
                textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                textureImporter.isReadable = true;
                textureImporter.mipmapEnabled = false;
                textureImporter.npotScale = TextureImporterNPOTScale.None;
                textureImporter.SaveAndReimport();
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
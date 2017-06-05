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
            GenerateBaesTexture(texture, width, height, width_buffer, startingHeight);

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
                texture.SetPixel((int)x + width_buffer, y, Color.green);

                // make it a double thick line
                if (y > 0)
                {
                    texture.SetPixel((int)x + width_buffer, y - 1, Color.green);
                }
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

                string easingsPath = EditorHelper.GetProjectDirectory("/Editor/Graphics/Easings/");
                string fullPath = easingsPath + filePath + ".png";
                File.WriteAllBytes(fullPath, pngBytes);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

                // we have to setup our import settings so we can use it for our easings dialog and to read it in future tests
                TextureImporter textureImporter = AssetImporter.GetAtPath(fullPath) as TextureImporter;
                textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                textureImporter.isReadable = true;
                textureImporter.mipmapEnabled = false;
                textureImporter.alphaIsTransparency = true;
                textureImporter.npotScale = TextureImporterNPOTScale.None;
                textureImporter.SaveAndReimport();
            }
        }

        private static void GenerateBaesTexture(Texture2D texture, int width, int height, int widthBuffer, int startingHeight)
        {
            for (int x = 0; x < width + widthBuffer * 2; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (y == startingHeight || y == height - startingHeight - 1 || x == widthBuffer || x == width + widthBuffer)
                    {
                        texture.SetPixel(x, y, Color.black);
                    }
                    else
                    {
                        texture.SetPixel(x, y, Color.clear);
                    }
                }
            }
        }

        [Test]
        public void Linear()
        {
            CompareEasingToCache(Easings.Linear, "Linear");
        }

        [Test]
        public void Quadratic_In()
        {
            CompareEasingToCache(Easings.Quadratic.In, "QuadraticIn");
        }

        [Test]
        public void Quadratic_Out()
        {
            CompareEasingToCache(Easings.Quadratic.Out, "QuadraticOut");
        }

        [Test]
        public void Quadratic_InOut()
        {
            CompareEasingToCache(Easings.Quadratic.InOut, "QuadraticInOut");
        }

        [Test]
        public void Cubic_In()
        {
            CompareEasingToCache(Easings.Cubic.In, "CubicIn");
        }

        [Test]
        public void Cubic_Out()
        {
            CompareEasingToCache(Easings.Cubic.Out, "CubicOut");
        }

        [Test]
        public void Cubic_InOut()
        {
            CompareEasingToCache(Easings.Cubic.InOut, "CubicInOut");
        }
    }
}
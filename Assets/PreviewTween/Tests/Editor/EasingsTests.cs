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

        private static void CompareEasingToCache(Func<float, float> easingFunc, string filePath, int imageheight, int startingHeight)
        {
            const int lerp_height = 99;
            const int width = 100;
            const int width_buffer = 5;

            // add a few pixels of width so we can use a value of 1 and not go out of range
            Texture2D texture = new Texture2D(width + width_buffer * 2, imageheight, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point
            };
            GenerateBaseTexture(texture, width, imageheight, width_buffer, startingHeight);

            for (float x = 0; x <= width; x += 0.25f)
            {
                float progress = x / width;
                float factor = easingFunc(progress);

                int y = Mathf.RoundToInt(factor * lerp_height + startingHeight);
                Assert.IsTrue(y < imageheight);

                // set the value to black on our texture
                texture.SetPixel((int)x + width_buffer, y, Color.green);

                // make it a double thick line
                if (y > 0)
                {
                    texture.SetPixel((int)x + width_buffer, y - 1, Color.green);
                }
            }

            byte[] pngBytes = texture.EncodeToPNG();

            string easingsPath = EditorHelper.GetProjectDirectory("/Editor/Graphics/Easings/");
            string fullPath = easingsPath + filePath + ".png";
            if (File.Exists(fullPath))
            {
                byte[] existingTextureBytes = File.ReadAllBytes(fullPath);
                CollectionAssert.AreEqual(existingTextureBytes, pngBytes);
            }
            else
            {
                // if we dont have the image already, this will create it
                // it is up to the creator to verify the image is accurate after making it
                // if its not accurate, edit the algorithm and delete the image to remake the comparison image
                File.WriteAllBytes(fullPath, pngBytes);
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

                // we have to setup our import settings so we can use it for our easings display and in future tests
                TextureImporter textureImporter = AssetImporter.GetAtPath(fullPath) as TextureImporter;
                textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                textureImporter.isReadable = true;
                textureImporter.mipmapEnabled = false;
                textureImporter.alphaIsTransparency = true;
                textureImporter.npotScale = TextureImporterNPOTScale.None;
                textureImporter.SaveAndReimport();
            }
        }

        private static void GenerateBaseTexture(Texture2D texture, int width, int height, int widthBuffer, int startingHeight)
        {
            for (int x = 0; x < width + widthBuffer * 2; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (y == startingHeight || y == 100 + startingHeight - 1 || x == widthBuffer || x == width + widthBuffer)
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

        [Test]
        public void Quartic_In()
        {
            CompareEasingToCache(Easings.Quartic.In, "QuarticIn");
        }

        [Test]
        public void Quartic_Out()
        {
            CompareEasingToCache(Easings.Quartic.Out, "QuarticOut");
        }

        [Test]
        public void Quartic_InOut()
        {
            CompareEasingToCache(Easings.Quartic.InOut, "QuarticInOut");
        }

        [Test]
        public void Quintic_In()
        {
            CompareEasingToCache(Easings.Quintic.In, "QuinticIn");
        }

        [Test]
        public void Quintic_Out()
        {
            CompareEasingToCache(Easings.Quintic.Out, "QuinticOut");
        }

        [Test]
        public void Quintic_InOut()
        {
            CompareEasingToCache(Easings.Quintic.InOut, "QuinticInOut");
        }

        [Test]
        public void Bounce_In()
        {
            CompareEasingToCache(Easings.Bounce.In, "BounceIn");
        }

        [Test]
        public void Bounce_Out()
        {
            CompareEasingToCache(Easings.Bounce.Out, "BounceOut");
        }

        [Test]
        public void Bounce_InOut()
        {
            CompareEasingToCache(Easings.Bounce.InOut, "BounceInOut");
        }

        [Test]
        public void Back_In()
        {
            CompareEasingToCache(Easings.Back.In, "BackIn", 120, 20);
        }

        [Test]
        public void Back_Out()
        {
            CompareEasingToCache(Easings.Back.Out, "BackOut", 120, 0);
        }

        [Test]
        public void Back_InOut()
        {
            CompareEasingToCache(Easings.Back.InOut, "BackInOut", 140, 20);
        }
    }
}
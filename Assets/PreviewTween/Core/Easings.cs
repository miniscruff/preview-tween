namespace PreviewTween
{
    public static class Easings
    {
        public static float Linear(float time)
        {
            return time;
        }

        public static class Quadratic
        {
            public static float In(float time)
            {
                return time * time;
            }

            public static float Out(float time)
            {
                return -time * (time - 2f);
            }

            public static float InOut(float time)
            {
                time *= 2f;
                if (time < 1f)
                {
                    return 0.5f * time * time;
                }
                time--;
                return -0.5f * (time * (time - 2) - 1f);
            }
        }

        public static class Cubic
        {
            public static float In(float time)
            {
                return time * time * time;
            }

            public static float Out(float time)
            {
                time -= 1f;
                return time * time * time + 1;
            }

            public static float InOut(float time)
            {
                time *= 2f;
                if (time < 1f)
                {
                    return 0.5f * time * time * time;
                }
                time -= 2f;
                return 0.5f * (time * time * time + 2f);
            }
        }

        // QuarticIn
        // QuarticOut
        // QuarticInOut

        // QuinticIn
        // QuinticOut
        // QuinticInOut

        // ExponentialIn
        // ExponentialOut
        // ExponentialInOut

        // BounceIn
        // BounceOut
        // BounceInOut
    }
}
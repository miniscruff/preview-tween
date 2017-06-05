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

        public static class Quartic
        {
            public static float In(float time)
            {
                return time * time * time * time;
            }

            public static float Out(float time)
            {
                time -= 1f;
                return -(time * time * time * time - 1f);
            }

            public static float InOut(float time)
            {
                time *= 2f;
                if (time < 1f)
                {
                    return 0.5f * time * time * time * time;
                }
                time -= 2f;
                return -0.5f * (time * time * time * time - 2f);
            }
        }

        public static class Quintic
        {
            public static float In(float time)
            {
                return time * time * time * time * time;
            }

            public static float Out(float time)
            {
                time -= 1f;
                return time * time * time * time * time + 1f;
            }

            public static float InOut(float time)
            {
                time *= 2f;
                if (time < 1f)
                {
                    return 0.5f * time * time * time * time * time;
                }
                time -= 2f;
                return 0.5f * (time * time * time * time * time + 2f);
            }
        }

        public static class Bounce
        {
            public static float In(float time)
            {
                return 1f - Out(1f - time);
            }

            public static float Out(float time)
            {
                if (time < 1f / 2.75f)
                {
                    return 7.5625f * time * time;
                }
                if (time < 2 / 2.75)
                {
                    time -= 1.5f / 2.75f;
                    return 7.5625f * time * time + 0.75f;
                }
                if (time < 2.5 / 2.75)
                {
                    time -= 2.25f / 2.75f;
                    return 7.5625f * time * time + 0.9375f;
                }
                time -= 2.625f / 2.75f;
                return 7.5625f * time * time + 0.984375f;
            }

            public static float InOut(float time)
            {
                if (time < 0.5f)
                {
                    return In(time * 2f) * 0.5f;
                }
                return Out(time * 2 - 1f) * 0.5f + 0.5f;
            }
        }

        public static class Back
        {
            private const float overshoot = 1.70158f;

            public static float In(float time)
            {
                return time * time * ((overshoot + 1) * time - overshoot);
            }

            public static float Out(float time)
            {
                time -= 1f;
                return time * time * ((overshoot + 1) * time + overshoot) + 1;
            }

            public static float InOut(float time)
            {
                const float in_out_overshoot = overshoot * 1.525f;

                time *= 2f;
                if (time < 1f)
                {
                    return 0.5f * time * time * ((in_out_overshoot + 1f) * time - in_out_overshoot);
                }

                time -= 2f;
                return 0.5f * (time * time * ((in_out_overshoot + 1f) * time + in_out_overshoot) + 2f);
            }
        }
    }
}
namespace PreviewTween
{
    public enum Easing
    {
        Linear,
        QuadraticIn,
        QuadraticOut,
        QuadraticInOut
    }

    public static class Easings
    {
        public static float Linear(float time)
        {
            return time;
        }

        public static float QuadraticIn(float time)
        {
            return time * time;
        }

        public static float QuadraticOut(float time)
        {
            return -time * (time - 2f);
        }

        public static float QuadraticInOut(float time)
        {
            time *= 2f;
            if (time < 1f)
            {
                return 0.5f * time * time;
            }

            time--;
            return -0.5f * (time * (time - 2) - 1f);
        }

        // NOTES:
        // t = time
        // b = 0 (start)
        // c = 1 (delta)
        // d = 1 (duration)
    }
}
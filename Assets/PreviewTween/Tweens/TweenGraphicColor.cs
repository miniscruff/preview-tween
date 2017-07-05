namespace PreviewTween
{
    using UnityEngine;
    using UnityEngine.UI;

    public sealed class TweenGraphicColor : TweenBase
    {
        [SerializeField] private Graphic _target;
        [SerializeField] private Gradient _gradient;

        public Graphic target
        {
            get { return _target; }
            set { _target = value; }
        }

        public Gradient gradient
        {
            get { return _gradient; }
            set { _gradient = value; }
        }

        private void Reset()
        {
            _target = GetComponent<Graphic>();

            GradientColorKey[] colorKeys = new GradientColorKey[2];
            if (_target != null)
            {
                colorKeys[0] = new GradientColorKey(_target.color, 0f);
                colorKeys[1] = new GradientColorKey(_target.color, 1f);
            }
            else
            {
                colorKeys[0] = new GradientColorKey(Color.white, 0f);
                colorKeys[1] = new GradientColorKey(Color.black, 1f);
            }

            GradientAlphaKey[] alphaKeys = {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            };

            _gradient = new Gradient {
                colorKeys = colorKeys,
                alphaKeys = alphaKeys,
                mode = GradientMode.Blend
            };
        }

        public override void RecordStart()
        {
            GradientColorKey[] colorKeys = _gradient.colorKeys;
            colorKeys[0].color = _target.color;

            GradientAlphaKey[] alphaKeys = _gradient.alphaKeys;
            alphaKeys[0].alpha = _target.color.a;

            _gradient.SetKeys(colorKeys, alphaKeys);
        }

        public override void RecordEnd()
        {
            GradientColorKey[] colorKeys = _gradient.colorKeys;
            colorKeys[colorKeys.Length - 1].color = _target.color;

            GradientAlphaKey[] alphaKeys = _gradient.alphaKeys;
            alphaKeys[alphaKeys.Length - 1].alpha = _target.color.a;

            _gradient.SetKeys(colorKeys, alphaKeys);
        }

        protected override void UpdateValue(float smoothTime)
        {
            _target.color = _gradient.Evaluate(smoothTime);
        }
    }
}
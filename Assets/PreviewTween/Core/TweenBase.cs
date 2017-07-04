namespace PreviewTween
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// How the tween should initially start playing
    /// </summary>
    public enum PlayMode
    {
        /// <summary>
        /// Doesnt play automatically, call PlayForward through script or an event
        /// </summary>
        None,
        /// <summary>
        /// Uses the built in Start function to play
        /// </summary>
        Start,
        /// <summary>
        /// Uses the built in OnEnable function to play
        /// </summary>
        OnEnable
    }

    /// <summary>
    /// How the tween should handle reaching the end point
    /// </summary>
    public enum WrapMode
    {
        /// <summary>
        /// Stops, keeping the end value
        /// </summary>
        Once,
        /// <summary>
        /// Starts over from the beginning
        /// </summary>
        Loop,
        /// <summary>
        /// Reverses direction and easing
        /// </summary>
        PingPong
    }

    /// <summary>
    /// Built in supported easings, use custom if you want to define something else.
    /// Note: See easing graphics for a visual of what the easing will do in the app
    /// </summary>
    public enum EasingMode
    {
        Linear,
        // Back
        BackIn,
        BackOut,
        BackInOut,
        // Bounce
        BounceIn,
        BounceOut,
        BounceInOut,
        // Cubic
        CubicIn,
        CubicOut,
        CubicInOut,
        // Quadratic
        QuadraticIn,
        QuadraticOut,
        QuadraticInOut,
        // Quartic
        QuarticIn,
        QuarticOut,
        QuarticInOut,
        // Qunitic
        QuinticIn,
        QuinticOut,
        QuinticInOut,
        // Other
        CustomCurve
    }

    /// <summary>
    /// Base abstract class for all tweens
    /// </summary>
    public abstract class TweenBase : MonoBehaviour
    {
        /// <summary>
        /// Smallest value our duration can have, checked by script and in the editor
        /// </summary>
        public const float minimum_duration = 0.1f;

        // tween values
        private float _progress;
        private bool _isPlaying;
        private int _direction = 1;

        // settings
        [SerializeField] private float _delay;
        [SerializeField] private float _duration = 1f;
        [SerializeField] private PlayMode _playMode;
        [SerializeField] private WrapMode _wrapMode;
        [SerializeField] private EasingMode _easingMode;
        [SerializeField] private AnimationCurve _customCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private UnityEvent _onComplete = new UnityEvent();

        /// <summary>
        /// The current progress of the tween from 0 (start) to 1 (end).
        /// If you manually change this value you will need to call Apply for it to take effect.
        /// </summary>
        public float progress
        {
            get { return _progress; }
            set { _progress = value; }
        }

        /// <summary>
        /// Whether or not the tween is currently playing
        /// </summary>
        public bool isPlaying
        {
            get { return _isPlaying; }
        }

        /// <summary>
        /// The current direction of the tween where 1 is forward and -1 is in reverse.
        /// You can not set this value to anything other than 1 and -1 or an error will occur.
        /// </summary>
        public int direction
        {
            get { return _direction; }
            set
            {
                if (value != 1 && value != -1)
                {
                    throw new ArgumentOutOfRangeException("value", "Cant set direction to a value other than 1 and -1");
                }
                _direction = value;
            }
        }

        /// <summary>
        /// The amount of time in seconds before the tween actually begins.
        /// This value is applied to everytime the tween starts again.
        /// Must be >= 0.
        /// </summary>
        public float delay
        {
            get { return _delay; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", "Cant set delay to a value < 0");
                }
                _delay = value;
            }
        }

        /// <summary>
        /// The total time in seconds the tween will take place in.
        /// Will cause jumpy behavior if you drastically change this value while its playing.
        /// </summary>
        public float duration
        {
            get { return _duration; }
            set
            {
                if (value < minimum_duration)
                {
                    throw new ArgumentOutOfRangeException("value", "Cant set duration to a value < " + minimum_duration);
                }
                _duration = value;
            }
        }

        /// <summary>
        /// Current play mode, reference <see cref="PlayMode"/>.
        /// </summary>
        public PlayMode playMode
        {
            get { return _playMode; }
            set { _playMode = value; }
        }

        /// <summary>
        /// Current wrap mode, reference <see cref="WrapMode"/>
        /// </summary>
        public WrapMode wrapMode
        {
            get { return _wrapMode; }
            set { _wrapMode = value; }
        }

        /// <summary>
        /// Current easing mode, reference <see cref="EasingMode"/>
        /// </summary>
        public EasingMode easingMode
        {
            get { return _easingMode; }
            set { _easingMode = value; }
        }

        /// <summary>
        /// Animation curve used when our easing mode is set to Custom.
        /// Has no effect if our easing is not custom.
        /// </summary>
        public AnimationCurve customCurve
        {
            get { return _customCurve; }
            set { _customCurve = value; }
        }

        /// <summary>
        /// Event executed after the tween is complete,
        /// called after each run if wrap mode is loop or ping pong
        /// </summary>
        public UnityEvent onComplete
        {
            get { return _onComplete; }
        }

        private void Start()
        {
            if (_playMode == PlayMode.Start)
            {
                Play();
            }
        }

        private void OnEnable()
        {
            if (_playMode == PlayMode.OnEnable)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            Stop();
        }

        /// <summary>
        /// Applies our current tween status, should be called if you manually made
        /// modifications to tween values such as progress.
        /// </summary>
        public void Apply()
        {
            UpdateValue(GetSmoothTime());
        }

        /// <summary>
        /// Will start our tween if we are enabled.
        /// </summary>
        public void Play()
        {
            if (!CanPlay())
            {
                return;
            }

            Apply();
            if (!_isPlaying)
            {
                _isPlaying = true;
                StartCoroutine(RunTween());
            }
        }

        /// <summary>
        /// Will restart a tween and play again, useful for UI events.
        /// </summary>
        public void Replay()
        {
            if (!CanPlay())
            {
                return;
            }

            _progress = 0f;
            _direction = 1;
            Play();
        }

        /// <summary>
        /// Will flip the direction of our tween and start the tween if its not already playing.
        /// </summary>
        public void Toggle()
        {
            if (!CanPlay())
            {
                return;
            }

            if (progress <= 0f)
            {
                _direction = 1;
            }
            else if (progress >= 1f)
            {
                _direction = -1;
            }
            else
            {
                _direction *= -1;
            }

            Play();
        }

        /// <summary>
        /// Stops our tween from playing anymore, doesnt reset progress.
        /// </summary>
        public void Stop()
        {
            _isPlaying = false;
        }

        private IEnumerator RunTween()
        {
            if (_delay > 0f)
            {
                yield return new WaitForSeconds(_delay);
            }

            while (_isPlaying)
            {
                yield return null;

                Tick(Time.deltaTime);
            }

            _isPlaying = false;
        }

        /// <summary>
        /// Mostly used internally for testing but you can manually tick the tween if you wanted to.
        /// Maybe a jump or fast forward button could use this.
        /// </summary>
        /// <param name="deltaTime">The amount of time that our tween should elapse for. Normally Time.deltaTime</param>
        public void Tick(float deltaTime)
        {
            if (deltaTime <= 0f)
            {
                throw new ArgumentOutOfRangeException("deltaTime", "Cant tick the tween by a value <= 0");
            }

            _progress += deltaTime / _duration * _direction;
            if (_progress <= 0f || _progress >= 1f)
            {
                HandleWrapping();
                Apply();

                // we want to call our onComplete AFTER we apply
                _onComplete.Invoke();
            }
            else
            {
                Apply();
            }
        }

        private void HandleWrapping()
        {
            switch (wrapMode)
            {
                case WrapMode.Once:
                    _isPlaying = false;
                    _progress = Mathf.Clamp01(_progress);
                    break;
                case WrapMode.Loop:
                    // we add one incase we are < 0
                    _progress = (_progress + 1f) % 1f;
                    break;
                case WrapMode.PingPong:
                    // ping pong bounces progress and flips direction
                    if (_progress < 0f)
                    {
                        _progress *= -1;
                        _direction = 1;
                    }
                    else if (_progress > 1f)
                    {
                        _progress = 2f - _progress;
                        _direction = -1;
                    }
                    break;
            }
        }

        private bool CanPlay()
        {
            return enabled;
        }

        private float GetSmoothTime()
        {
            switch (_easingMode)
            {
                case EasingMode.Linear:
                    return Easings.Linear(_progress);
                // Quadratic
                case EasingMode.QuadraticIn:
                    return Easings.Quadratic.In(_progress);
                case EasingMode.QuadraticOut:
                    return Easings.Quadratic.Out(_progress);
                case EasingMode.QuadraticInOut:
                    return Easings.Quadratic.InOut(_progress);
                // Cubic
                case EasingMode.CubicIn:
                    return Easings.Cubic.In(_progress);
                case EasingMode.CubicOut:
                    return Easings.Cubic.Out(_progress);
                case EasingMode.CubicInOut:
                    return Easings.Cubic.InOut(_progress);
                // Quartic
                case EasingMode.QuarticIn:
                    return Easings.Quartic.In(_progress);
                case EasingMode.QuarticOut:
                    return Easings.Quartic.Out(_progress);
                case EasingMode.QuarticInOut:
                    return Easings.Quartic.InOut(_progress);
                // Quintic
                case EasingMode.QuinticIn:
                    return Easings.Quintic.In(_progress);
                case EasingMode.QuinticOut:
                    return Easings.Quintic.Out(_progress);
                case EasingMode.QuinticInOut:
                    return Easings.Quintic.InOut(_progress);
                // Bounce
                case EasingMode.BounceIn:
                    return Easings.Bounce.In(_progress);
                case EasingMode.BounceOut:
                    return Easings.Bounce.Out(_progress);
                case EasingMode.BounceInOut:
                    return Easings.Bounce.InOut(_progress);
                // Back
                case EasingMode.BackIn:
                    return Easings.Back.In(_progress);
                case EasingMode.BackOut:
                    return Easings.Back.Out(_progress);
                case EasingMode.BackInOut:
                    return Easings.Back.InOut(_progress);
                // Custom
                case EasingMode.CustomCurve:
                    return _customCurve.Evaluate(_progress);
            }
            throw new NotImplementedException("Easing [" + _easingMode + "] not implemented");
        }

        /// <summary>
        /// Used to record the current value of our target and store it directly into our starting value
        /// </summary>
        public abstract void RecordStart();

        /// <summary>
        /// Used to record the current value of our target and store it directly into our ending value.
        /// </summary>
        public abstract void RecordEnd();

        /// <summary>
        /// Override to implement the tween change. Usually using Mathf.Lerp, Vector.Lerp or similar.
        /// </summary>
        /// <param name="smoothTime">Pre-smoothed progress value</param>
        protected abstract void UpdateValue(float smoothTime);
    }
}
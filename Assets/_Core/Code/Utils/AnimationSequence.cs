using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UltEvents;
using UnityEngine;

namespace SkillGame.Utils {

    public enum AnimationReference { Absolute, Local, Relative }

    public class AnimationSequence : MonoBehaviour {

        [SerializeField] UltEvent onStart, onFinished;
        [SerializeReference, SerializeReferenceDropdown] SequenceOptionData[] options;

        CancellationTokenSource _source;

        private void Awake() {
            foreach (var option in options)
                option.Initialize( this );
        }

        private void OnDisable() => DisposeToken();

        public void Play() => Play( null );
        public void PlayBackwards() => PlayBackwards( null );

        public void Play( Action onFinished ) {
            Stop();
            DoAnimationSequence( options.Select( opt => opt.GetAnimation() ).ToArray(), onFinished ).Forget();
        }

        public void PlayBackwards( Action onFinished ) {
            Stop();
            DoAnimationSequence( options.Select( opt => opt.GetAnimation( true ) ).ToArray(), onFinished ).Forget();
        }

        public void Stop() => DisposeToken();

        async UniTaskVoid DoAnimationSequence( Tween[] tweens, Action onFinished = null ) {
            _source = new();
            try {
                onStart?.Invoke();
                await UniTask.WhenAll( tweens.Select( t => t.ToUniTask( cancellationToken: _source.Token ) ) );
                onFinished?.Invoke();
                this.onFinished?.Invoke();
            } catch { }
            DisposeToken();
        }

        void DisposeToken() {
            _source?.DisposeToken();
            _source = null;
        }

    }

    [Serializable]
    public abstract class SequenceOptionData {

        public float Duration = 1;
        public float Delay;
        public Ease AnimEase = Ease.Linear;
        public bool AbsoluteStartValue;

        protected AnimationSequence mainSequence;

        internal virtual void Initialize( AnimationSequence sequence ) => mainSequence = sequence;
        internal Tween GetAnimation( bool backward = false ) {
            var target = backward ? GetBackwardAnimation() : GetForwardAnimation();
            target.SetEase( AnimEase ).SetDelay( Delay );
            return target;
        }

        protected abstract Tween GetForwardAnimation();
        protected abstract Tween GetBackwardAnimation();

    }

    public abstract class SequenceOptionData<T> : SequenceOptionData where T : Component {

        [SerializeField] protected T target;

        internal override void Initialize( AnimationSequence sequence ) {
            base.Initialize( sequence );
            if (!target)
                if (typeof( Transform ).IsAssignableFrom( typeof( T ) ))
                    target = mainSequence.transform as T;
                else
                    target = mainSequence.GetComponent<T>();
        }

    }

    public abstract class SequenceOptionData<T, K> : SequenceOptionData where T : Component {

        [SerializeField] protected T target;
        [SerializeField] protected K startValue;
        [SerializeField] protected K targetValue;

        internal override void Initialize( AnimationSequence sequence ) {
            base.Initialize( sequence );
            if (!target)
                if (typeof( Transform ).IsAssignableFrom( typeof( T ) ))
                    target = mainSequence.transform as T;
                else
                    target = mainSequence.GetComponent<T>();
        }


        protected override Tween GetForwardAnimation() => GetTween( startValue, targetValue );
        protected override Tween GetBackwardAnimation() => GetTween( targetValue, startValue );

        protected abstract Tween GetTween( K start, K end );

    }


    [Serializable]
    public class TransformMoveOptionData : SequenceOptionData<Transform, Vector3> {

        [SerializeField] AnimationReference reference = AnimationReference.Absolute;

        protected override Tween GetTween( Vector3 start, Vector3 end ) {
            if (AbsoluteStartValue)
                InitializePosition( start );
            return GetActualTween( end );
        }

        void InitializePosition( Vector3 pos ) {
            if (reference == AnimationReference.Local)
                target.localPosition = pos;
            else
                target.position = pos;
        }

        Tween GetActualTween( Vector3 pos ) =>
            reference switch {
                AnimationReference.Absolute => target.DOMove( pos, Duration ),
                AnimationReference.Local => target.DOLocalMove( pos, Duration ),
                AnimationReference.Relative => target.DOMove( target.position + pos, Duration ),
                _ => null
            };

    }
    

    [Serializable]
    public class TransformScaleOptionData : SequenceOptionData<Transform, Vector3> {

        protected override Tween GetTween( Vector3 start, Vector3 end ) {
            if (AbsoluteStartValue)
                target.localScale = start;
            return target.DOScale( end, Duration );
        }
    }

    [Serializable]
    public class TransformShakeOptionData : SequenceOptionData<Transform> {

        public float Strength = 1;
        public int Vibrato = 10;
        public float Randomness = 90;
        public bool FadeOut = true;

        protected override Tween GetForwardAnimation() => GetActualTween();
        protected override Tween GetBackwardAnimation() => GetActualTween();

        Tween GetActualTween() => target.DOShakePosition( Duration, Strength, Vibrato, Randomness, fadeOut: FadeOut );

    }

    [Serializable]
    public class SpriteColorOptionData : SequenceOptionData<SpriteRenderer, Color> {

        protected override Tween GetTween( Color startValue, Color targetValue ) {
            if (AbsoluteStartValue)
                target.color = startValue;
            return target.DOColor( targetValue, Duration );
        }

    }

    [Serializable]
    public class TextColorOptionData : SequenceOptionData<TMP_Text, Color> {

        [SerializeField] AnimationReference reference = AnimationReference.Absolute;

        protected override Tween GetTween( Color start, Color end ) {
            if (AbsoluteStartValue)
                target.color = start;
            if (reference == AnimationReference.Relative)
                return DOTween.To( () => target.color, val => target.color = val, target.color * end, Duration );
            return DOTween.To( () => target.color, val => target.color = val, end, Duration );
        }

    }

    [Serializable]
    public class MeshColorPulseOptionData : SequenceOptionData {

        [SerializeField] MeshRenderer[] targetRenders;
        [SerializeField] Color startColor;
        [SerializeField] Color targetColor;

        Dictionary<MeshRenderer, Color[]> _meshColorMap;

        internal override void Initialize( AnimationSequence mainSequence ) {
            base.Initialize( mainSequence );
            _meshColorMap = new();
            foreach (var render in targetRenders)
                _meshColorMap.Add( render, render.materials.Select( mat => mat.GetColor( "_BaseColor" ) ).ToArray() );
        }

        protected override Tween GetForwardAnimation() => GetTween();
        protected override Tween GetBackwardAnimation() => GetTween();

        Tween GetTween() {
            var sequence = DOTween.Sequence( mainSequence );
            foreach (var render in targetRenders) {
                var originalColors = _meshColorMap[ render ];
                for (int i = 0; i < render.materials.Length; i++) {
                    int index = i;
                    var mat = render.materials[ index ];
                    sequence.Join( mat.DOColor( Color.red, Duration ).OnComplete( () => mat.DOColor( originalColors[ index ], .5f ) ) );
                }
            }
            return sequence;
        }

    }

    [Serializable]
    public class CanvasGroupOptionData : SequenceOptionData<CanvasGroup, float> {

        protected override Tween GetTween( float start, float end ) {
            if (AbsoluteStartValue)
                target.alpha = start;
            return target.DOFade( end, Duration );
        }

    }

}

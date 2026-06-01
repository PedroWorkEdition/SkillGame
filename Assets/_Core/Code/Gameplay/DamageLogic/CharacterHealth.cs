using DG.Tweening;
using SkillGame.Data;
using SkillGame.StateMachineLogic;
using SkillGame.Utils;
using System;
using UltEvents;
using UnityEngine;

namespace SkillGame {

    public class CharacterHealth : Damageble {

        [SerializeField] int defaultHealth;
        [SerializeField] float hurtTime = .5f;
        [SerializeField] float knockBackForce = 10;
        [SerializeField] float invincibilityTime = .75f;
        [SerializeField] Rigidbody2D rb;
        [SerializeField] UltEvent onDeath;
        [SerializeField] UltEvent<HealthChangeData> onHealthChanged;
        [SerializeField] UltEvent<HealthDeltaContext> onDamageTaken;
        [SerializeField] UltEvent<HealthDeltaContext> onHealed;
        [SerializeField] string hurtAnimState;
        [SerializeField] string deadAnimState;

        internal event Action<Character> OnDeath;
        internal bool IsDead => HP == 0;

        int _maxHP;
        int _currentHP;
        Tweener _kbTween;
        bool _invincibility = false;
        bool _trigger = false;

        public int HP {
            get => _currentHP;
            set {
                var prev = _currentHP;
                _currentHP = Mathf.Clamp( value, 0, _maxHP );
                onHealthChanged?.Invoke( new( _currentHP, _maxHP, prev - _currentHP ) );
                if (_currentHP > 0) return;
                _kbTween?.Kill();
                onDeath?.Invoke();
                OnDeath?.Invoke( character );
            }
        }

        private void Awake() {
            if (character || defaultHealth == 0) return;
            ResetHP( defaultHealth );
        }

        private void OnDestroy() => _kbTween?.Kill();

        internal void Initialize( Character character ) {
            this.character = character;
            rb = character.rb;
            ResetHP( base.character.Data ? base.character.Stats.HP : defaultHealth );
            var hurtStateCtx = new StateContext( Character.STATE_HURT, typeof( TimedState ), OnEnterHurtState, OnExitHurtState, hurtTime );
            var deadStateCtx = new StateContext( Character.STATE_DEATH, OnEnterDeathState );
            hurtStateCtx = hurtStateCtx.AddLink( base.character.GetIdleState(), () => true );
            var hurtState = base.character.RegisterState( hurtStateCtx );
            var deadState = base.character.RegisterState( deadStateCtx );
            base.character.RegisterGlobalLink( hurtState, () => _trigger && HP > 0 );
            base.character.RegisterGlobalLink( deadState, () => HP == 0 );
        }

        void OnEnterHurtState() {
            _trigger = false;
            character.animator.Play( hurtAnimState );
        }

        void OnExitHurtState() {
            _kbTween?.Kill();
            character.rb.linearVelocity = Vector3.zero;
        }

        void OnEnterDeathState() {
            character.rb.linearVelocity = Vector3.zero;
            character.animator.Play( deadAnimState );
        }

        public override void TakeDamage( DamageContext ctx ) {
            if (_invincibility || IsDead) return;
            _trigger = true;
            HP -= Mathf.Abs( ctx.DamageDealt );
            var targetTransform = character ? character.model.transform : transform;
            var knockbackMultiplier = ctx.Data.KnockBackStrength * knockBackForce;
            if (rb && ctx.Data.UseKnockBack && knockbackMultiplier > 0 && character) {
                var knockback = character.Cam.transform.right * ( ( targetTransform.position - ctx.Source.position ).normalized.x > 0 ? 1 : -1 );
                _kbTween?.Kill( true );
                UnitaskUtils.WaitForSeconds( .05f, () => ApplyKnockback( knockback * knockbackMultiplier ) ).Forget();
            }
            if (invincibilityTime > 0) {
                _invincibility = true;
                UnitaskUtils.WaitForSeconds( invincibilityTime, () => _invincibility = false ).Forget();
            }
            onDamageTaken?.Invoke( new( ctx.DamageDealt, transform ) );
            base.TakeDamage( ctx );
        }

        void ApplyKnockback( Vector3 knockback ) {
            var camRight = character.Cam.transform.right;
            rb.linearVelocity = new Vector3( knockback.x, 0, knockback.z );
            _kbTween = DOTween.To( () => rb.linearVelocity, val => rb.linearVelocity = val, Vector2.zero, hurtTime );
        }

        public void TakeDamage( int dmg ) {
            HP -= Mathf.Abs( dmg );
            onDamageTaken?.Invoke( new( dmg, transform ) );
        }

        public void Heal( int val ) {
            HP += Mathf.Abs( val );
            onHealed?.Invoke( new ( val, transform ) );
        }

        void ResetHP( int hp ) => HP = _maxHP = hp;

    }

}

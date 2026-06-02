using HaroLibs;
using SkillGame.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SkillGame {

    public partial class Character : MonoBehaviour {

        [Header( "Optional Data" )]
        [SerializeField] CharacterData data;
        [SerializeField] ValueContainerBase<CharacterStatusData> stats;
        [SerializeField] CharacterHealth health;

        [Header( "Obligatory Components" )]
        [SerializeField] internal Transform model;
        [SerializeField] internal Rigidbody2D rb;
        [SerializeField] internal Animator animator;
        [SerializeField] private CharacterBehaviourBase[] generalBehaviours;

        [Header( "Input Buffer" )]
        [SerializeField] float inputBufferAliveTime = .2f;
        [SerializeField] InputBufferCollection inputBufferOverride;

        [Header( "State Machine Data" )]
        [SerializeField] StateBehaviourLink[] links;
        [SerializeField] string idleStateAnimState;
        [SerializeField] string lockedStateAnimState;
        [Header( "Debug" )]
        [SerializeField] string currentState;

        Dictionary<Type, CharacterBehaviourBase> _behaviourMap;
        Vector2 _direction;
        internal Vector2 Direction {
            get => _direction;
            private set {
                _direction = value;
                if (_direction.magnitude > 0) LastValidDirection = _direction;
                _directionBuffer.SetCallback( () => DirectionTriggered?.Invoke( _directionBuffer, _direction ), GetAliveTime( inputBufferOverride.Movement ) );
            }
        }

        internal Vector2 LastValidDirection { get; private set; } = Vector2.right;
        public CharacterStatusData Stats => ( data ? data.Data : CharacterStatusData.Default() ) + 
                                            ( stats ? stats.Value : CharacterStatusData.Default( maxCombo: 0 ) );
        public CharacterData Data => data;
        public WeaponData Weapon { get; internal set; }

        internal Camera Cam { get; private set; }
        internal CharacterHealth Health => health;

        internal event Action<Vector2> DirectionForced;
        internal event Action<InputBuffer, Vector2> DirectionTriggered;
        internal event Action<InputBuffer> AttackTriggered;
        internal event Action<InputBuffer> SubAttackTriggered;
        internal event Action<InputBuffer> ActionTriggered;
        internal event Action<InputBuffer> SubActionTriggered;
        
        InputBuffer _directionBuffer, _inputBuffer, _emptyBuffer;

        private void Awake() {
            Cam = Camera.main;
            _directionBuffer = new( inputBufferAliveTime );
            _inputBuffer = new( inputBufferAliveTime );
            _emptyBuffer = new( 0 );
            InitializeStateMachine();
            health.Initialize( this );
        }

        private void Update() { 
            _stateMachine.Update(); 
            _directionBuffer.Update();
            _inputBuffer.Update();
        }

        public T GetBehaviour<T>() where T : CharacterBehaviourBase => _behaviourMap.TryGetValue( typeof( T ), out var behaviour ) ? ( T )behaviour : default;
        public bool TryGetBehaviour<T>( out T behaviour ) where T : CharacterBehaviourBase => behaviour = GetBehaviour<T>();

        public void RefreshDirection() => _emptyBuffer.SetCallback( () => DirectionTriggered?.Invoke( _emptyBuffer, Direction ) );
        public void ForceDirection() => DirectionForced?.Invoke( LastValidDirection );

        public void SetDirection( Vector2 dir ) => Direction = dir;
        public void TriggerAttack() => 
            _inputBuffer.SetCallback( () => AttackTriggered?.Invoke( _inputBuffer ), GetAliveTime( inputBufferOverride.Attack ) );
        public void TriggerSubAttack() => 
            _inputBuffer.SetCallback( () => SubAttackTriggered?.Invoke( _inputBuffer ), GetAliveTime( inputBufferOverride.SubAttack ) );
        public void TriggerAction() => 
            _inputBuffer.SetCallback( () => ActionTriggered?.Invoke( _inputBuffer ), GetAliveTime( inputBufferOverride.Action ) );
        public void TriggerSubAction() => 
            _inputBuffer.SetCallback( () => SubActionTriggered?.Invoke( _inputBuffer ), GetAliveTime( inputBufferOverride.SubAction ) );

        float GetAliveTime( InputBufferData data ) => data.Override ? data.AliveTime : inputBufferAliveTime;

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        public void SetMovementOverrideCheck( bool val ) => inputBufferOverride.Movement.Override = val;
        public void SetAttackOverrideCheck( bool val ) => inputBufferOverride.Attack.Override = val;
        public void SetDashOverrideCheck( bool val ) => inputBufferOverride.SubAction.Override = val;
        public void SetMoveOverrideValue( float val ) => inputBufferOverride.Movement.AliveTime = val;
        public void SetAttackOverrideValue( float val ) => inputBufferOverride.Attack.AliveTime = val;
        public void SetDashOverrideValue( float val ) => inputBufferOverride.SubAction.AliveTime = val;
#endif

        [Serializable]
        internal struct InputBufferCollection {

            public InputBufferData Movement;
            public InputBufferData Attack;
            public InputBufferData SubAttack;
            public InputBufferData Action;
            public InputBufferData SubAction;

        }

        [Serializable]
        internal struct InputBufferData {

            public bool Override;
            public float AliveTime;

        }

    }

}

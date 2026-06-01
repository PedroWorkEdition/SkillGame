using SkillGame.StateMachineLogic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SkillGame {

    public partial class Character {

        internal const string STATE_IDLE = "Idle";
        internal const string STATE_LOCKED = "Locked";
        internal const string STATE_MOVE = "Moving";
        internal const string STATE_DASH = "Dash";
        internal const string STATE_ATTACK = "Attack";
        internal const string STATE_HURT = "Hurt";
        internal const string STATE_DEATH = "Death";
        internal const string STATE_POWER = "PowerUp";

        StateMachine _stateMachine;
        State _idleState;
        State _lockState;
        Dictionary<string, (State state, StateBehaviourLink link)> _registeredStates;
        bool _locked = false;

        void InitializeStateMachine() {
            _registeredStates = new();
            var idleStateCtx = new StateContext( STATE_IDLE, OnEnterIdleState );
            var lockedStateCtx = new StateContext( STATE_LOCKED, OnEnterLockedState );
            _idleState = RegisterState( idleStateCtx );
            _lockState = RegisterState( lockedStateCtx );
            _lockState.AddLink( _lockState, () => _locked );
            _lockState.AddLink( _lockState, () => _locked );
            _lockState.AddLink( _idleState, () => !_locked );
            _stateMachine = new( _idleState, state => currentState = state.Name );
            RegisterGlobalLink( _lockState, () => _locked );
            foreach (var behaiour in behaviours) behaiour.Initialize( this );
            foreach (var behaiour in behaviours) behaiour.PostInitilize();
        }

        void OnEnterIdleState() {
            animator.Play( STATE_IDLE );
            RefreshDirection();
        }

        void OnEnterLockedState() {
            animator.Play( STATE_IDLE );
            rb.linearVelocity *= 0;
        }

        internal void AddLinkToIdleState( Link link ) => _idleState.AddLink( link );
        internal void AddLinkToIdleState( State target, Func<bool> condition ) => _idleState.AddLink( target, condition );

        internal State GetIdleState() => _idleState;

        internal State GetState( string name ) => _registeredStates.TryGetValue( name, out var state ) ? state.state : null;

        internal void AddLinkToRegisteredState( string stateName, Link link ) {
            if (!_registeredStates.ContainsKey( stateName )) return;
            _registeredStates[ stateName ].state.AddLink( link );
        }

        internal State RegisterState( StateContext ctx ) {
            if (_registeredStates.TryGetValue( ctx.Name, out var registeredState )) return registeredState.state;
            var link = links.FirstOrDefault( l => l.Name == ctx.Name );
            if (link.Name != null) RegisterBehaviourLink( ref ctx );
            var state = ctx.CreateState();
            _registeredStates.Add( state.Name, (state, link) );
            return state;
        }

        internal void RegisterGlobalLink( Link link ) => _stateMachine.RegisterLink( link );
        internal void RegisterGlobalLink( State target, Func<bool> condition ) => _stateMachine.RegisterLink( target, condition );

        void RegisterBehaviourLink( ref StateContext ctx ) {
            string stateName = ctx.Name;
            ctx.RegisterOnEnterCallback( () => { foreach (var behaviour in _registeredStates[ stateName ].link.AvailableBehaviours) behaviour.Active = true; } );
            ctx.RegisterOnExitCallback( () => { foreach (var behaviour in _registeredStates[ stateName ].link.AvailableBehaviours) behaviour.Active = false; } );
        }

        public void LockPlayer( bool locked ) => _locked = locked;

        [Serializable]
        internal struct StateBehaviourLink {

            public string Name;
            public CharacterBehaviourBase[] AvailableBehaviours;

        }

    }

}

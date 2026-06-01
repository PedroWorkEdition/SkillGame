using System;
using System.Collections.Generic;

namespace SkillGame.StateMachineLogic {

    public class StateMachine {

        State _current;
        readonly List<Link> _globalLinks;
        readonly Action<State> _onStateChange;

        public StateMachine( State startState, Action<State> onStateChanged = null ) {
            _globalLinks = new();
            _onStateChange = onStateChanged;
            ChangeState( startState );
        }

        public void RegisterLink( Link state ) => _globalLinks.Add( state );
        public void RegisterLink( State target, Func<bool> condition ) => _globalLinks.Add( Link.Create( target, condition ) );

        public void Update() {
            var nextState = GlobalsUpdate() ?? _current.Update();
            if (nextState == null) return;
            ChangeState( nextState );
        }

        void ChangeState( State newState ) {
            _current?.OnExit();
            _current = newState;
            _current.OnEnter();
            _onStateChange?.Invoke( newState );
        }

        State GlobalsUpdate() {
            foreach (var link in _globalLinks) 
                if (link.Validate( out var state ))
                    return state;
            return null;
        }

    }

}

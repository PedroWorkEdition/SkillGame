using System;
using UnityEngine;

namespace SkillGame.StateMachineLogic {
    public class Link {

        readonly Func<bool> _condition;
        readonly State _targetState;

        private Link() { }
        private Link( State taregtState, Func<bool> condition ) => (_targetState, _condition) = (taregtState, condition);

        public bool Validate( out State state ) {
            state = null;
            var result = _condition();
            if (result) state = _targetState;
            return result;
        }

        internal static Link Create( State targetState, Func<bool> condition ) {
            if (condition == null || targetState == null) {
                Debug.LogError( $"Can't create link, the target state or condition is null -> state is null? {targetState == null}, condition is null? {condition == null}" );
                return null;
            }
            return new( targetState, condition );
        }

    }

}

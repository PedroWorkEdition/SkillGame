using System;
using System.Collections.Generic;

namespace SkillGame.StateMachineLogic {

    public class State {

        public readonly string Name;
        readonly List<Link> _links;
        readonly Action _onEnter, _onExit;

        protected State() { }

        public State( StateContext ctx ) {
            Name = ctx.Name;
            _links = ctx.Links;
            _onEnter = ctx.OnEnter;
            _onEnter += ctx.onEnter;
            _onExit = ctx.OnExit;
            _onExit += ctx.onExit;
        }

        public State AddLink( params Link[] links ) {
            foreach (var link in links) 
                _links.Add( link );
            return this;
        }

        public State AddLink( State target, Func<bool> condition ) { 
            _links.Add( Link.Create( target, condition ) ); 
            return this;
        }

        public virtual void OnEnter() => _onEnter?.Invoke();

        public virtual State Update() {
            foreach (var link in _links) {
                if (link.Validate( out var state ))
                    return state;
            }
            return null;
        }

        public virtual void OnExit() => _onExit?.Invoke();

    }

}

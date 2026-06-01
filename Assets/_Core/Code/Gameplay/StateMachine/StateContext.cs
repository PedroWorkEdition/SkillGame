using System;
using System.Collections.Generic;

namespace SkillGame.StateMachineLogic {

    public struct StateContext {

        public string Name;
        public readonly List<Link> Links;
        public Action OnEnter, OnExit;
        public readonly Type StateType;
        public readonly object[] Arguments;

        internal readonly Action onEnter, onExit;

        public StateContext( string name, Type type, Action onEnter = null, Action onExit = null, params object[] arguments ) { 
            if (!typeof( State ).IsAssignableFrom( type )) 
                throw new ArgumentException( $"The Current Type '{type}' is not supported" );
            Name = name; 
            Links = new List<Link>();
            OnEnter = null;
            OnExit = null;
            this.onEnter = onEnter;
            this.onExit = onExit;
            Arguments = arguments;
            StateType = type;
        }

        public StateContext( string name, Action onEnter = null, Action onExit = null ) {
            Name = name;
            Links = new List<Link>();
            OnEnter = null;
            OnExit = null;
            this.onEnter = onEnter;
            this.onExit = onExit;
            Arguments = null;
            StateType = typeof( State );
        }

        public readonly StateContext AddLink( params Link[] links ) {
            foreach (var link in links)
                Links.Add( link );
            return this;
        }

        public readonly StateContext AddLink( State target, Func<bool> condition ) { 
            Links.Add( Link.Create( target, condition ) );
            return this;
        }

        public void RegisterOnEnterCallback( Action callback ) => OnEnter += callback;
        public void RegisterOnExitCallback( Action callback ) => OnExit += callback;

        public readonly State CreateState() { 
            var args = new List<object>( Arguments ?? Array.Empty<object>() );
            args.Insert( 0, this );
            return Activator.CreateInstance( StateType, args.ToArray() ) as State;
        }
        
    }

}

using System;

namespace HaroLibsEditor {
    public class AnomAction<T> : IAnomActionProvider {
        Action<T> _action;
        public AnomAction( Action<T> action ) => _action = action;
        public void SetAction( Action<T> action ) => _action = action;
        public void Call( object arg ) => _action?.Invoke( (T)arg );
        public static implicit operator Action<T>( AnomAction<T> action ) => action._action;
    }
}

using SkillGame.Utils;
using System;
using System.Threading;

namespace SkillGame {

    public class InputBuffer {

        internal bool Consumed { get; private set; } = true;

        readonly float _aliveTime;
        Action _callback;
        CancellationTokenSource _source;
        string _debugName;

        internal InputBuffer( float aliveTime ) => _aliveTime = aliveTime;

        internal void SetCallback( Action callback, float? aliveTime = null, string debugName = null ) {
            if (_debugName != null)
                UnityEngine.Debug.Log( $"Buffer for {_debugName} has been eaten" );
            _debugName = debugName;
            _source?.DisposeToken();
            _source = null;
            var currentAliveTime = aliveTime ?? _aliveTime;
            Consumed = currentAliveTime == 0;
            (_callback = callback)();
            if (_debugName != null) 
                UnityEngine.Debug.Log( $"Buffer for {_debugName}: is Consumed? {Consumed}" );
            if (currentAliveTime < 0 || Consumed) return;
            _source = new CancellationTokenSource();
            UnitaskUtils.WaitForSeconds( currentAliveTime, Consume, _source.Token ).Forget();
        }

        internal void Update() {
            if (Consumed) return;
            _callback();
        }

        internal void Consume() {
            if (_debugName != null)
                UnityEngine.Debug.Log( $"Buffer for {_debugName} has been consumed" );
            Consumed = true;
            _source?.DisposeToken();
            _source = null;
            _callback = null;
        }

    }

}

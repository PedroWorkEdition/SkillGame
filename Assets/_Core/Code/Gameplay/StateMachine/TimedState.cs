using SkillGame.Utils;
using System;
using System.Threading;

namespace SkillGame.StateMachineLogic {

    internal class TimedState : State {

        readonly float _sleepTime;
        readonly Func<float> _sleepTimeCallback;
        bool _triggered;
        CancellationTokenSource _source;

        float SleepTime => _sleepTimeCallback?.Invoke() ?? _sleepTime;

        public TimedState( StateContext ctx, float time ) : base( ctx ) => _sleepTime = time;
        public TimedState( StateContext ctx, Func<float> time ) : base( ctx ) => _sleepTimeCallback = time;

        public override void OnEnter() {
            _triggered = false;
            base.OnEnter();
            _source = new();
            UnitaskUtils.WaitForSeconds( SleepTime, () => _triggered = true, _source.Token ).Forget();
        }

        public override void OnExit() {
            base.OnExit();
            _source.Cancel();
        }

        public override State Update() {
            if (!_triggered) return null;
            return base.Update();
        }

    }

}

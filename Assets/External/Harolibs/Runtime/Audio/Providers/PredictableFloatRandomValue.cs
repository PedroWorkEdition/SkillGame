namespace HaroLibs {
    public class PredictableFloatRandomValue : FloatRandomValueProvider {

        public override float Get( params object[] args ) {
            char target = default;
            foreach (var arg in args) {
                if ( arg is char c )
                    target = c;
            }
            if (target == default) return base.Get( min, max, args );
            float endValue = min;
            var hashCode = target.GetHashCode();
            var minPitchInt = ( int )( min * 100 );
            var maxPitchInt = ( int )( max * 100 );
            var pitchRange = maxPitchInt - minPitchInt;
            if (pitchRange != 0)
            {
                var predictablePitchInt = ( hashCode % pitchRange ) + minPitchInt;
                endValue = predictablePitchInt / 100f;
            }
            return endValue;
        }

    }

}

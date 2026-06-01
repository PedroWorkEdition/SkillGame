namespace SkillGame.Data {
    public readonly struct HealthChangeData {

        public readonly int Current;
        public readonly int Max;
        public readonly int Delta;

        public HealthChangeData( int current, int max, int delta ) => (Current, Max, Delta) = (current, max, delta);

    }

}

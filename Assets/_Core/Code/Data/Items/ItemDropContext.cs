namespace SkillGame.Data {
    public readonly struct ItemDropContext {

        readonly public ItemData Data;
        readonly public int Count;

        public ItemDropContext( ItemData data, int count ) => (Data, Count) = (data, count);

    }

}

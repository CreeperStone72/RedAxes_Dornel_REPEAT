namespace DataTypes {
    public class Pair<TF, TS> {
        public readonly TF first;
        public readonly TS second;

        public Pair(TF first, TS second) {
            this.first = first;
            this.second = second;
        }

        public override string ToString() { return $"({first}, {second})"; }
    }
}
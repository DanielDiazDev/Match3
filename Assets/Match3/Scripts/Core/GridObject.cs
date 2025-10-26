namespace Core
{
    public class GridObject<T>
    {
        private GridSystem<GridObject<T>> _gridSystem;
        public int x { get; private set; }
        public int y { get; private set; }
        private T _gem;

        public GridObject(GridSystem<GridObject<T>> grid, int x, int y)
        {
            this._gridSystem = grid;
            this.x = x;
            this.y = y;
        }

        public void SetValue(T gem)
        {
            _gem = gem;
        }

        public T GetValue() => _gem;
    }
}
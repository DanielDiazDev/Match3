namespace Core
{
    public class GridObject<T>
    {
        private GridSystem<GridObject<T>> _gridSystem;
        private int x;
        private int y;
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
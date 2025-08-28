namespace Match3.Randomizers
{
    public class SystemRandomCellsRandomizer : ICellsRandomizer
    {
        private readonly System.Random _random;

        public SystemRandomCellsRandomizer() 
        {
            _random = new System.Random();
        }

        public CellType GetNext()
        {
            return (CellType)_random.Next((int)CellType.Type1, (int)CellType.Type4 + 1);
        }
    }
}

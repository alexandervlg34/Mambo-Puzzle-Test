namespace Match3
{
    public class Cell
    {
        public CellType Type { get; set; }
        public bool IsEmpty => Type == CellType.Empty;
        public void SetEmpty() => Type = CellType.Empty;

        public Cell(CellType type)
        {
            Type = type;
        }
    }

    public enum CellType
    {
        Empty = -1,
        Type1 = 0,
        Type2 = 1,
        Type3 = 2,
        Type4 = 3
    }
}

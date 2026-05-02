namespace Crysc.Presentation.Arrangements
{
    public interface IArrangementCalculator
    {
        public ElementPlacement[] CalculateElementPlacements(ComplexArrangement arrangement);
    }
}

using System;

namespace Crysc.Presentation.Arrangements
{
    public class ArrangementEventArgs : EventArgs
    {
        public ElementMovementPlan Plan { get; }
        public ArrangementEventArgs(ElementMovementPlan plan) { Plan = plan; }
    }
}

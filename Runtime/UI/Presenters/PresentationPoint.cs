using System;
using UnityEngine;

namespace Crysc.UI.Presenters
{
    [Serializable]
    public struct PresentationPoint
    {
        public enum PointType
        {
            Local,
            World,
        }

        public Vector3 Position;
        public PointType Type;
    }

    public static class PresentationPointExtensions
    {
        public static Vector3 LocalPosition(this PresentationPoint point, Transform parent)
        {
            return point.Type switch
            {
                PresentationPoint.PointType.Local => point.Position,
                PresentationPoint.PointType.World => parent.InverseTransformPoint(point.Position),
                _                                 => throw new ArgumentOutOfRangeException(point.Type.ToString()),
            };
        }
    }
}

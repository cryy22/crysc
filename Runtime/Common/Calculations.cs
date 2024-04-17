using UnityEngine;

namespace Crysc.Common
{
    public static class Calculations
    {
        public static float CalculateAirtime(float height, float gravity = 9.81f)
        {
            return Mathf.Sqrt(height * 8 / gravity);
        }
    }
}

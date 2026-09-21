using Assets;
using UnityEngine;

namespace Assets.GlobalFuncs
{
    public class Functions
    {
        public float Round(float value, int precision)
        {
            int decPow = (int)Mathf.Pow(10, precision);

            value = Mathf.Round(decPow * value) / decPow;

            return value;
        }


        public Vector3 Round(Vector3 value, int precision)
        {
            int decPow = (int)Mathf.Pow(10, precision);

            value.x = Mathf.Round(decPow * value.x) / decPow;
            value.y = Mathf.Round(decPow * value.y) / decPow;
            value.z = Mathf.Round(decPow * value.z) / decPow;

            return value;
        }
        public Vector2 Round(Vector2 value, int precision)
        {
            int decPow = (int)Mathf.Pow(10, precision);

            value.x = Mathf.Round(decPow * value.x) / decPow;
            value.y = Mathf.Round(decPow * value.y) / decPow;

            return value;
        }
    }
}

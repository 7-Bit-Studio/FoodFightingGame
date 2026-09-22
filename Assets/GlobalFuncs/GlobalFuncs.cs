using System;
using UnityEngine;

namespace Assets.GlobalFuncs
{
    public readonly struct Functions
    {
        /// <summary>
        /// Rounds float to the number of decimal places provided
        /// </summary>
        /// <param name="value"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public float Round(float value, int precision)
        {
            float decPow = Mathf.Pow(10, precision);

            value = Mathf.Round(decPow * value) / decPow;

            return value;
        }

        /// <summary>
        /// Rounds Vector3 to the number of decimal places provided
        /// </summary>
        /// <param name="value"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public Vector3 Round(Vector3 value, int precision)
        {
            float decPow = Mathf.Pow(10, precision);

            value.x = Mathf.Round(decPow * value.x) / decPow;
            value.y = Mathf.Round(decPow * value.y) / decPow;
            value.z = Mathf.Round(decPow * value.z) / decPow;

            return value;
        }

        /// <summary>
        /// Rounds Vector2 to the number of decimal places provided
        /// </summary>
        /// <param name="value"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public Vector2 Round(Vector2 value, int precision)
        {
            float decPow = Mathf.Pow(10, precision);

            value.x = Mathf.Round(decPow * value.x) / decPow;
            value.y = Mathf.Round(decPow * value.y) / decPow;

            return value;
        }
        public float AverageOverFrames(float[] data)
        {
            float average = 0f;

            foreach (float item in data)
            {
                average += item;
            }

            average /= data.Length;

            return average;
        }
        public Vector3 AverageOverFrames(Vector3[] data)
        {
            float[] x = new float[data.Length];
            float[] y = new float[data.Length];
            float[] z = new float[data.Length];

            for(int i = 0; i < data.Length; i++)
            {
                x[i] = data[i].x;
                y[i] = data[i].y;
                z[i] = data[i].z;
            }

            return new Vector3(AverageOverFrames(x), AverageOverFrames(y), AverageOverFrames(z));
        }
    }
}

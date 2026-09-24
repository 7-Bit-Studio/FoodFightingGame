using System;
using UnityEngine;

namespace Assets.Globals
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
        public double Round(double value, int precision)
        {
            double decPow = Mathf.Pow(10, precision);

            value = Mathf.Round((float)(decPow * value)) / decPow;

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
        public float AverageOverFrames(float[] data, int length)
        {
            float average = 0f;

            foreach (float item in data)
            {
                average += item;
            }

            average /= length;

            return average;
        }
        public Vector3 AverageOverFrames(Vector3[] data, int length)
        {
            float[] x = new float[length];
            float[] y = new float[length];
            float[] z = new float[length];

            for(int i = 0; i < length; i++)
            {
                x[i] = data[i].x;
                y[i] = data[i].y;
                z[i] = data[i].z;
            }

            return new Vector3(AverageOverFrames(x, length), AverageOverFrames(y, length), AverageOverFrames(z, length));
        }
        
        public static Sprite ConvertToSprite(Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
    }
    public struct VarTypes
    {
        public enum Direction
        {
            Left,
            Right,
            Up,
            Down
        }
    }
}

using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        /// <summary>
        /// Rounds double to the number of decimal places provided
        /// </summary>
        /// <param name="value"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public double Round(double value, int precision)
        {
            double decPow = Mathf.Pow(10, precision);

            value = Math.Round(decPow * value) / decPow;

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

        /// <summary>
        /// smoothes out bumpy data, float
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
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

        /// <summary>
        /// smoothes out bumpy data, Vector3
        /// </summary>
        /// <param name="data"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public Vector3 AverageOverFrames(Vector3[] data, int length)
        {
            float[] x = new float[length];
            float[] y = new float[length];
            float[] z = new float[length];

            for (int i = 0; i < length; i++)
            {
                x[i] = data[i].x;
                y[i] = data[i].y;
                z[i] = data[i].z;
            }

            return new Vector3(AverageOverFrames(x, length), AverageOverFrames(y, length), AverageOverFrames(z, length));
        }

        /// <summary>
        /// gets sibling GameObject from name
        /// </summary>
        /// <param name="SiblingName"></param>
        /// <returns></returns>
        public static GameObject GetSiblingGameObject(string SiblingName)
        {
            Scene scene;
            GameObject[] gameObjects;

            GameObject returnObject = new();

            scene = SceneManager.GetActiveScene();
            gameObjects = scene.GetRootGameObjects();

            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject.name == SiblingName)
                {
                    returnObject = gameObject;
                }
            }

            if (returnObject.name != SiblingName)
            {
                Debug.LogError($"No sibling of name {SiblingName} was found");
            }

            return returnObject;
        }

        /// <summary>
        /// i totally stole this code lol
        /// couldn't tell ya how tf this works, but it does
        /// </summary>
        /// <param name="yaw"></param>
        /// <param name="pitch"></param>
        /// <param name="roll"></param>
        /// <returns></returns>
        public static Quaternion ToQuaternion(float yaw, float pitch, float roll)
        {
            float cy = Mathf.Cos(yaw * 0.5f);
            float sy = Mathf.Sin(yaw * 0.5f);
            float cp = Mathf.Cos(pitch * 0.5f);
            float sp = Mathf.Sin(pitch * 0.5f);
            float cr = Mathf.Cos(roll * 0.5f);
            float sr = Mathf.Sin(roll * 0.5f);

            Quaternion q = new()
            {
                w = cr * cp * cy + sr * sp * sy,
                x = sr * cp * cy - cr * sp * sy,
                y = cr * sp * cy + sr * cp * sy,
                z = cr * cp * sy - sr * sp * cy
            };

            return q;
        }

        /// <summary>
        /// couldn't tell ya how tf this works, but it does
        /// </summary>
        /// <param name="vector3"></param>
        /// <returns></returns>
        public static Quaternion ToQuaternion(Vector3 vector3)
        {
            return ToQuaternion(vector3.x, vector3.y, vector3.z);
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
        public struct Data
        {
            public Transform position;
            public Vector3 velocity;
            public Direction direction;
            public int health;
        }
    }
}

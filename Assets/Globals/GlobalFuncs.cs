using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Globals
{
    /// <summary>
    /// Static versions (the real implementations).
    /// Usage: Functions.Round(x, 2)
    /// </summary>
    public readonly struct Functions
    {
        public static Vector3 Friction(Vector3 vel, float fricCoef)
        {
            return vel * fricCoef;
        }

        public static Sprite[] SliceGrid(Texture2D tex, int cellW, int cellH, float ppu = 100f)
        {
            int cols = tex.width / cellW;
            int rows = tex.height / cellH;
            Sprite[] result = new Sprite[cols * rows];
            int i = 0;
            for (int y = rows - 1; y >= 0; y--)
            {
                for (int x = 0; x < cols; x++)
                {
                    result[i++] = Sprite.Create(tex, new Rect(x * cellW, y * cellH, cellW, cellH), new Vector2(0.5f, 0.5f), ppu);
                }
            }
            return result;
        }

        public static float DotProduct(Vector3 a, Vector3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static float DotProduct(Vector2 a, Vector2 b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public static Vector3 CrossProduct(Vector3 a, Vector3 b)
        {
            return new Vector3(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
        }

        public static float AngleBetween(Vector3 a, Vector3 b)
        {
            float dot = DotProduct(a, b);
            float magA = a.magnitude;
            float magB = b.magnitude;
            // clamp so float error can't push acos out of [-1, 1] and return NaN
            float cos = Mathf.Clamp(dot / (magA * magB), -1f, 1f);
            return Mathf.Acos(cos) * Mathf.Rad2Deg;
        }

        public static Vector3 RotationVector(Vector3 from, Vector3 to)
        {
            float angle = AngleBetween(from, to);
            Vector3 axis = CrossProduct(from, to).normalized;
            return axis * angle;
        }

        /// <summary>Rounds float to the number of decimal places provided</summary>
        public static float Round(float value, int precision)
        {
            float decPow = Mathf.Pow(10, precision);
            return Mathf.Round(decPow * value) / decPow;
        }

        /// <summary>Rounds double to the number of decimal places provided</summary>
        public static double Round(double value, int precision)
        {
            double decPow = Math.Pow(10, precision);
            return Math.Round(decPow * value) / decPow;
        }

        /// <summary>Rounds Vector3 to the number of decimal places provided</summary>
        public static Vector3 Round(Vector3 value, int precision)
        {
            float decPow = Mathf.Pow(10, precision);
            value.x = Mathf.Round(decPow * value.x) / decPow;
            value.y = Mathf.Round(decPow * value.y) / decPow;
            value.z = Mathf.Round(decPow * value.z) / decPow;
            return value;
        }

        /// <summary>Rounds Vector2 to the number of decimal places provided</summary>
        public static Vector2 Round(Vector2 value, int precision)
        {
            float decPow = Mathf.Pow(10, precision);
            value.x = Mathf.Round(decPow * value.x) / decPow;
            value.y = Mathf.Round(decPow * value.y) / decPow;
            return value;
        }

        /// <summary>smoothes out bumpy data, float</summary>
        public static float AverageOverFrames(float[] data, int length)
        {
            float average = 0f;
            for (int i = 0; i < length; i++)
            {
                average += data[i];
            }
            return average / length;
        }

        /// <summary>smoothes out bumpy data, Vector3</summary>
        public static Vector3 AverageOverFrames(Vector3[] data, int length)
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

        /// <summary>gets root GameObject in the active scene from name</summary>
        public static GameObject GetSiblingGameObject(string SiblingName)
        {
            foreach (GameObject go in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (go.name == SiblingName)
                {
                    return go;
                }
            }

            Debug.LogError($"No sibling of name {SiblingName} was found");
            return null;
        }

        /// <summary>
        /// i totally stole this code lol
        /// couldn't tell ya how tf this works, but it does
        /// </summary>
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

        public static Quaternion ToQuaternion(Vector3 vector3)
        {
            return ToQuaternion(vector3.x, vector3.y, vector3.z);
        }

        public static void Kill(GameObject gameobject)
        {
            UnityEngine.Object.Destroy(gameobject);
        }
    }

    /// <summary>
    /// Instance versions, same names as the static ones, each just calls through.
    /// Usage: var f = new FunctionsInstance(); f.Round(x, 2)
    /// </summary>
    public readonly struct FunctionsInstance
    {
        public Vector3 Friction(Vector3 vel, float fricCoef) => Functions.Friction(vel, fricCoef);

        public Sprite[] SliceGrid(Texture2D tex, int cellW, int cellH, float ppu = 100f) => Functions.SliceGrid(tex, cellW, cellH, ppu);

        public float DotProduct(Vector3 a, Vector3 b) => Functions.DotProduct(a, b);
        public float DotProduct(Vector2 a, Vector2 b) => Functions.DotProduct(a, b);

        public Vector3 CrossProduct(Vector3 a, Vector3 b) => Functions.CrossProduct(a, b);

        public float AngleBetween(Vector3 a, Vector3 b) => Functions.AngleBetween(a, b);

        public Vector3 RotationVector(Vector3 from, Vector3 to) => Functions.RotationVector(from, to);

        public float Round(float value, int precision) => Functions.Round(value, precision);
        public double Round(double value, int precision) => Functions.Round(value, precision);
        public Vector3 Round(Vector3 value, int precision) => Functions.Round(value, precision);
        public Vector2 Round(Vector2 value, int precision) => Functions.Round(value, precision);

        public float AverageOverFrames(float[] data, int length) => Functions.AverageOverFrames(data, length);
        public Vector3 AverageOverFrames(Vector3[] data, int length) => Functions.AverageOverFrames(data, length);

        public GameObject GetSiblingGameObject(string SiblingName) => Functions.GetSiblingGameObject(SiblingName);

        public Quaternion ToQuaternion(float yaw, float pitch, float roll) => Functions.ToQuaternion(yaw, pitch, roll);
        public Quaternion ToQuaternion(Vector3 vector3) => Functions.ToQuaternion(vector3);

        public void Kill(GameObject gameobject) => Functions.Kill(gameobject);
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
            public Vector3 position;
            public Transform transform;
            public Vector3 velocity;
            public Direction direction;
            public int health;
        }
    }
}
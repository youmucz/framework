using Godot;
using System.Collections;
using System.Collections.Generic;

namespace framework.extensions
{
    public static class GeometryExtensions
    {
        public static List<Vector3> FibonacciSphere(int n)
        {
            var list = new List<Vector3>();
            var phi = Mathf.Pi * (3f - Mathf.Sqrt(5f));

            for (var i = 0; i < n; i++)
            {
                var y = 1f - (i / (float)(n - 1)) * 2f;
                var r = Mathf.Sqrt(1f - y * y);
                var theta = phi * i;

                var x = Mathf.Cos(theta) * r;
                var z = Mathf.Sin(theta) * r;

                list.Add(new Vector3(x, y, z));
            }

            return list;
        }
    }
}


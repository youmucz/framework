using Godot;
using System;
using System.Diagnostics;

namespace Minikit
{
    /// <summary> Helper class with additional functions that aren't included in Unity's Debug class </summary>
    public static class Debug
    {
        // Sphere with radius of 1
        private static readonly Vector4[] UnitSphere = MakeUnitSphere(16);

        private static Vector4[] MakeUnitSphere(int length)
        {
            System.Diagnostics.Debug.Assert(length > 2);
            var v = new Vector4[length * 3];
            for (int i = 0; i < length; i++)
            {
                var f = i / (float)length;
                float c = Mathf.Cos(f * (float)(Mathf.Pi * 2.0));
                float s = Mathf.Sin(f * (float)(Mathf.Pi * 2.0));
                v[0 * length + i] = new Vector4(c, s, 0, 1);
                v[1 * length + i] = new Vector4(0, c, s, 1);
                v[2 * length + i] = new Vector4(s, 0, c, 1);
            }
            return v;
        }

        public static void DrawSphere(Vector4 _position, float _radius, Color _color, float _duration = 0f)
        {
            Vector4[] v = UnitSphere;
            int len = UnitSphere.Length / 3;
            for (int i = 0; i < len; i++)
            {
                var sX = _position + _radius * v[0 * len + i];
                var eX = _position + _radius * v[0 * len + (i + 1) % len];
                var sY = _position + _radius * v[1 * len + i];
                var eY = _position + _radius * v[1 * len + (i + 1) % len];
                var sZ = _position + _radius * v[2 * len + i];
                var eZ = _position + _radius * v[2 * len + (i + 1) % len];
                // Debug.DrawLine(sX, eX, _color, _duration);
                // Debug.DrawLine(sY, eY, _color, _duration);
                // Debug.DrawLine(sZ, eZ, _color, _duration);
            }
        }

        public static void DrawWireBox(Vector3 _position, Quaternion _rotation, Vector3 _size, Color _color, float _duration = 0f)
        {
            // Matrix4x4 m = new Matrix4x4();
            // m.SetTRS(_position, _rotation, _size);
            //
            // var point1 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, 0.5f));
            // var point2 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, 0.5f));
            // var point3 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, -0.5f));
            // var point4 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, -0.5f));
            //
            // var point5 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, 0.5f));
            // var point6 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, 0.5f));
            // var point7 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, -0.5f));
            // var point8 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, -0.5f));
            //
            // Debug.DrawLine(point1, point2, _color, _duration);
            // Debug.DrawLine(point2, point3, _color, _duration);
            // Debug.DrawLine(point3, point4, _color, _duration);
            // Debug.DrawLine(point4, point1, _color, _duration);
            //
            // Debug.DrawLine(point5, point6, _color, _duration);
            // Debug.DrawLine(point6, point7, _color, _duration);
            // Debug.DrawLine(point7, point8, _color, _duration);
            // Debug.DrawLine(point8, point5, _color, _duration);
            //
            // Debug.DrawLine(point1, point5, _color, _duration);
            // Debug.DrawLine(point2, point6, _color, _duration);
            // Debug.DrawLine(point3, point7, _color, _duration);
            // Debug.DrawLine(point4, point8, _color, _duration);
        }

        public static void DrawArrow(Vector3 _position, Vector3 _direction, Color _color, float _duration = 0f, float _arrowHeadLength = 0.25f, float _arrowHeadAngle = 20.0f)
        {
            // Debug.DrawRay(_position, _direction, _color, _duration);
            //
            // Vector3 right = Quaternion.LookRotation(_direction) * Quaternion.Euler(0, 180 + _arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            // Vector3 left = Quaternion.LookRotation(_direction) * Quaternion.Euler(0, 180 - _arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            // Debug.DrawRay(_position + _direction, right * _arrowHeadLength, _color, _duration);
            // Debug.DrawRay(_position + _direction, left * _arrowHeadLength, _color, _duration);
        }
    }
}

using Godot;
using System;
using System.Diagnostics;

namespace framework.debug
{
    public static class DebugLog
    {
        public static void InfoLog(string message)
        {
            GD.Print(message);
        }

        public static void ErrorLog(string message)
        {
            GD.PrintErr(message);
        }
    }
}

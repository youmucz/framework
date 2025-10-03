using System;
using Godot;
using System.Collections.Generic;
using Range = Godot.Range;

namespace framework.extensions
{
    public static class ListExtensions
    {
        /// <summary> Shuffles the element order of the specified list. </summary>
        public static void Shuffle<T>(this IList<T> ts)
        {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r =  GD.RandRange(i, count);
                (ts[i], ts[r]) = (ts[r], ts[i]);
            }
        }
    }
}

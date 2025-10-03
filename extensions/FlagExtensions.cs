using System;
using System.Collections;
using System.Collections.Generic;

namespace framework.extensions
{
    public static class FlagExtensions
    {
        /// <summary> Checks if an enum value has any overlapping values with another enum of the same type </summary>
        public static bool HasAnyFlags(this Enum left, Enum right)
        {
            return (Convert.ToInt32(left) & Convert.ToInt32(right)) != 0;
        }
    }
}

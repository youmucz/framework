using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minikit
{
    public static class MKFlagExtensions
    {


        /// <summary> Checks if an enum value has any overlapping values with another enum of the same type </summary>
        public static bool HasAnyFlags(this Enum _left, Enum _right)
        {
            return (Convert.ToInt32(_left) & Convert.ToInt32(_right)) != 0;
        }
    }
} // Minikit namespace

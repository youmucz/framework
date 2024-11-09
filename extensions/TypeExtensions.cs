using System;
using System.Collections;
using System.Collections.Generic;

namespace Minikit
{
    public static class TypeExtensions
    {
        public static bool IsStatic(this Type type)
        {
            return type.IsAbstract && type.IsSealed;
        }
    }
}

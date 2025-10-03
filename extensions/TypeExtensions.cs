using System;
using System.Collections;
using System.Collections.Generic;

namespace framework.extensions
{
    public static class TypeExtensions
    {
        public static bool IsStatic(this Type type)
        {
            return type.IsAbstract && type.IsSealed;
        }
    }
}

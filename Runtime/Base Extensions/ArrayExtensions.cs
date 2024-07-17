using System;

namespace FantasticCore.Runtime.Base_Extensions
{
    public static class ArrayExtensions
    {
        public static bool IsArrayValidAndNotEmpty(this Array array)
            => array is { Length: > 0 };
    }
}
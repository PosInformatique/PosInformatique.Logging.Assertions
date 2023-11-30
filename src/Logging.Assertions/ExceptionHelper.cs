//-----------------------------------------------------------------------
// <copyright file="ExceptionHelper.cs" company="P.O.S Informatique">
//     Copyright (c) P.O.S Informatique. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace PosInformatique.Logging.Assertions
{
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal static class ExceptionHelper
    {
        private static readonly MethodInfo GetExceptionPointersMethod = typeof(Marshal).GetMethod("GetExceptionPointers");

        public static bool IsExceptionOnGoing()
        {
            var ptr = (IntPtr)GetExceptionPointersMethod.Invoke(null, null);

            if (ptr == IntPtr.Zero)
            {
                return false;
            }

            return true;
        }
    }
}

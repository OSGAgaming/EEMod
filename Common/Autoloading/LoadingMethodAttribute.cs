using EEMod.Common.IDs;
using System;

namespace EEMod.Common.Autoloading
{
    /// <summary>
    /// Methods with this attribute will be called during <see cref="EEMod.Load"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    internal class LoadingMethodAttribute : Attribute
    {
        public LoadModeID mode;

        public LoadingMethodAttribute(LoadModeID loadmode) => mode = loadmode;

        public LoadingMethodAttribute()
        {
        }
    }
}
using EEMod.Common.IDs;
using System;

namespace EEMod.Common.Autoloading.AutoloadTypes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TypeListenerAttribute : Attribute
    {
        public TypeListenerAttribute()
        {
        }

        internal TypeListenerAttribute(LoadModeID mode) => loadMode = mode;

        internal LoadModeID loadMode;
    }
}
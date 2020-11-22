using System;

namespace EEMod.Common.Autoloading
{
    /// <summary>
    /// Methods with this attribute will be called during <see cref="EEMod.Unload"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class UnloadingMethodAttribute : Attribute { }
}
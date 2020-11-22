﻿using System;

namespace EEMod.Common.Autoloading
{
    /// <summary>
    /// Static fields with this attribute will <strong>NOT</strong> be set to null during unload.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    internal class UnloadIgnoreAttribute : Attribute { }
}
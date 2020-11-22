using Terraria.Localization;

namespace EEMod.Common.Utilities
{
    public static class LanguageUtilities
    {
        internal static string GetEEModTextValue(string key) => Language.GetTextValue($"Mods.EEMod.{key}");
    }
}
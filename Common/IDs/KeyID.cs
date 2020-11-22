using Terraria;

namespace EEMod.Common.IDs
{
    public class KeyID
    {
        public const string Pyramids = "Pyramids";
        public const string Sea = "Sea";
        public const string CoralReefs = "CoralReefs";
        public const string Island = "Island";
        public const string VolcanoIsland = "VolcanoIsland";
        public const string VolcanoInside = "VolcanoInside";
        public const string Cutscene1 = "Cutscene1";
        public const string Island2 = "Island2";

        public static string BaseWorldName => Main.LocalPlayer.GetModPlayer<EEPlayer>().baseWorldName;
    }
}
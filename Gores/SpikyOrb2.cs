using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EEMod.Gores
{
    public class SpikyOrb2 : ModGore
    {
        public override void OnSpawn(Gore gore)
        {
            gore.behindTiles = true;
            gore.timeLeft = 180;
            updateType = GoreID.CrimsonBunnyHead;
        }
    }
}
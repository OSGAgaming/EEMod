using EEMod.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace EEMod.Gores
{
    public class GlowshroomCannonGore1 : ModGore
    {
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            gore.behindTiles = true;
            gore.timeLeft = 180;
            //updateType = GoreID.CrimsonBunnyHead;
        }
    }
}
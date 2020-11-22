using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EEMod.Tiles.Walls
{
    public class AnemoneWallTile : ModWall
    {
        public override void SetDefaults()
        {
            AddMapEntry(new Color(66, 46, 156));

            Main.wallHouse[Type] = false;
            dustType = DustID.ToxicBubble;
            soundStyle = 1;
        }
    }
}
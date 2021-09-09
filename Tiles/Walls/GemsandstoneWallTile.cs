using EEMod.Items.Placeables.Walls;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EEMod.Tiles.Walls
{
    public class GemsandstoneWallTile : ModWall
    {
        public override void SetStaticDefaults()
        {
            AddMapEntry(new Color(67, 47, 155));

            Main.wallHouse[Type] = true;
            dustType = DustID.Rain;
            drop = ModContent.ItemType<GemsandstoneWall>();
            soundStyle = 1;
        }
    }
}
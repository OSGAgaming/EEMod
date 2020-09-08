using EEMod.Items.Placeables;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace EEMod.Tiles
{
    public class GlacialIceTile : ModTile
    {
        /*public override void ChangeWaterfallStyle(ref int style)
        {
            style = mod.GetWaterfallStyleSlot("Surfacebg");
        }*/

        public override void SetDefaults()
        {
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlendAll[Type] = true;

            AddMapEntry(new Color(48, 115, 135));

            dustType = 154;
            drop = ModContent.ItemType<GlacialIce>();
            soundStyle = 1;
            mineResist = 1f;
            minPick = 0;
        }
    }
}
using EEMod.Items.Placeables;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EEMod.Tiles
{
    public class KelpRockTile : EETile
    {
        public override void SetDefaults()
        {
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlendAll[Type] = true;

            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = true;

            AddMapEntry(new Color(48, 200, 135));

            dustType = DustID.Rain;
            drop = ModContent.ItemType<Gemsandstone>();
            SoundStyle = 1;
            mineResist = 1f;
            minPick = 0;
        }
    }
}
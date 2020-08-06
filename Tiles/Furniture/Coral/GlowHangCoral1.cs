using Terraria;
using Terraria.ObjectData;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using System;

namespace EEMod.Tiles.Furniture.Coral
{
    public class GlowHangCoral1 : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1xX);
            TileObjectData.newTile.Height = 11;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16,16,16,16,16,16,16,16,16,16,16 };
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 0);
            TileObjectData.newTile.AnchorBottom = default;
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Coral Lamp");
            AddMapEntry(new Color(0, 100, 200), name);
            dustType = DustID.Dirt;
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.frameX < 18)
            {
                r = 0.05f;
                g = 0.05f;
                b = 0.05f;
            }
        }
        float kaylerp = 0;
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            if (tile != null && tile.active() && tile.type == Type)
            {
            kaylerp += 0.001f;
            Color color = Color.White;
            Main.tile[i, j].frameX = 17;
            int frameX = Main.tile[i, j].frameX;
            int frameY = Main.tile[i, j].frameY;
            int width = 20;
            int offsetY = 2;
            int height = 20;
            int offsetX = 2;
            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }
                for (int k = 0; k < 7; k++)
                {
                    Main.spriteBatch.Draw(mod.GetTexture("Tiles/Furniture/Coral/GlowHangCoral1Glow"), new Vector2(i * 16 - (int)Main.screenPosition.X + offsetX - (width - 16f) / 2f, j * 16 - (int)Main.screenPosition.Y + offsetY) + zero, new Rectangle(frameX, frameY, width, height), color * ((float)Math.Sin(kaylerp) * 0.5f + 0.5f), 0f, default, 1f, SpriteEffects.None, 0f);
                }
            }
        }
    }
}
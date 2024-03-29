﻿using EEMod.Items.Materials;
using EEMod.NPCs;
using EEMod.NPCs.TropicalIslands;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace EEMod.Tiles.Foliage
{
    public class TropicalTree : EETile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolidTop[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 11;
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 24 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Direction = TileObjectDirection.None;
            // TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Tropical Tree");
            AddMapEntry(new Color(20, 60, 20), name);
            DisableSmartCursor = true;
            DustType = DustID.Dirt;
        }

        public override void KillMultiTile(int i, int j, int TileFrameX, int TileFrameY)
        {
            if (Main.rand.NextBool(5))
            {
                NPC.NewNPC(null, i, j, ModContent.NPCType<Cococritter>());
            }
            Item.NewItem(null, new Vector2(i, j), ModContent.ItemType<TropicalWoodItem>(), Main.rand.Next(12, 24));
            Item.NewItem(null, new Vector2(i, j), ModContent.ItemType<Coconut>(), Main.rand.Next(3, 5));
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            base.SpecialDraw(i, j, spriteBatch);

            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }

            if (Framing.GetTileSafely(i, j).TileFrameX == 0 && Framing.GetTileSafely(i, j).TileFrameY == 0)
                Main.spriteBatch.Draw(Mod.Assets.Request<Texture2D>("Tiles/Foliage/TropicalTreeLeaves").Value, new Vector2((i * 16) - 48, (j * 16) - 60) - Main.screenPosition + zero, new Rectangle(0, 0, 120, 100), Lighting.GetColor(i, j));
        }
    }
}
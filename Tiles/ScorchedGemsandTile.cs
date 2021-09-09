using EEMod.Items.Placeables;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace EEMod.Tiles
{
    public class ScorchedGemsandTile : EETile
    {
        public override void SetDefaults()
        {
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlendAll[Type] = true;

            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = true;

            Main.tileFrameImportant[Type] = true;

            AddMapEntry(new Color(204, 51, 0));

            dustType = DustID.Rain;
            soundStyle = 1;
            mineResist = 1f;
            minPick = 0;
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            int tilescale = 18;

            int frameXOffset = (i % 2) * 72;
            int frameYOffset = (j % 2) * 90;

            int newFrameX = 0;
            int newFrameY = 0;

            Tile tileAbove = Main.tile[i, j - 1];
            Tile tileBelow = Main.tile[i, j + 1];

            Tile tileLeft = Main.tile[i - 1, j];
            Tile tileRight = Main.tile[i + 1, j];

            Tile tileTopLeft = Main.tile[i - 1, j - 1];
            Tile tileTopRight = Main.tile[i + 1, j - 1];

            Tile tileBottomLeft = Main.tile[i - 1, j + 1];
            Tile tileBottomRight = Main.tile[i + 1, j + 1];

            if (tileAbove.active() && tileBelow.active() && !tileLeft.active() && !tileRight.active())
            {
                newFrameX = 1;
                newFrameY = 3;
            }

            if (!tileAbove.active() && !tileBelow.active() && tileLeft.active() && tileRight.active())
            {
                newFrameX = 0;
                newFrameY = 3;
            }

            if (!tileAbove.active() && !tileBelow.active() && !tileLeft.active() && !tileRight.active())
            {
                newFrameX = 2;
                newFrameY = 3;
            }

            if (!tileAbove.active() && tileBelow.active() && tileLeft.active() && tileRight.active())
            {
                newFrameX = 1;
                newFrameY = 0;
            }

            if (tileAbove.active() && tileBelow.active() && !tileLeft.active() && tileRight.active())
            {
                newFrameX = 0;
                newFrameY = 1;
            }

            if (tileAbove.active() && !tileBelow.active() && tileLeft.active() && tileRight.active())
            {
                newFrameX = 1;
                newFrameY = 2;
            }

            if (tileAbove.active() && tileBelow.active() && tileLeft.active() && !tileRight.active())
            {
                newFrameX = 2;
                newFrameY = 1;
            }

            if (!tileAbove.active() && tileBelow.active() && !tileLeft.active() && !tileRight.active())
            {
                newFrameX = 3;
                newFrameY = 0;
            }

            if (tileAbove.active() && !tileBelow.active() && !tileLeft.active() && !tileRight.active())
            {
                newFrameX = 3;
                newFrameY = 3;
            }

            if (!tileAbove.active() && !tileBelow.active() && !tileLeft.active() && tileRight.active())
            {
                newFrameX = 0;
                newFrameY = 4;
            }

            if (!tileAbove.active() && !tileBelow.active() && tileLeft.active() && !tileRight.active())
            {
                newFrameX = 3;
                newFrameY = 4;
            }

            if (!tileAbove.active() && tileBelow.active() && !tileLeft.active() && tileRight.active())
            {
                newFrameX = 0;
                newFrameY = 0;
            }


            if (!tileAbove.active() && tileBelow.active() && tileLeft.active() && !tileRight.active())
            {
                newFrameX = 2;
                newFrameY = 0;
            }

            if (tileAbove.active() && !tileBelow.active() && !tileLeft.active() && tileRight.active())
            {
                newFrameX = 0;
                newFrameY = 2;
            }

            if (tileAbove.active() && !tileBelow.active() && tileLeft.active() && !tileRight.active())
            {
                newFrameX = 2;
                newFrameY = 2;
            }


            if (tileAbove.active() && tileBelow.active() && tileLeft.active() && tileRight.active())
            {
                newFrameX = 1;
                newFrameY = 1;

                if (tileTopRight.active() && tileBottomRight.active() && !tileTopLeft.active() && !tileBottomLeft.active())
                {
                    newFrameX = 1;
                    newFrameY = 4;
                }

                if (tileTopLeft.active() && tileBottomLeft.active() && !tileTopRight.active() && !tileBottomRight.active())
                {
                    newFrameX = 2;
                    newFrameY = 4;
                }

                if (tileTopLeft.active() && tileTopRight.active() && !tileBottomLeft.active() && !tileBottomRight.active())
                {
                    newFrameX = 3;
                    newFrameY = 2;
                }

                if (tileBottomLeft.active() && tileBottomRight.active() && !tileTopLeft.active() && !tileTopRight.active())
                {
                    newFrameX = 3;
                    newFrameY = 1;
                }
            }

            Texture2D tex = ModContent.Request<Texture2D>("EEMod/Tiles/BrimstoneTile");

            tile.frameX = (short)(((newFrameX * tilescale) + frameXOffset) + ((Math.Sin((i - (i % 2)) * (j - (j % 2))) >= 0) ? 144 : 0));
            tile.frameY = (short)((newFrameY * tilescale) + frameYOffset);

            return true;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Texture2D tex = ModContent.Request<Texture2D>("EEMod/Tiles/ScorchedGemsandTileGlow");

            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }

            Main.spriteBatch.Draw(tex, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(Main.tile[i, j].frameX, Main.tile[i, j].frameY, 16, 16), Lighting.GetColor(i, j) * (1.5f + ((float)Math.Sin((i * j) + Main.GameUpdateCount / 20f) * 0.3f)), 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
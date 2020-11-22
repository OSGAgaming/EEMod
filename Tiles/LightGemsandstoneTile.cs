using EEMod.Items.Placeables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace EEMod.Tiles
{
    public class LightGemsandstoneTile : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileMergeDirt[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlendAll[Type] = true;

            AddMapEntry(new Color(88, 179, 179));

            dustType = 154;
            drop = ModContent.ItemType<LightGemsandstone>();
            soundStyle = 1;
            mineResist = 1f;
            minPick = 0;
        }

        private void PlaceGroundGrass(int i, int j)
        {
            int noOfGrassBlades = (int)(((i + j) % 16) * 0.4f);
            string tex = "Tiles/KelpGrassLong";
            string tex3 = "Tiles/KelpGrassShort";
            string tex4 = "Tiles/KelpGrassStubbed";
            string tex5 = "Tiles/KelpGrassLongX";
            string tex6 = "Tiles/KelpGrassLongXX";
            string tex2 = "Tiles/KelpGrassMedium";
            string Chosen = tex;

            for (int a = 0; a < noOfGrassBlades; a++)
            {
                switch ((i + j + a * 7) % 6)
                {
                    case 0:
                        Chosen = tex;
                        break;

                    case 1:
                        Chosen = tex2;
                        break;

                    case 2:
                        Chosen = tex3;
                        break;

                    case 3:
                        Chosen = tex4;
                        break;

                    case 4:
                        Chosen = tex5;
                        break;

                    case 5:
                        Chosen = tex6;
                        break;
                }
                float pos = i * 16 + (i + j * a + a * 7) % 16;
                ModContent.GetInstance<EEMod>().TVH.AddElement(new Leaf(new Vector2(pos, j * 16), Chosen, 0f, Color.Lerp(Color.LightGreen, Color.Green, ((i + j + a * 3) % 4) / 4f), ((i + j * a * 2) % 2 == 0)));
            }
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            PlaceGroundGrass(i, j);
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (!Main.tileSolid[Framing.GetTileSafely(i, j - 1).type] || !Framing.GetTileSafely(i, j - 1).active() && Framing.GetTileSafely(i, j).slope() == 0)
            {
                PlaceGroundGrass(i, j);
            }
            if (!Main.tileSolid[Framing.GetTileSafely(i, j + 1).type] || !Framing.GetTileSafely(i, j + 1).active() && Framing.GetTileSafely(i, j).slope() == 0)
            {
            }
            return true;
        }
    }
}
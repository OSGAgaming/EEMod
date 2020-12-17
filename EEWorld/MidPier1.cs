using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EEMod.Tiles;
using Terraria.ModLoader;
using System.Threading.Tasks;
using Terraria.ID;
using static EEMod.EEWorld.EEWorld.MidPier1ArrayVals;

namespace EEMod.EEWorld
{
    public partial class EEWorld
    {
        internal static class MidPier1ArrayVals
        {
            internal const ushort A = 0;
            internal const ushort B = TileID.Rope;
            internal const ushort C = TileID.Sand;
            internal const ushort D = TileID.PalmWood;
            internal const ushort E = TileID.WoodBlock;
            internal const ushort F = TileID.LivingWood;
            internal const ushort G = TileID.Ebonwood;
            internal const ushort H = TileID.BeachPiles;
            internal const ushort I = TileID.Platforms;
            internal const ushort J = TileID.Lamps;
            internal const ushort K = TileID.Coral;
            internal const ushort L = TileID.Painting3X3;
            internal const ushort M = TileID.RichMahogany;
            internal const ushort N = TileID.Chain;
            internal const ushort O = TileID.Banners;
            internal const ushort A0 = 0;
            internal const ushort B1 = WallID.BorealWoodFence;
            internal const ushort C2 = WallID.ShadewoodFence;
            internal const ushort D3 = WallID.EbonwoodFence;
            internal const ushort E4 = WallID.RichMahoganyFence;
            internal const ushort F5 = WallID.RichMaogany;
            internal const ushort G6 = WallID.PalmWoodFence;
            internal const ushort H7 = WallID.Ebonwood;
        }
        public static int[,,] MidPier1 = new int[,,]
        {
{{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{E,0,0,0,0,0,0,0,0,54}},
{{B,B1,0,0,0,0,0,0,126,72},{B,B1,0,0,0,0,0,0,126,72},{B,B1,0,0,0,0,0,0,144,72},{B,B1,0,0,0,0,0,0,144,72},{B,B1,0,0,0,0,0,0,108,72},{B,B1,0,0,0,0,0,0,216,36},{N,C2,0,0,28,0,0,0,144,0},{B,B1,0,0,0,0,0,0,162,36},{B,B1,0,0,0,0,0,0,126,72},{B,B1,0,0,0,0,0,0,108,72},{B,B1,0,0,0,0,0,0,108,72},{B,B1,0,0,0,0,0,0,144,72},{B,B1,0,0,0,0,0,0,108,72},{B,B1,0,0,0,0,0,0,108,72},{E,B1,0,0,0,0,0,0,162,0},{D,D3,28,0,28,0,0,0,36,36}},
{{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,G6,0,0,28,0,0,0,0,0},{0,G6,0,0,28,0,0,0,0,0},{N,C2,0,0,28,0,0,0,126,54},{0,G6,0,0,28,0,0,0,0,0},{0,G6,0,0,28,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{O,G6,0,0,28,0,0,0,54,0},{0,D3,0,0,28,0,0,0,0,0}},
{{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,G6,0,0,28,0,0,0,0,0},{0,C2,0,0,28,0,0,0,0,0},{0,G6,0,0,28,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{O,G6,0,0,28,0,0,0,54,18},{0,D3,0,0,28,0,0,0,0,0}},
{{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{L,G6,0,0,28,0,0,0,540,54},{L,C2,0,0,28,0,0,0,558,54},{L,G6,0,0,28,0,0,0,576,54},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{O,G6,0,0,28,0,0,0,54,36},{0,D3,0,0,28,0,0,0,0,0}},
{{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{L,G6,0,0,28,0,0,0,540,72},{L,C2,0,0,28,0,0,0,558,72},{L,G6,0,0,28,0,0,0,576,72},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,G6,0,0,28,0,0,0,0,0},{0,D3,0,0,28,0,0,0,0,0}},
{{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{L,G6,0,0,28,0,0,0,540,90},{L,C2,0,0,28,0,0,0,558,90},{L,G6,0,0,28,0,0,0,576,90},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,G6,0,0,28,0,0,0,0,0},{0,D3,0,0,28,0,0,0,0,0}},
{{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{J,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,G6,0,0,28,0,0,0,0,0},{0,C2,0,0,28,0,0,0,0,0},{0,G6,0,0,28,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{J,0,0,0,0,0,0,0,18,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,G6,0,0,28,0,0,0,0,0},{0,D3,0,0,28,0,0,0,0,0}},
{{0,0,0,0,0,0,0,0,0,0},{0,G6,0,0,0,0,0,0,0,0},{J,0,0,0,0,0,0,0,0,18},{0,0,0,0,0,0,0,0,0,0},{0,G6,0,0,28,0,0,0,0,0},{H,G6,0,0,28,0,0,0,22,0},{0,C2,0,0,28,0,0,0,0,0},{0,G6,0,0,28,0,0,0,0,0},{0,G6,0,0,28,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{J,0,0,0,0,0,0,0,18,18},{0,0,0,0,0,0,0,0,0,0},{0,G6,0,0,28,0,0,0,0,0},{0,G6,0,0,0,0,0,0,0,0},{0,D3,0,0,28,0,0,0,0,0}},
{{C,C2,0,0,28,0,0,0,54,0},{C,G6,0,0,28,0,0,0,18,54},{J,G6,0,0,28,0,0,0,0,36},{0,G6,0,0,28,0,0,0,0,0},{0,G6,0,0,28,0,0,0,0,0},{C,G6,0,0,28,0,0,0,0,54},{C,G6,0,0,28,0,0,0,54,0},{C,G6,0,0,28,0,0,0,54,54},{0,G6,0,0,28,0,0,0,0,0},{0,G6,0,0,28,0,0,0,0,0},{H,G6,28,0,28,0,0,0,0,0},{J,G6,0,0,28,0,0,0,18,36},{0,G6,0,0,28,0,0,0,0,0},{H,G6,0,0,28,0,0,0,44,0},{0,G6,0,0,28,0,0,0,0,0},{0,C2,0,0,28,0,0,0,0,0}},
{{D,0,0,0,0,0,0,0,18,36},{D,0,0,0,0,0,0,0,54,36},{D,0,0,0,0,0,0,0,36,0},{D,0,0,0,0,0,0,0,144,72},{D,0,0,0,0,0,0,0,144,72},{D,0,0,0,0,0,0,0,54,36},{D,0,0,0,0,0,0,0,36,18},{D,0,0,0,0,0,0,0,54,36},{D,0,0,0,0,0,0,0,108,72},{D,0,0,0,0,0,0,0,54,0},{D,0,0,0,0,0,0,0,144,72},{D,0,0,0,0,0,0,0,126,72},{D,0,0,0,0,0,0,0,144,72},{D,0,0,0,0,0,0,0,54,0},{D,0,0,0,0,0,0,0,144,72},{D,C2,0,0,0,0,0,0,108,72}},
{{0,D3,0,0,28,0,0,0,0,0},{0,F5,0,0,28,0,0,0,0,0},{G,F5,28,0,28,0,0,0,90,36},{0,F5,0,0,28,0,0,0,0,0},{0,F5,0,0,28,0,0,0,0,0},{M,F5,28,2,28,0,0,0,126,0},{M,F5,28,4,28,0,0,0,144,54},{0,F5,0,0,28,0,0,0,0,0},{0,F5,0,0,28,0,0,0,0,0},{M,F5,28,4,28,0,0,0,0,72},{M,F5,28,1,28,0,0,0,90,54},{0,F5,0,0,28,0,0,0,0,0},{0,F5,0,0,28,0,0,0,0,0},{G,F5,28,0,28,0,0,0,90,36},{0,F5,0,0,28,0,0,0,0,0},{0,D3,0,0,28,0,0,0,0,0}},
{{E,D3,0,1,28,0,0,0,54,54},{0,E4,0,0,28,0,0,0,0,0},{E,0,0,0,0,0,0,0,0,18},{E,0,0,0,0,0,0,0,108,72},{E,0,0,0,0,0,0,0,144,72},{E,0,0,0,0,0,0,0,54,36},{E,0,0,0,0,0,0,0,126,72},{E,0,0,0,0,0,0,0,126,72},{E,0,0,0,0,0,0,0,126,72},{E,0,0,0,0,0,0,0,108,72},{E,0,0,0,0,0,0,0,18,36},{E,0,0,0,0,0,0,0,144,72},{E,0,0,0,0,0,0,0,108,72},{E,0,0,0,0,0,0,0,72,18},{0,E4,0,0,28,0,0,0,0,0},{E,C2,0,0,28,0,0,0,72,54}},
{{F,E4,0,0,28,0,0,0,72,0},{F,E4,0,2,28,0,0,0,0,54},{F,0,0,3,0,0,0,0,90,72},{B,E4,0,0,28,0,0,0,90,0},{0,E4,0,0,28,0,0,0,0,0},{I,0,0,0,0,0,0,0,90,180},{N,0,28,0,0,0,0,0,108,0},{I,0,0,0,0,0,0,0,36,180},{I,0,0,0,0,0,0,0,18,180},{N,0,0,0,0,0,0,0,144,0},{I,0,0,0,0,0,0,0,90,180},{0,E4,0,0,28,0,0,0,0,0},{0,E4,0,0,28,0,0,0,0,0},{F,0,0,4,0,0,0,0,0,72},{F,E4,0,1,28,0,0,0,54,54},{F,E4,0,0,28,0,0,0,0,0}},
{{F,0,0,0,0,0,0,0,54,18},{F,C2,0,3,28,0,0,0,54,72},{0,0,0,0,0,0,0,0,0,0},{B,E4,0,0,28,0,0,0,126,54},{0,E4,0,0,28,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{N,0,28,0,0,0,0,0,126,54},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{N,H7,28,0,28,0,0,0,90,0},{0,0,0,0,0,0,0,0,0,0},{0,E4,0,0,28,0,0,0,0,0},{B,E4,0,0,0,0,0,0,108,0},{0,H7,0,0,28,0,0,0,0,0},{F,H7,0,4,28,0,0,0,72,72},{F,0,0,0,0,0,0,0,36,18}},
{{F,0,0,0,0,0,0,0,72,36},{0,C2,0,0,28,0,0,0,0,0},{K,0,0,0,0,0,0,0,78,0},{0,E4,0,0,28,0,0,0,0,0},{K,E4,0,0,28,0,0,0,104,0},{0,H7,0,0,28,0,0,0,0,0},{0,H7,0,0,28,0,0,0,0,0},{0,H7,0,0,28,0,0,0,0,0},{0,H7,0,0,28,0,0,0,0,0},{N,H7,0,0,28,0,0,0,144,54},{H,H7,0,0,28,0,0,0,22,0},{0,E4,0,0,28,0,0,0,0,0},{B,E4,0,0,0,0,0,0,90,36},{0,H7,0,0,28,0,0,0,0,0},{0,H7,0,0,28,0,0,0,0,0},{F,H7,0,0,28,0,0,0,0,36}},
{{F,F5,0,0,28,0,0,0,198,0},{G,F5,28,0,28,0,0,0,108,72},{G,F5,28,0,28,0,0,0,126,72},{G,E4,28,0,28,0,0,0,144,72},{G,E4,28,0,28,0,0,0,108,72},{G,F5,28,1,28,0,0,0,90,54},{0,F5,0,0,28,0,0,0,0,0},{G,F5,28,0,28,0,0,0,162,0},{G,F5,28,0,28,0,0,0,108,72},{G,F5,28,0,0,0,0,0,126,72},{G,F5,28,0,0,0,0,0,144,72},{G,E4,28,0,28,0,0,0,108,72},{G,E4,28,0,28,0,0,0,144,72},{G,F5,28,0,0,0,0,0,126,72},{G,F5,28,0,0,0,0,0,108,72},{F,F5,0,0,0,0,0,0,180,0}},
{{F,0,0,0,0,0,0,0,72,18},{0,C2,0,0,28,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,E4,0,0,28,0,0,0,0,0},{B,E4,0,0,28,0,0,0,108,54},{G,0,28,4,0,0,0,0,36,72},{G,0,28,1,0,0,0,0,216,18},{0,E4,0,0,28,0,0,0,0,0},{0,E4,0,0,28,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{B,E4,0,0,28,0,0,0,90,18},{0,E4,0,0,28,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{I,C2,0,0,28,0,0,0,126,180},{F,0,0,0,0,0,0,0,0,18}},
{{F,0,0,0,0,0,0,0,72,18},{0,C2,0,0,28,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,E4,0,0,28,0,0,0,0,0},{0,E4,0,0,28,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,E4,0,0,0,0,0,0,0,0},{0,E4,0,0,28,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{B,E4,0,0,28,0,0,0,144,54},{0,E4,0,0,28,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,C2,0,0,28,0,0,0,0,0},{F,0,0,0,0,0,0,0,0,36}},
{{F,0,0,0,0,0,0,0,72,36},{H,C2,0,0,28,0,0,0,22,0},{0,0,0,0,0,0,0,0,0,0},{0,E4,0,0,28,0,0,0,0,0},{B,E4,0,0,28,0,0,0,108,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,E4,0,0,28,0,0,0,0,0},{0,E4,0,0,28,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,E4,0,0,28,0,0,0,0,0},{0,E4,0,0,28,0,0,0,0,0},{K,0,0,0,0,0,0,0,0,0},{0,C2,0,0,28,0,0,0,0,0},{F,0,0,0,0,0,0,0,0,36}},
{{F,F5,0,0,28,0,0,0,198,36},{G,F5,28,0,28,0,0,0,126,72},{G,F5,28,0,28,0,0,0,126,72},{G,E4,28,0,28,0,0,0,108,72},{G,E4,28,0,28,0,0,0,144,72},{G,F5,28,0,28,0,0,0,144,72},{G,F5,28,0,28,0,0,0,144,72},{G,F5,28,0,28,0,0,0,144,72},{G,F5,28,0,28,0,0,0,144,72},{G,F5,28,0,28,0,0,0,144,72},{G,F5,28,0,28,0,0,0,144,72},{G,E4,28,0,28,0,0,0,126,72},{G,E4,28,0,28,0,0,0,144,72},{G,F5,28,0,28,0,0,0,126,72},{G,F5,28,0,0,0,0,0,144,72},{F,F5,0,0,0,0,0,0,180,18}},
{{F,0,0,0,0,0,0,0,72,36},{I,C2,0,0,28,0,0,0,108,180},{0,0,0,0,0,0,0,0,0,0},{0,E4,0,0,28,0,0,0,0,0},{B,E4,0,0,0,0,0,0,90,18},{0,H7,0,0,28,0,0,0,0,0},{0,H7,0,0,28,0,0,0,0,0},{0,H7,0,0,28,0,0,0,0,0},{0,H7,0,0,28,0,0,0,0,0},{0,H7,0,0,28,0,0,0,0,0},{0,H7,0,0,28,0,0,0,0,0},{0,E4,0,0,28,0,0,0,0,0},{B,E4,0,0,28,0,0,0,90,18},{0,H7,0,0,28,0,0,0,0,0},{0,H7,0,0,28,0,0,0,0,0},{F,H7,0,0,28,0,0,0,0,0}},
{{F,0,0,0,0,0,0,0,72,0},{0,C2,0,0,28,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,E4,0,0,28,0,0,0,0,0},{B,E4,0,0,28,0,0,0,90,18},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,H7,0,0,28,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,0,0,0,0,0,0,0,0,0},{0,E4,0,0,28,0,0,0,0,0},{B,E4,0,0,0,0,0,0,90,0},{0,0,0,0,0,0,0,0,0,0},{0,H7,0,0,28,0,0,0,0,0},{F,H7,0,0,0,0,0,0,0,18}},
        };
    }
}

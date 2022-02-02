using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using EEMod.Seamap.SeamapContent;
using EEMod.ID;
using Microsoft.Xna.Framework.Graphics;

namespace EEMod.Seamap.SeamapAssets
{
    public class TropicalIslandAlt : Island
    {
        public override string name => "Tropical Island Alt";
        public override int framecount => 1;
        public override int framespid => 0;
        public override bool cancollide => true;

        public override Texture2D islandTex => ModContent.Request<Texture2D>("EEMod/Seamap/SeamapAssets/TropicalIslandAlt", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        public TropicalIslandAlt(Vector2 pos): base(pos)
        {
            width = 230;
            height = 170;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.GameContent.UI.States;

/*using Terraria.Localization;*/

using Terraria.Utilities;
using Terraria.World.Generation;

namespace EEMod.Worldbuilding
{
    public class EEWorldGenerator
    {
        public List<GenPass> passes = new List<GenPass>();
        public int seed;

        public EEWorldGenerator(int seed) => this.seed = seed;

        public void Append(GenPass pass) => passes.Add(pass);

        public void GenerateWorld(GenerationProgress progress = null)
        {
            progress = progress ?? new GenerationProgress();
            Stopwatch stopwatch = new Stopwatch();
            progress.TotalWeight = 0f;

            foreach (GenPass pass in passes)
                progress.TotalWeight += pass.Weight;

            Main.menuMode = 888;
            Main.MenuUI.SetState(new UIWorldLoad(progress));

            foreach (GenPass pass in passes)
            {
                WorldGen._genRand = new UnifiedRandom(seed);
                Main.rand = new UnifiedRandom(seed);
                stopwatch.Start();
                progress.Start(pass.Weight);

                try
                {
                    pass.Apply(progress);
                }
                catch (Exception e)
                {
                    //string text = string.Join("\n", Language.GetTextValue("tModLoader.WorldGenError"), pass.Name, e);
                    throw e;
                }

                progress.End();
                stopwatch.Reset();
            }
        }
    }
}
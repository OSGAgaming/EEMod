using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;


namespace EEMod.NPCs.CoralReefs
{
    public class OrbCollection : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Orb");
        }

        public int rippleCount = 2;
        public int rippleSize = 13;
        public int rippleSpeed = 200;
        public float distortStrength = 5;
        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            npc.friendly = true;
            npc.HitSound = SoundID.NPCHit25;
            npc.DeathSound = SoundID.NPCDeath28;
            npc.alpha = 20;
            npc.lifeMax = 1000000;
            npc.width = 128;
            npc.height = 130;
            npc.noGravity = true;
            npc.lavaImmune = true;
            npc.noTileCollide = true;
            npc.dontTakeDamage = true;
            npc.damage = 0;
            Main.npcFrameCount[npc.type] = 4;
        }
        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter++;
            if (npc.frameCounter == 6)
            {
                npc.frame.Y = npc.frame.Y + frameHeight;
                npc.frameCounter = 0;
            }
            if (npc.frame.Y >= frameHeight * 3)
            {
                npc.frame.Y = 0;
                return;
            }
        }
        public override bool CheckActive()
        {
            return false;
        }
        bool isPicking;
        bool otherPhase;
        float t;
        Vector2[] Holder = new Vector2[2];
        public override void AI()
        {
            npc.ai[0] += 0.05f;
            if(!otherPhase)
            npc.position.Y += (float)Math.Sin(npc.ai[0]) * 2;
            if (npc.life == 0)
            {
                if (Main.netMode != NetmodeID.Server && Filters.Scene["EEMod:Shockwave"].IsActive())
                {
                    Filters.Scene["EEMod:Shockwave"].Deactivate();
                }
            }
            if(Main.player[(int)npc.ai[1]].GetModPlayer<EEPlayer>().isPickingUp)
            {
                npc.Center = Main.player[(int)npc.ai[1]].Center - new Vector2(0, 80);
                if (Main.player[(int)npc.ai[1]].GetModPlayer<EEPlayer>().isPickingUp)
                {
                    Main.player[(int)npc.ai[1]].bodyFrame.Y = 56 * 5;
                }
            }
            if(isPicking && !Main.player[(int)npc.ai[1]].GetModPlayer<EEPlayer>().isPickingUp)
            {
                otherPhase = true;
                Holder[0] = npc.Center;
                Holder[1] = Main.MouseWorld;
            }
            if(otherPhase)
            {
                t += 0.01f;
                if (t <= 1)
                {
                    Vector2 mid = (Holder[0] + Holder[1]) / 2;
                    npc.Center = Helpers.TraverseBezier(Holder[1], Holder[0], mid - new Vector2(0, 200), mid - new Vector2(0, 200), t);
                    Main.LocalPlayer.GetModPlayer<EEPlayer>().FixateCameraOn(npc.Center, 16f, false, true, 0);
                }
                else if (t <= 2)
                {
                    Main.LocalPlayer.GetModPlayer<EEPlayer>().FixateCameraOn(npc.Center, 16f, true, false, 10);
                }
                else
                {
                    t = 0;
                    otherPhase = false;
                }
            }
            else
            {
                Main.LocalPlayer.GetModPlayer<EEPlayer>().TurnCameraFixationsOff();
            }
            isPicking = Main.player[(int)npc.ai[1]].GetModPlayer<EEPlayer>().isPickingUp;
        }

        public int size = 128;
        public int sizeGrowth;
        public float num88 = 1;
    }
}

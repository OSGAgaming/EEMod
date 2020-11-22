using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EEMod.NPCs.CoralReefs
{
    public class Isopod : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Isopod");
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            //spriteBatch.End();
            //spriteBatch.Begin();
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;

            npc.HitSound = SoundID.NPCHit25;
            npc.DeathSound = SoundID.NPCDeath28;

            npc.alpha = 20;

            npc.lifeMax = 550;
            npc.defense = 10;

            npc.width = 34;
            npc.height = 134;

            npc.noGravity = true;

            npc.lavaImmune = false;
            npc.noTileCollide = false;
            //bannerItem = ModContent.ItemType<Items.Banners.GiantSquidBanner>();
        }

        public override void AI()
        {
            npc.ai[1]++;

            npc.TargetClosest();
            Player target = Main.player[npc.target];
            if (npc.ai[1] % 90 == 0)
            {
                npc.velocity += Vector2.Normalize(target.Center - npc.Center) * 12;
            }
            npc.velocity *= 0.98f;

            npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
        }
    }
}
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EEMod.NPCs.CoralReefs
{
    public class ToxicPuffer : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Toxic Puffer");
        }

        public override void SetDefaults()
        {
            npc.aiStyle = 18;

            npc.HitSound = SoundID.NPCHit25;
            npc.DeathSound = SoundID.NPCDeath28;

            npc.alpha = 20;

            npc.lifeMax = 38;
            npc.defense = 2;

            npc.buffImmune[BuffID.Confused] = true;

            npc.width = 22;
            npc.height = 50;

            npc.noGravity = true;

            npc.lavaImmune = false;
            npc.noTileCollide = false;
        }
    }
}
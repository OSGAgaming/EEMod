using EEMod.Buffs.Buffs;
using EEMod.Projectiles.Summons;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EEMod.Items.Weapons.Summon
{
    public class AncientBubbleStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Bubble Staff");
        }

        public override void SetDefaults()
        {
            item.melee = false;
            item.summon = true;
            item.noMelee = true;
            item.autoReuse = true;
            item.value = Item.sellPrice(0, 0, 18);
            item.damage = 13;
            item.useTime = 26;
            item.useAnimation = 26;
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Green;
            item.knockBack = 5f;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.UseSound = SoundID.Item8;
            item.shoot = ModContent.ProjectileType<AncientBubble>();
        }

        public override bool CanUseItem(Player player)
        {
            return !(Main.LocalPlayer.ownedProjectileCounts[item.shoot] <= 1);
        }
    }
}
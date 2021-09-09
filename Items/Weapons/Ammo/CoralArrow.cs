using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EEMod.Items.Weapons.Ammo
{
    public class CoralArrow : EEItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coral Arrow");
            ItemID.Sets.SortingPriorityMaterials[Item.type] = 59; // influences the inventory sort order. 59 is PlatinumBar, higher is more valuable.
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(0, 0, 18, 0);
            Item.rare = ItemRarityID.Green;
            Item.consumable = true;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 12;
            Item.shoot = ModContent.ProjectileType<CoralArrowProjectile>();
            Item.ammo = AmmoID.Arrow;
        }
    }
}
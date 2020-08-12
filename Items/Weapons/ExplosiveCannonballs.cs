using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EEMod.Items.Weapons
{
    public class ExplosiveCannonballs : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Explosive Cannonballs");
            Tooltip.SetDefault("Use this to make your boat shoot explosive cannonballs.");
            ItemID.Sets.SortingPriorityMaterials[item.type] = 59; // influences the inventory sort order. 59 is PlatinumBar, higher is more valuable.
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 1;
            item.value = Item.buyPrice(0, 0, 18, 0);
            item.rare = ItemRarityID.Green;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.UseSound = SoundID.Item1;
            item.consumable = false;
            item.useTime = 15;
            item.useAnimation = 15;
        }

        public override void OnConsumeItem(Player player)
        {
            EEPlayer modPlayer = player.GetModPlayer<EEPlayer>();
            modPlayer.cannonballType = 2;
            Main.NewText(modPlayer.boatSpeed);
        }

        public override bool UseItem(Player player)
        {
            return true;
        }
    }
}
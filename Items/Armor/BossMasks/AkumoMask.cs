using Terraria.ID;
using Terraria.ModLoader;

namespace EEMod.Items.Armor.BossMasks
{
    [AutoloadEquip(EquipType.Head)]
    public class AkumoMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Akumo Mask");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.rare = ItemRarityID.Blue;
        }
    }
}
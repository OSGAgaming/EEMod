using EEMod.Tiles.Furniture.Paintings;
using Terraria.ID;
using Terraria.ModLoader;

namespace EEMod.Items.Placeables.Paintings
{
    public class GuardianOfTheCoralReefs : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Guardian of the Coral Reefs");
        }

        public override void SetDefaults()
        {
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.maxStack = 99;
            item.consumable = true;
            item.width = 12;
            item.height = 12;

            item.createTile = ModContent.TileType<MoonTile>();
        }
    }
}
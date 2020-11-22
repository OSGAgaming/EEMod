using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace EEMod
{
    public class TileVisualHandler : IComponentHandler<TileObjVisual>
    {
        private List<TileObjVisual> TileVisuals = new List<TileObjVisual>();

        public void Update()
        {
            foreach (TileObjVisual TV in TileVisuals)
            {
                TV.Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (TileObjVisual TV in TileVisuals)
            {
                TV.Draw(spriteBatch);
            }
        }

        public void AddElement(TileObjVisual TV)
        {
            foreach (TileObjVisual TOV in TileVisuals)
            {
                if (TOV.position == TV.position)
                    return;
            }
            TileVisuals.Add(TV);
        }
    }
}
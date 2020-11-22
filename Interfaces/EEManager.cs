using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace EEMod
{
    public class ComponenetManager<T> where T : Entity, IComponent
    {
        private List<T> Objects = new List<T>();

        public void Update()
        {
            foreach (T TV in Objects)
            {
                TV.Update();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (T TV in Objects)
            {
                TV.Draw(spriteBatch);
            }
        }

        public void AddElement(T TV)
        {
            foreach (T TOV in Objects)
            {
                if (TOV.position == TV.position)
                    return;
            }
            Objects.Add(TV);
        }
    }
}
﻿using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EEMod.Extensions;
using EEMod.ID;
using Terraria.ModLoader;

namespace EEMod.Seamap.SeamapContent
{
    public class Island : SeamapObject
    {
        public virtual string name => "Island";
        public virtual int framecount => 1;
        public virtual int framespid => 0;
        public virtual bool cancollide => false;

        public virtual Texture2D islandTex => ModContent.Request<Texture2D>("EEMod/Empty");

        public virtual IslandID id => IslandID.Default;

        public Vector2 posToScreen => position - Main.screenPosition;
        public bool isCollidingWithPlayer => SeamapPlayerShip.localship.rect.Intersects(this.rect);

        public int framecounter;
        public int frame;

        public Island(Vector2 pos): base(pos, Vector2.Zero)
        {
            texture = islandTex;

            width = texture.Width;
            height = texture.Height / framecount;
        }

        public void AnimateIsland()
        {
            if(++framecounter > framespid)
            {
                framecounter = 0;
                if(++frame > framecount-1)
                {
                    frame = 0;
                }
            }
        }

        public override void Update()
        {
            AnimateIsland();
        }

        public override bool CustomDraw(SpriteBatch spriteBatch)
        {
            Vector2 currentPos = position.ForDraw();
            Color drawColour = Helpers.GetLightingColor(position) * Main.LocalPlayer.GetModPlayer<EEPlayer>().seamapLightColor;
            drawColour.A = 255;
            spriteBatch.Draw(texture, position.ForDraw(), new Rectangle(0, texture.Height / framecount * frame, texture.Width, texture.Height / framecount), Color.White);
            return true;
        }

        public virtual void CustomDraw()
        {

        }
    }
}

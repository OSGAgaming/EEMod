using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using System;
using Microsoft.Xna.Framework.Graphics;

namespace EEMod.NPCs.Bosses.Kraken
{
    public class Tentacle : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tentacle");
        }

        public override void SetDefaults()
        {
            npc.width = 184;
            npc.height = 80;
            npc.damage = 0;
            npc.aiStyle = -1;
            npc.lifeMax = 5000;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.knockBackResist = 0f;
        }
        Vector2 startingPosition;
        Vector2 distance;
        bool isGrabbing0;
        bool isGrabbing1;
        bool isRetrating = false;
        bool yeet;
        float distanceCovered = 2000;
        float alpha = 1;
        public override bool CheckActive()
        {
            return false;
        }

        public override void AI()
        {
            npc.ai[0]++;
            npc.TargetClosest(true);
            Rectangle npcHitBox = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
            Rectangle playerHitBox = new Rectangle((int)Main.player[npc.target].position.X, (int)Main.player[npc.target].position.Y, Main.player[npc.target].width, Main.player[npc.target].height);
            if (npc.ai[3] == 0)
            {

                npc.spriteDirection = -1;
                if (isGrabbing0 && !isRetrating)
                {
                    alpha = Math.Abs(distance.X / distanceCovered);
                    (Main.npc[(int)npc.ai[2]].modNPC as KrakenHead).GETHIMBOIS = true;
                    if (!yeet)
                    {
                        Main.npc[(int)npc.ai[2]].ai[0] = 0;
                        yeet = true;
                    }
                    Main.player[npc.target].Center = npc.Center;
                    npc.velocity *= 0.94f;
                    if (npc.life < 1000)
                    {
                        if (!isRetrating)
                        {
                            (Main.npc[(int)npc.ai[2]].modNPC as KrakenHead).Reset(3);
                            isRetrating = true;
                        }
                    }
                    if (distance.X < 0)
                    {
                        npc.life = 0;
                    }
                }
                else
                {
                    if (npc.ai[0] == 1)
                    {
                        startingPosition = npc.Center;
                    }
                    if (distance.X > distanceCovered)
                    {
                        npc.ai[1] = 1;
                    }
                    if (npc.ai[1] == 0)
                    {
                        npc.velocity.X = npc.ai[0] / 5;
                        npc.velocity.Y = (Main.player[npc.target].Center.Y - npc.Center.Y) / 47f;
                    }
                    else
                    {
                        isRetrating = true;
                        alpha = Math.Abs(distance.X / distanceCovered);
                        npc.velocity.X = -5;
                        if (distance.X < 0)
                        {
                            npc.life = 0;
                        }
                    }
                }
                if (Main.npc[(int)npc.ai[2]].ai[0] >= 278)
                {
                    isRetrating = true;
                }
                if (isRetrating)
                {
                    if (distance.X < 0)
                    {
                        npc.life = 0;
                    }
                    alpha = Math.Abs(distance.X / distanceCovered);
                    npc.velocity.X = -5;
                    npc.velocity.Y = (startingPosition.Y - npc.Center.Y) / 100f;
                }
                else
                {
                    npc.velocity.Y += (float)Math.Sin(npc.ai[0] / 10f) * 0.04f;
                }
                distance = (npc.Center - startingPosition);

            }
            if (npc.ai[3] == 1)
            {
                npc.spriteDirection = 1;
                if (isGrabbing1 && !isRetrating)
                {
                    alpha = Math.Abs(distance.X / distanceCovered);
                    (Main.npc[(int)npc.ai[2]].modNPC as KrakenHead).GETHIMBOIS = true;
                    if (!yeet)
                    {
                        Main.npc[(int)npc.ai[2]].ai[0] = 0;
                        yeet = true;
                    }
                    Main.player[npc.target].Center = npc.Center;
                    npc.velocity *= 0.94f;
                    if (npc.life < 1000)
                    {
                        if (!isRetrating)
                        {
                            (Main.npc[(int)npc.ai[2]].modNPC as KrakenHead).Reset(3);
                            isRetrating = true;
                        }
                    }
                    if (distance.X > 0)
                    {
                        npc.life = 0;
                    }
                }
                else
                {
                    if (npc.ai[0] == 1)
                    {
                        startingPosition = npc.Center;
                    }
                    if (distance.X < -distanceCovered)
                    {
                        npc.ai[1] = 1;
                    }
                    if (npc.ai[1] == 0)
                    {
                        npc.velocity.X = -(npc.ai[0] / 5);
                        npc.velocity.Y = (Main.player[npc.target].Center.Y - npc.Center.Y) / 47f;
                    }
                    else
                    {
                        isRetrating = true;
                        alpha = Math.Abs(distance.X / distanceCovered);
                        npc.velocity.X = 5;
                        if (distance.X > 0)
                        {
                            npc.life = 0;
                        }
                    }
                    distance = (npc.Center - startingPosition);
                }
                if (Main.npc[(int)npc.ai[2]].ai[0] >= 278)
                {
                    isRetrating = true;
                }
                if (isRetrating)
                {
                    if (distance.X > 0)
                    {
                        npc.life = 0;
                    }
                    alpha = Math.Abs(distance.X / distanceCovered);
                    npc.velocity.X = 5;
                    npc.velocity.Y = (startingPosition.Y - npc.Center.Y) / 100f;
                }
                else
                {
                    npc.velocity.Y += (float)Math.Sin(npc.ai[0] / 10f) * 0.04f;
                }
                distance = (npc.Center - startingPosition);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (npc.ai[3] == 0)
            {
                isGrabbing0 = true;
                (Main.npc[(int)npc.ai[2]].modNPC as KrakenHead).isRightOrLeft = true;
            }
            if (npc.ai[3] == 1)
            {
                isGrabbing1 = true;
                (Main.npc[(int)npc.ai[2]].modNPC as KrakenHead).isRightOrLeft = false;
            }
        }
        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D texture = TextureCache.TentacleChainSmol;
            Texture2D texture2 = TextureCache.TentacleEnd;
            Helpers.DrawBezier(spriteBatch, texture, "", drawColor * alpha, npc.Center, startingPosition, startingPosition + (npc.Center - startingPosition) * 0.33f + new Vector2((float)Math.Cos(npc.ai[0] / 23f) * 50, (float)Math.Sin(npc.ai[0] / 10f) * 40), startingPosition + (npc.Center - startingPosition) * 0.66f + new Vector2((float)Math.Sin(npc.ai[0] / 20f) * 50, -(float)Math.Cos(npc.ai[0] / 15f) * 55), (npc.width * 0.8f) / distanceCovered, (float)Math.PI / 2, texture2);
            /*  if(npc.ai[3] == 0)
              Main.spriteBatch.Draw(texture, npc.Center + new Vector2(npc.width / 2, 0) - Main.screenPosition - distance / 2 + new Vector2(70,0), new Rectangle(texture.Width - (int)distance.X, 0, (int)distance.X, texture.Height), drawColor, npc.rotation, new Rectangle(texture.Width - (int)distance.X, 0, (int)distance.X, texture.Height).Size() / 2, npc.scale, npc.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
              if (npc.ai[3] == 1)
                  Main.spriteBatch.Draw(texture, npc.Center - new Vector2(npc.width / 2, 0) - Main.screenPosition - distance / 2 + new Vector2(70, 0), new Rectangle(texture.Width + (int)distance.X, 0, -(int)distance.X, texture.Height), drawColor, npc.rotation, new Rectangle(texture.Width + (int)distance.X, 0, -(int)distance.X, texture.Height).Size() / 2, npc.scale, npc.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);*/

        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            return false;
        }
    }
}

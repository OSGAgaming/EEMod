using EEMod.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace EEMod
{
    public class Particle : Entity
    {
        internal float timeLeft;
        private List<IParticleModule> Modules = new List<IParticleModule>();
        private Texture2D texture;
        private int RENDERDISTANCE => 2000;
        public float varScale;
        public float scale { get; set; }
        public float alpha;
        public Color colour;
        public float rotation;
        private int TrailLength;
        public Rectangle Frame;
        public List<Vector2> PositionCache = new List<Vector2>();

        public void UpdatePositionCache()
        {
            PositionCache.Insert(0, position);
            if (PositionCache.Count > TrailLength)
            {
                PositionCache.RemoveAt(PositionCache.Count - 1);
            }
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnDraw()
        {
        }

        public Particle(Vector2 position, int timeLeft, Texture2D texture, Vector2? velocity = null, int scale = 1, Color? colour = null, params IParticleModule[] StartingModule)
        {
            this.timeLeft = timeLeft;
            this.position = position;
            this.velocity = velocity ?? Vector2.Zero;
            this.texture = texture;
            active = true;
            this.scale = scale;
            alpha = 1;
            this.colour = colour ?? Color.White;
            TrailLength = 18;
            Frame = new Rectangle(0, 0, 1, 1);
            SetModules(StartingModule.ToArray() ?? new IParticleModule[0]);
        }

        public void AddModule(IParticleModule Module) => Modules.Add(Module);

        public void SetModules(params IParticleModule[] Module) => Modules = Module.ToList();

        public void Update()
        {
            position += velocity;
            OnUpdate();
            UpdatePositionCache();
            if (timeLeft > 1)
                timeLeft--;
            if (timeLeft == 1)
            {
                varScale *= 0.99f;
                if (varScale < 0.01f)
                {
                    timeLeft--;
                }
            }
            else if (Math.Abs(scale - varScale) > 0.01f && timeLeft != 0)
            {
                varScale += (scale - varScale) / 14f;
            }
            if (timeLeft == 0)
            {
                active = false;
            }

            foreach (IParticleModule Module in Modules)
            {
                Module.Update(this);
            }
        }

        public void Draw()
        {
            foreach (IParticleModule Module in Modules)
            {
                Module.Draw(this);
            }
            Vector2 positionDraw = position.ForDraw();
            Main.spriteBatch.Draw(texture, positionDraw, Frame, colour * alpha, rotation, new Rectangle(0, 0, 1, 1).Size() / 2, varScale, SpriteEffects.None, 0f);
            OnDraw();
        }
    }

    internal class TestModule : IParticleModule
    {
        public void Update(in Particle particle)
        {
            particle.position.X++;
        }

        public void Draw(in Particle particle)
        {
            ;
        }
    }

    internal class SlowDown : IParticleModule
    {
        private float slowDownFactor;

        public SlowDown(float slowDownFactor)
        {
            this.slowDownFactor = slowDownFactor;
        }

        public void Update(in Particle particle)
        {
            particle.velocity *= slowDownFactor;
        }

        public void Draw(in Particle particle)
        {
            ;
        }
    }

    internal class RotateTexture : IParticleModule
    {
        private float rotationSpeed;

        public RotateTexture(float rotationSpeed)
        {
            this.rotationSpeed = rotationSpeed;
        }

        public void Update(in Particle particle)
        {
            particle.rotation += rotationSpeed;
        }

        public void Draw(in Particle particle)
        {
            ;
        }
    }

    internal class SimpleBrownianMotion : IParticleModule
    {
        private float intensity;

        public SimpleBrownianMotion(float intensity)
        {
            this.intensity = intensity;
        }

        public void Update(in Particle particle)
        {
            particle.velocity.X += Main.rand.NextFloat(-1, 1) * intensity;
            particle.velocity.Y += Main.rand.NextFloat(-1, 1) * intensity;
        }

        public void Draw(in Particle particle)
        {
            ;
        }
    }

    internal class AdditiveCircularMotion : IParticleModule
    {
        private float width;
        private float height;
        private float speed;
        private float timer;

        public AdditiveCircularMotion(float width, float height, float speed)
        {
            this.width = width;
            this.height = height;
            this.speed = speed;
        }

        public void Update(in Particle particle)
        {
            timer += speed;
            Vector2 rotVec = new Vector2((float)Math.Sin(timer) * width, (float)Math.Cos(timer) * height);
            particle.position.X += rotVec.X;
            particle.position.Y += rotVec.Y;
        }

        public void Draw(in Particle particle)
        {
            ;
        }
    }

    internal class AfterImageTrail : IParticleModule
    {
        private float alphaFallOff;

        public AfterImageTrail(float alphaFallOff)
        {
            this.alphaFallOff = alphaFallOff;
        }

        public void Draw(in Particle particle)
        {
            for (int i = 0; i < particle.PositionCache.Count; i++)
            {
                float globalFallOff = 1 - (i / (float)(particle.PositionCache.Count - 1)) * alphaFallOff;
                Main.spriteBatch.Draw(Main.magicPixel, particle.PositionCache[i].ForDraw(), new Rectangle(0, 0, 1, 1), particle.colour * particle.alpha * globalFallOff, particle.rotation, new Rectangle(0, 0, 1, 1).Size() / 2, particle.varScale * globalFallOff, SpriteEffects.None, 0f);
            }
        }

        public void Update(in Particle particle)
        {
            ;
        }
    }

    internal class Spew : IParticleModule
    {
        private float randomAngle;
        private float randomSpeed;
        private Vector2 initialSpeed;
        private float airResistance;
        private bool initial;

        public Spew(float randomAngle, float randomSpeed, Vector2 initialSpeed, float airResistance)
        {
            this.randomAngle = randomAngle;
            this.randomSpeed = randomSpeed;
            this.initialSpeed = initialSpeed;
            this.airResistance = airResistance;
        }

        public void Update(in Particle particle)
        {
            if (!initial)
            {
                initial = true;
                float randVel = 1 + Main.rand.NextFloat(-randomSpeed / 2, randomSpeed / 2);
                float randAngle = Main.rand.NextFloat(-randomAngle / 2, randomAngle / 2);
                particle.velocity = new Vector2(initialSpeed.X, initialSpeed.Y).RotatedBy(randAngle) * randVel;
            }
            particle.velocity *= airResistance;
        }

        public void Draw(in Particle particle)
        {
            ;
        }
    }

    internal class AddVelocity : IParticleModule
    {
        private Vector2 velocity;

        public AddVelocity(Vector2 velocity)
        {
            this.velocity = velocity;
        }

        public void Update(in Particle particle)
        {
            particle.velocity += velocity;
        }

        public void Draw(in Particle particle)
        {
            ;
        }
    }

    internal class RotateVelocity : IParticleModule
    {
        private float rotFac;

        public RotateVelocity(float rotFac)
        {
            this.rotFac = rotFac;
        }

        public void Update(in Particle particle)
        {
            particle.velocity = particle.velocity.RotatedBy(rotFac);
        }

        public void Draw(in Particle particle)
        {
            ;
        }
    }

    internal class SetFrame : IParticleModule
    {
        private Rectangle frame;

        public SetFrame(Rectangle bounds)
        {
            frame = bounds;
        }

        public void Update(in Particle particle)
        {
            particle.Frame = frame;
        }

        public void Draw(in Particle particle)
        {
            ;
        }
    }

    internal class CircularMotion : IParticleModule
    {
        private float width;
        private float height;
        private float speed;
        private float timer;
        private Entity orbitPoint;
        private float rotation;

        public CircularMotion(float width, float height, float speed, Entity orbitPoint, float rotation = 0f)
        {
            this.width = width;
            this.height = height;
            this.speed = speed;
            this.orbitPoint = orbitPoint;
            this.rotation = rotation;
        }

        public void Update(in Particle particle)
        {
            timer += speed;
            Vector2 rotVec = new Vector2((float)Math.Sin(timer) * width, (float)Math.Cos(timer) * height).RotatedBy(rotation);
            particle.position.X = orbitPoint.Center.X + rotVec.X;
            particle.position.Y = orbitPoint.Center.Y + rotVec.Y;
        }

        public void Draw(in Particle particle)
        {
            ;
        }
    }

    internal class CircularMotionSin : IParticleModule
    {
        private float width;
        private float height;
        private float speed;
        private float timer;
        private Entity orbitPoint;
        private float rotation;
        private float intensity;
        private float period;
        private bool disapearFromBack;

        public CircularMotionSin(float width, float height, float speed, Entity orbitPoint, float rotation = 0f, float intensity = 0f, float period = 0f, bool disapearFromBack = false)
        {
            this.width = width;
            this.height = height;
            this.speed = speed;
            this.orbitPoint = orbitPoint;
            this.rotation = rotation;
            this.intensity = intensity;
            this.disapearFromBack = disapearFromBack;
        }

        public void Update(in Particle particle)
        {
            timer += speed * (1 + (float)Math.Sin(timer * period) * intensity);
            Vector2 rotVec = new Vector2((float)Math.Sin(timer) * width, (float)Math.Cos(timer) * height).RotatedBy(rotation);
            particle.position.X = orbitPoint.Center.X + rotVec.X;
            particle.position.Y = orbitPoint.Center.Y + rotVec.Y;
            if (disapearFromBack)
            {
                if (timer % (float)Math.PI * 4 < (float)Math.PI)
                {
                    particle.alpha = 0f;
                }
                else
                {
                    particle.alpha = 1f;
                }
            }
        }

        public void Draw(in Particle particle)
        {
            ;
        }
    }

    internal class CircularMotionSinSpin : IParticleModule
    {
        private float width;
        private float height;
        private float speed;
        private float timer;
        private Entity orbitPoint;
        private float rotation;
        private float intensity;
        private float period;
        private float spinSpeed;
        private bool disapearFromBack;
        private float rot;

        public CircularMotionSinSpin(float width, float height, float speed, Entity orbitPoint, float spinSpeed = 0f, float rotation = 0f, float intensity = 0f, float period = 0f, bool disapearFromBack = false)
        {
            this.width = width;
            this.height = height;
            this.speed = speed;
            this.orbitPoint = orbitPoint;
            this.rotation = rotation;
            this.intensity = intensity;
            this.disapearFromBack = disapearFromBack;
            this.spinSpeed = spinSpeed;
        }

        public void Update(in Particle particle)
        {
            timer += speed * (1 + (float)Math.Sin(timer * period) * intensity);
            rot += spinSpeed;
            Vector2 rotVec = new Vector2((float)Math.Sin(timer) * width, (float)Math.Cos(timer) * height).RotatedBy(rotation);
            Vector2 alt = rotVec.RotatedBy(rot);
            particle.position.X = orbitPoint.Center.X + alt.X;
            particle.position.Y = orbitPoint.Center.Y + alt.Y;
            if (disapearFromBack)
            {
                if (timer % (float)Math.PI * 4 < (float)Math.PI)
                {
                    particle.alpha = 0f;
                }
                else
                {
                    particle.alpha = 1f;
                }
            }
        }

        public void Draw(in Particle particle)
        {
            ;
        }
    }

    internal class ZigzagMotion : IParticleModule
    {
        private float interval;
        private float maxDistance;
        private int timer;

        public ZigzagMotion(float interval, float maxDistance)
        {
            this.interval = interval;
            this.maxDistance = maxDistance;
        }

        public void Update(in Particle particle)
        {
            timer++;
            if (timer > interval)
            {
                particle.velocity = particle.velocity.RotatedBy(Main.rand.NextFloat(-maxDistance, maxDistance));
                timer = 0;
            }
        }

        public void Draw(in Particle particle)
        {
            ;
        }
    }

    internal class BaseModule : IParticleModule
    {
        public void Update(in Particle particle)
        {
            ;
        }

        public void Draw(in Particle particle)
        {
            ;
        }
    }

    public interface IParticleModule
    {
        void Update(in Particle particle);

        void Draw(in Particle particle);
    }
}
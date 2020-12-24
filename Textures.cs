using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EEMod.Autoloading;
using EEMod.Autoloading.AutoloadTypes;
using EEMod.Extensions;

namespace EEMod
{
    public static class Textures
    {
        public static event Action OnUnload;
        [FieldInit]
        internal static Dictionary<Type, List<Texture2D>> _textures;
        [TypeListener]
        private static void TypeListen(Type type)
        {
            string Sanitize(string path) => path?.Replace('.', '/');
            string Process(string path, TexturePath pathtype, Type t)
            {
                switch (pathtype)
                {
                    case TexturePath.Custom:
                        return path;
                    case TexturePath.Namespace:
                        return Sanitize(t.Namespace + "/" + path);
                    case TexturePath.NamespaceAndType:
                        return Sanitize(t.FullName);
                    default:
                        return path;
                }
            }
            var attributes = type.GetCustomAttributes<WithTextureAttribute>().ToList();
            if (attributes.Count > 0)
                AddTextures(type, attributes.OrderBy(attribute=>attribute. Index).Select(attribute => ModContent.GetTexture(Process(Sanitize(attribute.Texture), attribute.PathType, type))));
        }
        public static int AddTexture<T>(Texture2D texture) => AddTexture(typeof(T), texture);
        public static int AddTexture(Type fortype, Texture2D texture)
        {
            if(_textures.TryGetValue(fortype, out var textures))
            {
                int s = textures.Count;
                textures.Add(texture);
                return s;
            }
            else
            {
                _textures[fortype] = new List<Texture2D>(new Texture2D[] { texture });
                return 0;
            }
        }
        public static int AddTextures<T>(params Texture2D[] textures) => AddTextures(typeof(T), (IEnumerable<Texture2D>)textures);
        public static int AddTextures<T>(IEnumerable<Texture2D> textures) => AddTextures(typeof(T), textures);
        public static int AddTextures(Type fortype, params Texture2D[] textures) => AddTextures(fortype, (IEnumerable<Texture2D>)textures);
        public static int AddTextures(Type fortype, IEnumerable<Texture2D> textures)
        {
            if(_textures.TryGetValue(fortype, out var t))
            {
                int s = t.Count;
                t.AddRange(textures);
                return s;
            }
            else
            {
                _textures[fortype] = new List<Texture2D>(textures);
                return 0;
            }
        }
        [UnloadingMethod]
        private static void Unload()
        {
            OnUnload?.Invoke();
            foreach(var d in _textures)
            {
                if (d.Key != null && d.Key.IsAbstract != d.Key.IsSealed) // no static
                    typeof(Textures<>).MakeGenericType(d.Key).GetMethod("Unload", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, null);
                d.Value?.Clear();
            }
            _textures?.Clear();
            _textures = null;
        }
    }
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = true, Inherited = false)]
    public class WithTextureAttribute : Attribute
    {
        public string Texture { get; }
        public TexturePath PathType { get; }
        public int Index { get; }
        public WithTextureAttribute() : this(null, TexturePath.NamespaceAndType) { }
        public WithTextureAttribute(string texture, TexturePath pathType = TexturePath.Namespace, int index = 0)
        {
            Texture = texture;
            PathType = pathType;
            Index = index;
        }
    }
    public enum TexturePath
    {
        NamespaceAndType,
        Namespace,
        Custom
    }
    internal static class Textures<T>
    {
        private static IList<Texture2D> textures;
        private static IList<Texture2D> Texturesd
        {
            get
            {
                if (textures is null)
                {
                    textures = Textures._textures.TryGetValue(typeof(T), out var texturess) ? texturess : (Textures._textures[typeof(T)] = new List<Texture2D>());
                    Textures.OnUnload += Unload;
                }
                return textures;
            }
        }
        public static Texture2D Texture => Texturesd[0];
        public static Texture2D Texture2 => Texturesd[1];
        public static Texture2D Texture3 => Texturesd[2];
        public static Texture2D Texture4 => Texturesd[3];
        private static void Unload()
        {
            textures?.Clear();
            textures = null;
        }
    }
}

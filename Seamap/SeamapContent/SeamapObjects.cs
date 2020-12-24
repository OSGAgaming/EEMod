using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using EEMod.Autoloading;
using EEMod.Extensions;

namespace EEMod.Seamap.SeamapContent
{
    internal static class SeamapObjects
    {
        [FieldInit] public static List<Island> SeaObject = new List<Island>(); //List 1
        [FieldInit] public static List<int> SeaObjectFrames = new List<int>();
        [FieldInit] public static Dictionary<string, Island> Islands = new Dictionary<string, Island>(); //List 3
        [FieldInit] internal static List<ISeamapEntity> OceanMapElements = new List<ISeamapEntity>(); //List 4
    }
}

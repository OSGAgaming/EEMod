﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace EEMod
{
    public abstract class EENPC : ModNPC
    {
        public NPC NPC => base.NPC; // for 1.4 port
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSB
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await Discord.DiscordHandler.Init();
        }
    }
}

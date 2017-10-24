using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace TNTStewardProgram.Modules
{
    public class TestModule : ModuleBase
    {
        [Command("test")]
        public async Task TestCommand(String repeat)
        {
            await Context.Channel.SendMessageAsync(repeat);
        }

        /*
        [Command("welcome")]
        public async Task SetWelcomeChannel()
        {
            await
        }
        */
    }
}

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
        private CommandService _service;

        public TestModule(CommandService service)           /* Create a constructor for the commandservice dependency */
        {
            _service = service;
        }

        [Command("test")]
        [Remarks("Test command")]
        public async Task TestCommand(String repeat)
        {
            await Context.Channel.SendMessageAsync(repeat);
        }


        //comando help sin parametros
        [Command("help")]
        [Alias("Help", "HELP", "hELP")]
        [Remarks("Muestra una lista de todos los comandos disponibles por modulo")]
        public async Task HelpAsync()
        {
            string prefix = "tnt.";  /* Prefijo de los comandos */
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = "Los comandos que puedes usar son los siguientes:"
            };

            foreach (var module in _service.Modules) /* iteramos por los modulos */
            {
                string description = null;
                foreach (var cmd in module.Commands) /* iteramos por los comandos del modulo */
                {
                    var result = await cmd.CheckPreconditionsAsync(Context); /* comprobacion */
                    if (result.IsSuccess)
                        description += $"{prefix}{cmd.Name}\n"; /* si pasa la comprobacion, añadimos el prefijo y el nombre del comando a la descripcion del embed */
                }

                if (!string.IsNullOrWhiteSpace(description)) /* Si el modulo no estaba vacio, añadimos un campo en que vertir la descripcion del embed */
                {
                    builder.AddField(x =>
                    {
                        x.Name = module.Name;
                        x.Value = description;
                        x.IsInline = false;
                    });
                }
            }

            await Context.User.SendMessageAsync("", false, builder.Build()); /* Le enviamos el embed al usuario */
        }

        //comando help con un comando como parametro
        [Command("help")]
        [Remarks("Shows what a specific command does and what parameters it takes.")]
        public async Task HelpAsync(string command)
        {
            var result = _service.Search(Context, command);

            if (!result.IsSuccess)
            {
                await ReplyAsync($"Lo siento, no puedo encontrar el comando **{command}**.");
                return;
            }

            string prefix = "tnt.";
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = $"Aquí puede ver como usar el comando **{command}**"
            };
            int count = 0;
            foreach (var match in result.Commands)
            {
                if (count == 0)
                {
                    var cmd = match.Command;

                    builder.AddField(x =>
                    {
                        x.Name = prefix + match.Alias;
                        x.Value = $"Descripción: {cmd.Remarks}";
                        x.IsInline = false;
                    });
                    count++;
                }
                
            }
            await Context.User.SendMessageAsync("", false, builder.Build());
        }

        [Command("clear")]
        [Alias("Clear", "CLEAR", "cLEAR")]
        [Remarks("Elimina el numero de mensajes introducido a continuacion del comando (clear número). Si no se introduce ningún número, se eliminará el último mensage del canal.")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Clear(int delete)
        {
            IGuildUser Bot = await Context.Guild.GetUserAsync(Context.Client.CurrentUser.Id);
            if (!Bot.GetPermissions(Context.Channel as ITextChannel).ManageMessages)
            {
                await Context.User.SendMessageAsync("El bot no tiene los permisos suficientes para eliminar mensages.");
                return;
            }
           
            var GuildUser = await Context.Guild.GetUserAsync(Context.User.Id);
            if (!GuildUser.GetPermissions(Context.Channel as ITextChannel).ManageMessages)
            {
                await Context.User.SendMessageAsync("No tienes los permisos suficientes para eliminar mensages");
                return;
            }

            if (delete <= 100)
            {
                var messagesToDelete = await Context.Channel.GetMessagesAsync(delete + 1).Flatten();
                await Context.Channel.DeleteMessagesAsync(messagesToDelete);
                if (delete == 1)
                {
                    await Context.Channel.SendMessageAsync($"`{Context.User.Username} ha eliminado 1 mensage`");

                }
                else
                {
                    await Context.Channel.SendMessageAsync($"`{Context.User.Username} ha eliminado {delete} mensages");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("Lo siento, no puedes eliminar más de 100 mensages.");
            }
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

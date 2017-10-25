using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace TNTStewardProgram.Modules
{
    public class TestModule : ModuleBase
    {
        private CommandService _service;

        private EzEmbed ezEmbed = new EzEmbed();

        public TestModule(CommandService service)           /* Create a constructor for the commandservice dependency */
        {
            _service = service;
        }

        [Command("test")]
        [Name("test <texto a repetir>")]
        [Remarks("Responde con el mismo texto que se le envia")]
        public async Task TestCommand([Remainder]String repeat)
        {
            await Context.Channel.SendMessageAsync(repeat);
        }


        //comando help sin parametros
        [Command("help")]
        [Name("help")]
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
                        description += $"{prefix}{cmd.Name}\n  - {cmd.Remarks}\n\n"; /* si pasa la comprobacion, añadimos el prefijo y el nombre del comando a la descripcion del embed */
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
        [Name("help <comando>")]
        [Remarks("Muestra la descripción específica del comando introducido.")]
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
                        x.Name = prefix + match.Command.Name;
                        x.Value = $"Descripción: {cmd.Remarks}";
                        x.IsInline = false;
                    });
                    count++;
                }

            }
            await Context.User.SendMessageAsync("", false, builder.Build());
        }


        //Comando clear para eliminar n mensages
        [Command("clear")]
        [Name("clear <número de mensages>")]
        [Alias("Clear", "CLEAR", "cLEAR")]
        [Remarks("Elimina el numero de mensajes introducido. Si no se introduce ningún número, se eliminará el último mensage del canal.")]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Clear(int delete = 0)
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


        //Comando ban para banear al usuario mencionado
        [Command("ban")]
        [Name("ban <@usuario>")]
        [Alias("Ban", "BAN")]
        [Summary("ban @Username")]
        [Remarks("Comando para banear al usuario mencionado. Requiere mencionar a un usuario y a continuación escribir la razón del baneo.")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        public async Task BanAsync(SocketGuildUser user = null, [Remainder] string reason = null)
        {
            if (user == null) throw new ArgumentException("Debes mencionar al usuario a banear.");
            if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Debes escribir una razón para banear al usuario.");

            var gld = Context.Guild as SocketGuild;
            var embed = new EmbedBuilder(); //Inicializa el embed builder
            embed.WithColor(new Color(0x4900ff)); //Color del embed
            embed.Title = $"{user.Username} ha sido baneado."; //A quién se ha baneado (titular del mensage)
            embed.Description = $"Nombre de usuario: {user.Username}\nServidor: {user.Guild.Name}\nBaneado por: {Context.User.Mention}\nRazón: {reason}"; //Valores del embed

            await gld.AddBanAsync(user); //baneo al usuario
            await Context.Channel.SendMessageAsync("", false, embed); //envio del mensage embed
        }


        //Comando kick para echar al usuario mencionado
        [Command("kick")]
        [Name("kick <@usuario>")]
        [Alias("Kick", "KICK")]
        [Remarks("Comando para echar al usuario mencionado. Requiere mencionar a un usuario y a continuación escribir la razón del kick.")]
        [RequireBotPermission(GuildPermission.KickMembers)] ///Needed BotPerms///
        [RequireUserPermission(GuildPermission.KickMembers)] ///Needed User Perms///
        public async Task KickAsync(SocketGuildUser user, [Remainder] string reason)
        {
            if (user == null) throw new ArgumentException("Debes mencionar al usuario a echar.");
            if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Debes escribir una razón para echar al usuario.");

            var gld = Context.Guild as SocketGuild;
            var embed = new EmbedBuilder(); //Inicializa el embed builder
            embed.WithColor(new Color(0x4900ff)); //Color del embed
            embed.Title = $" {user.Username} has been kicked from {user.Guild.Name}"; //A quién se ha echado (titular del mensage)
            embed.Description = $"**Username: **{user.Username}\n**Guild Name: **{user.Guild.Name}\n**Kicked by: **{Context.User.Mention}!\n**Reason: **{reason}"; //Valores del embed

            await user.KickAsync(); //kick al usuario
            await Context.Channel.SendMessageAsync("", false, embed); //envio del mensage embed
        }

        /*
        //Comando para ver los colores disponibles para ezembed
        [Command("embedcolors")]
        [Alias("Embedcolors", "embedColors", "EmbedColors", "EMBEDCOLORS")]
        [Remarks("Comando para recibir una lista de los colores disponibles para el comando embed y embedfooter.")]
        public async Task ListEmbedColors()
        {
            string colores = "";
            foreach (string m in ezEmbed.colores)
            {
                colores += m;
                colores += "\n";
            }
            ezEmbed.CreateBasicEmbed("aqua", "Los colores disponibles para el comando embed són:", colores);
            await ezEmbed.SendEmbed(Context.User);
        }


        //Comando para crear y enviar un mensage embed simple
        [Command("embed")]
        [Alias("Embed", "EMBED")]
        [Remarks("Comando para enviar un mensage embed.")]
        public async Task SendEmbedMsg()
        {

        }*/

        //Comando infobot para recibir la informacion referente al bot
        [Command("infobot")]
        [Name("infobot")]
        [Alias("Infobot", "INFOBOT", "InfoUser")]
        [Remarks("Muestra la información del bot.")]
        public async Task Info()
        {
            var embed = new EmbedBuilder();
            var application = await Context.Client.GetApplicationInfoAsync(); //Version de la aplicacion 
            embed.ImageUrl = application.IconUrl;  //Icono del bot
            embed.WithColor(new Color(0x4900ff)) //Color del embed
                .AddField(y =>  //Nuevo campo
                {
                    y.Name = "Propietario";  //Titulo del campo
                    y.Value = application.Owner.Username; application.Owner.Id.ToString();  //Nombre del propietario
                    y.IsInline = false;
                })
                .AddField(y =>
                {
                    y.Name = "Versión de Discord.net";  //Titulo del campo
                    y.Value = DiscordConfig.Version;  //Version de las librerias discord del bot
                    y.IsInline = true;
                })
                .AddField(y =>
                {
                    y.Name = "Servidores";
                    string servers = "";
                    foreach (IGuild guild in (Context.Client as DiscordSocketClient).Guilds)
                    {
                        servers += guild.ToString() + "\n  ";
                    }
                    y.Value = "TNT-Steward está en " + (Context.Client as DiscordSocketClient).Guilds.Count.ToString() + " servidores. \nLos servidores son:\n  " + servers;
                    y.IsInline = false;
                })
                .AddField(y =>
                {
                    y.Name = "Cantidad de usuarios";
                    int numero = 0;
                    foreach (IGuild guild in (Context.Client as DiscordSocketClient).Guilds)
                    {
                        IReadOnlyCollection<IGuildUser> users = guild.GetUsersAsync().Result;
                        foreach (IGuildUser user in users)
                        {
                            numero++;
                        }
                    }
                    y.Value = numero.ToString();
                    y.IsInline = false;
                })
                .AddField(y =>
                {
                    y.Name = "Cantidad de canales";
                    int numero = 0;
                    foreach (IGuild guild in (Context.Client as DiscordSocketClient).Guilds)
                    {
                        IReadOnlyCollection<IGuildChannel> users = guild.GetChannelsAsync().Result;
                        foreach (IGuildChannel user in users)
                        {
                            numero++;
                        }
                    }
                    y.Value = numero.ToString();
                    y.IsInline = false;
                });
            await this.ReplyAsync("", embed: embed);
        }

        //Comando infouser para recibir informacion de un usuario
        [Command("infouser")]
        [Name("infouser <@usuario>")]
        [Alias("Infouser", "INFOUSER", "InfoUser", "userinfo", "uinfo", "USERINFO", "UserInfo", "Userinfo", "UINFO", "usrinfo", "infousr")]
        [Remarks("Muestra la informacion del usuario mencionado.")]
        public async Task UserInfo(IGuildUser user)
        {        
                var application = await Context.Client.GetApplicationInfoAsync();
                var thumbnailurl = user.GetAvatarUrl();
                var date = $"{user.CreatedAt.Month}/{user.CreatedAt.Day}/{user.CreatedAt.Year}";
                var auth = new EmbedAuthorBuilder()
                {
                    Name = user.Username,
                    IconUrl = thumbnailurl,
                };
                var embed = new EmbedBuilder()

                {
                    Color = new Color(29, 140, 209),
                    Author = auth
                };
                var us = user as SocketGuildUser;

                var D = us.Username;

                var A = us.Discriminator;
                var T = us.Id;
                var S = date;
                var C = us.Status;
                var CC = us.JoinedAt;
                var O = us.Game;
                embed.Title = $"Información de usuario de {D}";
                embed.Description = $"Nombre de usuario: **{D}**\nDiscriminator: **{A}**\nID de usuario: **{T}**\nFecha de creación: **{S}**\nEstado: **{C}**\nFecha de entrada al servidor: **{CC}**\nJugando: **{O}**";
                await ReplyAsync("", false, embed.Build());
        }

        //Comando serverinfo para recibir informacion del servidor
        [Command("infoserver")]
        [Name("infoserver")]
        [Alias("Infoserver", "INFOSERVER", "InfoServer", "serverinfo", "sinfo", "SERVERINFO", "ServerInfo", "Serverinfo", "SINFO", "servinfo", "infoserv")]
        [Remarks("Muestra la informacion del servidor.")]
        public async Task GuildInfo()
        {
            EmbedBuilder embedBuilder;
            embedBuilder = new EmbedBuilder();
            embedBuilder.WithColor(new Color(0, 71, 171));

            var gld = Context.Guild as SocketGuild;
            var client = Context.Client as DiscordSocketClient;

            if (!string.IsNullOrWhiteSpace(gld.IconUrl))  //Icono del server.
                embedBuilder.ThumbnailUrl = gld.IconUrl;
            var O = gld.Owner.Username; //Username del propietario del server.

            var V = gld.VoiceRegionId; //Región del server.
            var C = gld.CreatedAt; //Fecha de creación del server.
            var N = gld.DefaultMessageNotifications; //Notificaciones predeterminadas del server.
            var R = gld.Roles; //Roles del server.
            var VL = gld.VerificationLevel; //Nivel de verificacion del server.
            var XD = "";
            foreach (IRole role in R)
            {
                XD += "  " + role.Name.ToString() + "\n";
            }
            var X = gld.MemberCount; //Cantidad de usuarios del server
            var Z = client.ConnectionState; //Estado de la conexión

            embedBuilder.Title = $"{gld.Name}: Información del Servidor"; //Titulo del embed
            embedBuilder.Description = $"Propietario: **{O}**\nRegión: **{V}**\nCreado: **{C}**\nNotificaciones: **{N}**\nVerificación: **{VL}**\nRoles: \n**{XD}**Members: **{X}**\nConntection state: **{Z}**\n\n"; //Mensage embed
            await ReplyAsync("", false, embedBuilder); //Envio del embed
        }

        /*
        [Command("welcome")]
        [Name("welcome")]
        [RequireUserPermission(GuildPermission.ManageChannels)]
        [Remarks("Establece el canal de mensages de bienvenida.")]
        public async Task SetWelcomeChannel()
        {
            //TODO: actualizar _welcomeChannel en CommandHandler
            await Context.Channel.SendMessageAsync("Canal " + Context.Channel.Name.ToString() + " es ahora el canal de bienvenida.");
        }
        
        [Command("wnotify")]
        [Name("wnotify")]
        [RequireUserPermission(GuildPermission.ManageChannels)]
        [Remarks("Establece el canal de notificación de entrada y salida de usuarios (para administradores).")]
        public async Task SetWNotifyChannel()
        {
            //TODO: actualizar _wNotifyChannel en CommandHandler
            await Context.Channel.SendMessageAsync("Canal " + Context.Channel.Name.ToString() + " es ahora el canal de notificación de entrada y salida de usuarios.");
        }
        */
    }
}


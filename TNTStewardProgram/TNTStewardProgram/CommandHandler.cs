using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
using Discord;
using Discord.Commands;
using System.Reflection;
using System.Threading.Tasks;

namespace TNTStewardProgram
{
    public class CommandHandler
    {
        private DiscordSocketClient _client;

        private CommandService _service;

        //En welcomeChannel se daran mensages de bienvenida a los nuevos usuarios y se informara de manera informal del abandono de servidor de usuarios
        private SocketTextChannel _welcomeChannel;

        //En wNotifyChannel se notificara de manera formal la entrada y salida de usuarios del servidor
        private SocketTextChannel _wNotifyChannel;

        public CommandHandler(DiscordSocketClient client)
        {
            _client = client;

            _service = new CommandService();

            _service.AddModulesAsync(Assembly.GetEntryAssembly());

            //Evento de mensaje recibido (sea por mensage privado o por un canal en el que esta el bot) 
            _client.MessageReceived += HandleCommandAsync;

            //Evento de bot listo (se activa cuando se lanza la app y el server de Discord valida la conexion)
            _client.Ready += BotSetup;

            //Evento de nuevo usuario en el server
            _client.UserJoined += AnnounceUserJoined;

            //Evento de salida de usuario en el server
            _client.UserLeft += AnnounceUserLeft;
        }


        //Task que controla lo que hace el bot al leer un mensage
        private async Task HandleCommandAsync(SocketMessage smsg)
        {
            //msg solo tendra contenido si el bot lee el mensage de un usuario real (si es un mensage de otro bot msg sera null y hay que controlarlo)
            var msg = smsg as SocketUserMessage;
            if (msg == null) return;

            //variable de contexto del mensaje
            var context = new SocketCommandContext(_client, msg);

            //comprobamos si el mensage comienza con el prefijo "tnt.", eso determina un comando y se ejecuta la Task result que buscara el comando adecuado en los modulos y ejecutara su Task
            int argPos = 0;
            if (msg.HasStringPrefix("tnt.", ref argPos))
            {
                var result = await _service.ExecuteAsync(context, argPos);

                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }
            }
        }

        //Task que hace que el "juego al que esta jugando el bot" sea "TNT-Europe Administration"
        private async Task BotSetup()
        {
            await _client.SetGameAsync("TNT-Europe Administration");

            _welcomeChannel = _client.GetChannel(372398584844517379) as SocketTextChannel;

            _wNotifyChannel = _client.GetChannel(372462368153927682) as SocketTextChannel;
        }

        
        private async Task AnnounceUserJoined(SocketGuildUser user)
        {
            if (_welcomeChannel != null) await _welcomeChannel.SendMessageAsync(user.Mention + " ~ Bienvenido a TNT-Europe! Esperamos que lo pases genial, camarada " + user.Username + "!");
            if (_wNotifyChannel != null) await _wNotifyChannel.SendMessageAsync("Nuevo miembro en TNT-Europe: " + user.Mention);

            foreach (IRole role in user.Guild.Roles)
            {
                if (role.Name == "DEFAULT")
                {
                    await user.AddRoleAsync(role);
                }
            }            
        }


        private async Task AnnounceUserLeft(SocketGuildUser user)
        {
            if (_welcomeChannel != null) await _welcomeChannel.SendMessageAsync(" ~ " + user.Username + " ha abandonado el servidor. Esperemos que le vaya bien en bronce.");
            if (_wNotifyChannel != null) await _wNotifyChannel.SendMessageAsync("Miembro ha abandonado TNT-Europe: " + user.Mention);
        }






        //Metodos

        //Setter de welcomeChannel
        public void SetWelcomeChannel(SocketTextChannel channel)
        {
            _welcomeChannel = channel;
        }

        //Setter de wNotifyChannel
        public void SetWNotifyChannel(SocketTextChannel channel)
        {
            _wNotifyChannel = channel;
        }
    }
}

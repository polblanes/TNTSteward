using System;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace TNTStewardProgram
{
    class Program
    {
        static void Main(string[] args)
        => new Program().StartAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _client;

        private CommandHandler _handler;

        public async Task StartAsync()
        {
            _client = new DiscordSocketClient();

            _handler = new CommandHandler(_client);

            await _client.LoginAsync(TokenType.Bot, "MzcyMzkzNzA1NzE1NDAwNzA2.DNDrSw.uJc8wqUX5AJFNAGqSWWx0L6by7A");

            await _client.StartAsync();

            await Task.Delay(-1);
        }
    }
}

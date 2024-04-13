using DSharpPlus;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.Configuration;
using DSharpPlus.SlashCommands;
class Program
{
    static async Task Main(string[] args)
    {
        DiscordClient discord = new DiscordClient(new DiscordConfiguration()
        {
            Token = GetApiKey(),
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildVoiceStates
        });

        await discord.ConnectAsync();

        SlashCommandsExtension slash = discord.UseSlashCommands();

        discord.UseVoiceNext();

        slash.RegisterCommands<SlashCommands>();

        await Task.Delay(-1);
    }

    static string? GetApiKey()
    {
        var builder = new ConfigurationBuilder();
        builder.AddUserSecrets<Program>();
        var configuration = builder.Build();
        return configuration["Authentication:Discord:ApiKey"];
    }

}
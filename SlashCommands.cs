using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;

class SlashCommands : ApplicationCommandModule
{
    [SlashCommand("ping", "Replies with pong!")]
    public async Task Ping(InteractionContext ctx)
    {
        Console.WriteLine("Ping command received");
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Pong!"));
    }

    [SlashCommand("join", "Joins the voice channel")]
    public async Task Join(InteractionContext ctx, [Option("video", "video to play")] string video)
    {
        Console.WriteLine("Ping command received");
        VoiceNextExtension vnext = ctx.Client.GetVoiceNext();
        VoiceNextConnection vnc = vnext.GetConnection(ctx.Guild);

        if (vnc != null)
        {
            Console.WriteLine("Already connected in this guild.");
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("Already connected in this guild."));
            return;
        }

        DiscordVoiceState? vstat = ctx.Member?.VoiceState;
        if (vstat?.Channel == null)
        {
            Console.WriteLine("You are not in a voice channel.");
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("You are not in a voice channel."));
            return;
        }

        vnc = await vnext.ConnectAsync(vstat.Channel);
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"Connected to {vstat.Channel.Name}"));

        Console.WriteLine("Connected to voice channel");

        // Cool song
        string url = video;

        YoutubeService yt = new YoutubeService();

        Stream output = yt.GetVideo(url);

        VoiceTransmitSink transmit = vnc.GetTransmitSink();

        await output.CopyToAsync(transmit);
    }
}
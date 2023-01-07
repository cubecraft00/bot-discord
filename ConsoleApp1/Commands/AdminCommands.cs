using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Emzi0767;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Core.Bindings;
using MongoDB.Driver.Core.WireProtocol.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml.Linq;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConsoleApp1.Commands
{
    [SlashCommandGroup("admin", "command reserved for administrator", false)]
    public class AdminCommands : ApplicationCommandModule
    {
        [SlashCommand("ban", "ban user from the guild")]
        public async Task BanCommand(
            InteractionContext ctx)
        {
            // variables
            var members = await ctx.Guild.GetAllMembersAsync();
            var selectMenuOptions = new List<DiscordSelectComponentOption>();

            // insert and set data in select menu
            foreach (var i in members)
            {
                selectMenuOptions.Add(new DiscordSelectComponentOption(i.Username, i.Id.ToString()));
            }
            var selectMenu = new DiscordSelectComponent("memberBan", null, selectMenuOptions);

            // create embed
            var embed = new DiscordEmbedBuilder()
            {
                Title = "Ban member",
                Description = "choisie le membre a ban",
                Color = DiscordColor.Red,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail(){ Url = "https://cdn3.emoji.gg/emojis/5695_staffbot.png" }
            };
            // create message
            var messageBuilder = new DiscordInteractionResponseBuilder()
                .AddEmbed(embed)
                .AddComponents(selectMenu)
                .AsEphemeral();

            // send message
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, messageBuilder);

        }

        [SlashCommand("unban", "ban user from the guild")]
        public async Task UnbanCommand(
            InteractionContext ctx
        )
        {
            // variables
            var selectMenuOptions = new List<DiscordSelectComponentOption>();

            // connect to database
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("botdiscord");
            var collection = database.GetCollection<BsonDocument>("ban");

            // search data
            var data = await collection.Find(new BsonDocument()).ToListAsync();

            // insert and set data in select menu
            foreach (var i in data)
            {
                selectMenuOptions.Add(new DiscordSelectComponentOption(i.GetElement("username").Value.ToString(), i.GetElement("uuid").Value.ToString(), i.GetElement("reason").Value.ToString()));
            }
            var selectMenu = new DiscordSelectComponent("memberUnban", null, selectMenuOptions);

            // create embed
            var embed = new DiscordEmbedBuilder()
            {
                Title = "Ban member",
                Description = "choisie le membre a ban",
                Color = DiscordColor.Green,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail() { Url = "https://cdn3.emoji.gg/emojis/5695_staffbot.png" }
            };
            // create message
            var messageBuilder = new DiscordInteractionResponseBuilder()
                .AddEmbed(embed)
                .AddComponents(selectMenu)
                .AsEphemeral();

            // send message
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, messageBuilder);
        }

        [SlashCommand("clear", "clear message")]
        public async Task ClearCommand(
            InteractionContext ctx,
            [Option("nombre", "nombre de message a suprimer")] string number
        )
        {
            // create message
            var messageBuilder1 = new DiscordInteractionResponseBuilder()
                .WithContent("en cours...")
                .AsEphemeral();

            // send message
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, messageBuilder1);

            // insert and set data in select menu
            foreach (var i in ctx.Channel.GetMessagesAsync(Int32.Parse(number)).GetAwaiter().GetResult())
            {
                await i.DeleteAsync();
                await Task.Delay(100);
            }

            // create embed
            var embed = new DiscordEmbedBuilder()
            {
                Title = "Messages cleared",
                Description = number + " messages ont etaient suprimer",
                Color = DiscordColor.Black,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail() { Url = "https://cdn3.emoji.gg/emojis/5695_staffbot.png" }
            };

            // create message
            var messageBuilder2 = new DiscordWebhookBuilder()
                .AddEmbed(embed);

            // send message
            await ctx.EditResponseAsync(messageBuilder2);
        }

        [SlashCommand("kick", "kick user from the guild")]
        public async Task KickCommand(
            InteractionContext ctx)
        {
            // variables
            var members = await ctx.Guild.GetAllMembersAsync();
            var selectMenuOptions = new List<DiscordSelectComponentOption>();

            // insert and set data in select menu
            foreach (var i in members)
            {
                selectMenuOptions.Add(new DiscordSelectComponentOption(i.Username, i.Id.ToString()));
            }
            var selectMenu = new DiscordSelectComponent("memberKick", null, selectMenuOptions);

            // create embed
            var embed = new DiscordEmbedBuilder()
            {
                Title = "Kick member",
                Description = "choisie le membre a kick",
                Color = DiscordColor.Red,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail() { Url = "https://cdn3.emoji.gg/emojis/5695_staffbot.png" }
            };
            // create message
            var messageBuilder = new DiscordInteractionResponseBuilder()
                .AddEmbed(embed)
                .AddComponents(selectMenu)
                .AsEphemeral();

            // send message
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, messageBuilder);
        }

        [SlashCommand("warn", "warn user from the guild")]
        public async Task WarnCommand(
            InteractionContext ctx)
        {
            // variables
            var members = await ctx.Guild.GetAllMembersAsync();
            var selectMenuOptions = new List<DiscordSelectComponentOption>();

            // insert and set data in select menu
            foreach (var i in members)
            {
                selectMenuOptions.Add(new DiscordSelectComponentOption(i.Username, i.Id.ToString()));
            }
            var selectMenu = new DiscordSelectComponent("memberWarn", null, selectMenuOptions);

            // create embed
            var embed = new DiscordEmbedBuilder()
            {
                Title = "Warn member",
                Description = "choisie le membre a warn",
                Color = DiscordColor.Red,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail() { Url = "https://cdn3.emoji.gg/emojis/5695_staffbot.png" }
            };
            // create message
            var messageBuilder = new DiscordInteractionResponseBuilder()
                .AddEmbed(embed)
                .AddComponents(selectMenu)
                .AsEphemeral();

            // send message
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, messageBuilder);

        }
    }
}

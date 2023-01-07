using DSharpPlus;
using DSharpPlus.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZstdSharp.Unsafe;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ConsoleApp1.Interactions
{
    internal class AdminInteractions
    {
        public async Task MemberBan(DiscordClient sender, DSharpPlus.EventArgs.InteractionCreateEventArgs e)
        {
            // variable
            var response = ulong.Parse(e.Interaction.Data.Values.GetValue(0).ToString());
            var member = await e.Interaction.Guild.GetMemberAsync(response);

            // create message
            var builder = new DiscordInteractionResponseBuilder()
                .WithTitle("Ban " + member.Username)
                .WithCustomId("modalBan")
                .AddComponents(new TextInputComponent("Id (ne pas modifier)", "inputIdBan", null, member.Id.ToString(), true))
                .AddComponents(new TextInputComponent("Jour", "inputJourBan", "Si le champ est vide le membre sera ban definitif", null, true))
                .AddComponents(new TextInputComponent("Reason", "inputReasonBan", "Si le champ est vide le motif sera \"pas de raison\"", null, false));

            // send message
            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, builder);
        }

        public async Task MemberModalBan(DiscordClient sender, DSharpPlus.EventArgs.ModalSubmitEventArgs e)
        {
            // variable
            var responseId = ulong.Parse(e.Values["inputIdBan"].ToString());
            var member = await e.Interaction.Guild.GetMemberAsync(responseId);
            var reason = e.Values["inputReasonBan"] != "" ? e.Values["inputReasonBan"] : "pas de raison";
            DateTime timestamp = DateTime.Now.AddDays(Int32.Parse(e.Values["inputJourBan"]));

            // connect to database
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("botdiscord");

            // create the elements to insert in the database
            var collection = database.GetCollection<BsonDocument>("ban");
            var document = new BsonDocument
            {
               {"uuid", e.Values["inputIdBan"] },
               {"username", member.Username },
               {"reason",  reason},
               {"date", timestamp }
            };
            await collection.InsertOneAsync(document);

            // create embed
            var embed = new DiscordEmbedBuilder()
            {
                Title = "Ban de < " + member.Username + " >",
                Description = "Le membre " + member.Username + "a bien été ban avec le motif \"" + reason + "\" !",
                Color = DiscordColor.Black,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail() { Url = "https://cdn3.emoji.gg/emojis/5695_staffbot.png" }
            };

            // create message
            var builder = new DiscordInteractionResponseBuilder()
                .AddEmbed(embed)
                .AsEphemeral(true);

            // send message
            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);

            // ban member
            await member.BanAsync(0, reason);
        }

        public async Task MemberUnban(DiscordClient sender, DSharpPlus.EventArgs.InteractionCreateEventArgs e)
        {
            // variable
            var response = ulong.Parse(e.Interaction.Data.Values.GetValue(0).ToString());

            // create embed
            var embed = new DiscordEmbedBuilder()
            {
                Title = "Unban de < " + e.Interaction.Data.Name + " >",
                Description = "Le membre " + e.Interaction.Data.Name + " a bien été unban !",
                Color = DiscordColor.Black,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail() { Url = "https://cdn3.emoji.gg/emojis/5695_staffbot.png" }
            };

            // create message
            var builder = new DiscordInteractionResponseBuilder()
                .AddEmbed(embed)
                .AsEphemeral(true);

            // send message
            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);

            // connect to database
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("botdiscord");
            var collection = database.GetCollection<BsonDocument>("ban");

            // search data
            var filter = Builders<BsonDocument>.Filter.Eq("uuid", response.ToString());

            // delete data
            await collection.DeleteOneAsync(filter);

            // unban member
            await e.Interaction.Guild.GetBanAsync(response).Result.User.UnbanAsync(e.Interaction.Guild);
        }

        public async Task MemberKick(DiscordClient sender, DSharpPlus.EventArgs.InteractionCreateEventArgs e)
        {
            // variable
            var response = ulong.Parse(e.Interaction.Data.Values.GetValue(0).ToString());
            var member = await e.Interaction.Guild.GetMemberAsync(response);

            // kick
            await member.BanAsync();
            await Task.Delay(2000);
            await e.Interaction.Guild.GetBanAsync(response).Result.User.UnbanAsync(e.Interaction.Guild);
        }

        public async Task MemberWarn(DiscordClient sender, DSharpPlus.EventArgs.InteractionCreateEventArgs e)
        {
            // variable
            var response = ulong.Parse(e.Interaction.Data.Values.GetValue(0).ToString());
            var member = await e.Interaction.Guild.GetMemberAsync(response);

            // create message
            var builder = new DiscordInteractionResponseBuilder()
                .WithTitle("Ban " + member.Username)
                .WithCustomId("modalWarn")
                .AddComponents(new TextInputComponent("Id (ne pas modifier)", "inputIdBan", null, member.Id.ToString(), true))
                .AddComponents(new TextInputComponent("Reason", "inputReasonBan", "Si le champ est vide le motif sera \"pas de raison\"", null, false));

            // send message
            await e.Interaction.CreateResponseAsync(InteractionResponseType.Modal, builder);
        }

        public async Task MemberModalWarn(DiscordClient sender, DSharpPlus.EventArgs.ModalSubmitEventArgs e)
        {
            // variable
            var responseId = ulong.Parse(e.Values["inputIdBan"].ToString());
            var member = await e.Interaction.Guild.GetMemberAsync(responseId);
            var reason = e.Values["inputReasonBan"] != "" ? e.Values["inputReasonBan"] : "pas de raison";

            // connect to database
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("botdiscord");

            // create the elements to insert in the database
            var collection = database.GetCollection<BsonDocument>("warn");
            var document = new BsonDocument
            {
               {"uuid", e.Values["inputIdBan"] },
               {"username", member.Username },
               {"reason",  reason},
            };
            await collection.InsertOneAsync(document);

            // create embed
            var embed = new DiscordEmbedBuilder()
            {
                Title = "Warn de < " + member.Username + " >",
                Description =  member.Username + "a été warn avec le motif \"" + reason + "\" !",
                Color = DiscordColor.Red,
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail() { Url = "https://cdn3.emoji.gg/emojis/5695_staffbot.png" }
            };

            // create message
            var builder = new DiscordInteractionResponseBuilder()
                .AddEmbed(embed);

            // send message
            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
        }
    }
}



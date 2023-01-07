using ConsoleApp1.Commands;
using ConsoleApp1.Interactions;
using DSharpPlus;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ConsoleApp1
{
    class Program
    {
        public static DiscordClient? discord;
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = "MTA1ODAxMzc3ODMxNjM4MjIwOQ.GGGLXq.nv7dJKYR-bvmM8CU_x0MyywkqXziRR3INu_74c",
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.All,
            });

            var slashCommands = discord.UseSlashCommands();
            discord.UseInteractivity();

            

            discord.ComponentInteractionCreated += Discord_ComponentInteractionCreated;
            discord.ModalSubmitted += Discord_ModalSubmitted;           

            slashCommands.RegisterCommands<AdminCommands>(1058013386186706985);

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private static async Task Discord_ModalSubmitted(DiscordClient sender, DSharpPlus.EventArgs.ModalSubmitEventArgs e)
        {
            if (e.Interaction.Data.CustomId == "modalBan") await new AdminInteractions().MemberModalBan(sender, e);
            else if (e.Interaction.Data.CustomId == "modalWarn") await new AdminInteractions().MemberModalWarn(sender, e);
        }

        private static async Task Discord_ComponentInteractionCreated(DiscordClient sender, DSharpPlus.EventArgs.ComponentInteractionCreateEventArgs e)
        {
            if (e.Interaction.Data.CustomId == "memberBan") await new AdminInteractions().MemberBan(sender, e);
            else if (e.Interaction.Data.CustomId == "memberUnban") await new AdminInteractions().MemberUnban(sender, e);
            else if (e.Interaction.Data.CustomId == "memberKick") await new AdminInteractions().MemberKick(sender, e);
            else if (e.Interaction.Data.CustomId == "memberWarn") await new AdminInteractions().MemberWarn(sender, e);
        }

        private static async Task Update()
        {
            await Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    // connect to database
                    var client = new MongoClient("mongodb://localhost:27017");
                    var database = client.GetDatabase("botdiscord");

                    // collection
                    var collection = database.GetCollection<BsonDocument>("ban");

                    // find data
                    var data = await collection.Find(new BsonDocument()).ToListAsync();
                    foreach(var i in data)
                    {
                        if (i.GetElement("date").Value.ToLocalTime() >= DateTime.Now.ToLocalTime())
                        {
                            await discord.GetGuildAsync(1058013386186706985).Result.GetBanAsync(ulong.Parse(i.GetElement("uuid").Value.ToString())).Result.User.UnbanAsync(discord.GetGuildAsync(1058013386186706985).Result);

                            // delete data
                            var filter = Builders<BsonDocument>.Filter.Eq("uuid", i.GetElement("uuid").Value.ToString());
                            await collection.DeleteOneAsync(filter);
                        }
                    }

                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            });
        }
    }
}


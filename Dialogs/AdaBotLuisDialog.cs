using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace SimpleEchoBot.Dialogs
{
    [Serializable]
    public class AdaBotLuisDialog : LuisDialog<object>
    {
        public AdaBotLuisDialog() : base(new LuisService(model: new LuisModelAttribute(
            ConfigurationManager.AppSettings["LuisAppId"],
            ConfigurationManager.AppSettings["LuisAPIKey"],
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hmmm...");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greet")]
        public async Task GreetIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hello! And welcome to AdaBot!");
            context.Wait(MessageReceived);
        }

        [LuisIntent("StartConversation")]
        public async Task StartConversationIntent(IDialogContext context, LuisResult result)
        {
            EntityRecommendation age;
            EntityRecommendation medium;
            result.TryFindEntity("builtin.age", out age);
            result.TryFindEntity("medium", out medium);

            await context.PostAsync($"Awesome! Let's get some {medium.Entity} for your {age.Entity}.");
            context.Wait(MessageReceived);
        }
    }
}
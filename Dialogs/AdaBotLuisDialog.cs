using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace SimpleEchoBot.Dialogs
{
    using System.Collections.Generic;
    using System.Linq;

    using Provider;

    [Serializable]
    public class AdaBotLuisDialog : LuisDialog<object>
    {
        private string mediumType;

        private int age;

        private string level;

        private string domain;

        private static readonly IEnumerable<string> levels = new[] { "Beginner", "Intermediate", "Advanced" };

        private static readonly IEnumerable<string> interests = new[] { "Games", "Mobile", "Web", "Anything" };


        public AdaBotLuisDialog()
            : base(
                new LuisService(
                    model: new LuisModelAttribute(
                        ConfigurationManager.AppSettings["LuisAppId"],
                        ConfigurationManager.AppSettings["LuisAPIKey"],
                        domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hmmm...");
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Greet")]
        public async Task GreetIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hello! And welcome to AdaBot!");
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("StartConversation")]
        public async Task StartConversationIntent(IDialogContext context, LuisResult result)
        {
            EntityRecommendation inputAge;
            EntityRecommendation inputMedium;
            result.TryFindEntity("builtin.age", out inputAge);
            result.TryFindEntity("medium", out inputMedium);

            this.mediumType = inputMedium.Entity;
            this.age = 42;

            PromptDialog.Choice(
                context,
                LevelSelectedAsync,
                levels,
                "Great! What is your kid's level of knowledge?",
                "Didn't get that!");
        }

        public async Task LevelSelectedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            var selectedLevel = await argument;

            if (levels.Contains(selectedLevel))
            {
                // valid level selected
                this.level = selectedLevel;
                PromptDialog.Choice(
                    context,
                    InterestSelectedAsync,
                    interests,
                    $"I found a lot of courses for the {this.level} level. What is your kid interested in?",
                    "Didn't get that!");
            }
            else
            {
                await context.PostAsync("Please start over.");
                context.Wait(this.MessageReceived);
            }
        }

        public async Task InterestSelectedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            var selectedInterest = await argument;

            if (interests.Contains(selectedInterest))
            {
                // valid level selected
                this.domain = selectedInterest;
                
                await context.PostAsync($"Details: Medium={this.mediumType}, Age={this.age}, Level={this.level}, Interest={this.domain}");
            }
            else
            {
                await context.PostAsync("Please start over.");
            }

            context.Wait(this.MessageReceived);
        }
    }
}
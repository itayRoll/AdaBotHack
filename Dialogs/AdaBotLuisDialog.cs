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

    using Microsoft.Bot.Connector;

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
            
            this.age = 10;

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
                    $"I found a lot of {this.mediumType} options for the {this.level} level. What is your kid interested in?",
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

                // get input from user
                var query = new Query(this.age, this.level, this.domain, this.mediumType);

                var provider = new FileProvider();
                // get results accourding to the input
                var results = provider.GetResults(query);

                var thumbnailCard = getAdaptiveCard(results.FirstOrDefault());
                IMessageActivity reply = context.MakeMessage();
                reply.Attachments.Add(thumbnailCard.ToAttachment());

                await context.PostAsync(reply);
            }
            else
            {
                await context.PostAsync("Please start over.");
            }

            context.Wait(this.MessageReceived);
        }

        private ThumbnailCard getAdaptiveCard(Result suggestedResult)
        {
            return new ThumbnailCard
                       {
                           Title = suggestedResult.DisplayName,
                           Text = suggestedResult.Description,
                           Images = new List<CardImage> { new CardImage(suggestedResult.Image) },
                           Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: suggestedResult.Link) }
                       };
        }
    }
}
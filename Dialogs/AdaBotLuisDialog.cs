namespace SimpleEchoBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using DataProvider;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class AdaBotLuisDialog : LuisDialog<object>
    {
        private static readonly IEnumerable<string> levels = new[] {"Beginner", "Intermediate", "Advanced"};
        private static readonly IEnumerable<string> interests = new[] {"Game", "Mobile", "Web", "Hardware", "Anything"};
        private int age;
        private string domain;
        private string level;
        private string mediumType;

        public AdaBotLuisDialog()
            : base(
                new LuisService(
                    new LuisModelAttribute(
                        ConfigurationManager.AppSettings["LuisAppId"],
                        ConfigurationManager.AppSettings["LuisAPIKey"],
                        domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }

        public static string Greeting { get; } = @"Hi I’m AdaBot!
                                                    \n
                                                    I would love to provide you with some great content for programming.
                                                    \n
                                                    Let me know the Domain, Medium type, level and age of your child, so I can find something that will do.
                                                    \n
                                                    You can also provide the Language, Programming language and Duration.
                                                    \n
                                                    For example: “I want a workshop for mobile development for my 9 years old who is an intermediate”
                                                    \n
                                                    Or: “I'm looking for an inspirational video for my 9 years old who is a beginner”";

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hmmm...\nI don't have anything smart to say about that.");
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Greet")]
        public async Task GreetIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync(Greeting);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("StartConversation")]
        public async Task StartConversationIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                EntityRecommendation inputAge;
                EntityRecommendation inputMedium;

                if (result.TryFindEntity("builtin.age", out inputAge))
                {
                    Match match = Regex.Match(inputAge.Entity, "\\d+");
                    age = Convert.ToInt32(match.Value);

                    if (result.TryFindEntity("medium", out inputMedium))
                    {
                        mediumType = inputMedium.Entity;

                        PromptDialog.Choice(
                            context,
                            LevelSelectedAsync,
                            levels,
                            "Great! What is your kid's level of knowledge?",
                            "Didn't get that!");
                    }
                    else
                    {
                        await this.HandleError(context, "I'm sorry, I couldn't understand what you are looking for");
                    }
                }
                else
                {
                    await this.HandleError(context, "couldn't get age");
                }
            }
            catch (Exception e)
            {
                await this.HandleError(context, e.ToString());
            }
        }

        public async Task LevelSelectedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            string selectedLevel = await argument;

            if (levels.Contains(selectedLevel))
            {
                // valid level selected
                level = selectedLevel;
                PromptDialog.Choice(
                    context, this.InterestSelectedAsync,
                    interests,
                    $"I found a lot of {mediumType} options for the {level} level. What is your kid interested in?",
                    "Didn't get that!");
            }
            else
            {
                await this.HandleError(context, $"couldn't find level {selectedLevel}");
            }
        }

        public async Task InterestSelectedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            string selectedInterest = await argument;

            if (interests.Contains(selectedInterest))
            {
                // valid level selected
                domain = selectedInterest;

                // get input from user
                Query query = new Query(age, level, domain, mediumType);

                FileProvider provider = new FileProvider();
                // get results accourding to the input
                List<Result> results = provider.GetResults(query);

                Result result = results.FirstOrDefault();

                if (result != null)
                {
                    ThumbnailCard thumbnailCard = this.getAdaptiveCard(results.FirstOrDefault());
                    IMessageActivity reply = context.MakeMessage();
                    reply.Attachments.Add(thumbnailCard.ToAttachment());

                    await context.PostAsync(reply);
                }
                else
                {
                    await context.PostAsync("Sorry, couldn't find a good match for you.");
                }
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
                Images = new List<CardImage> {new CardImage(suggestedResult.Image)},
                Buttons = new List<CardAction>
                {
                    new CardAction(ActionTypes.OpenUrl, "Get Started", value: suggestedResult.Link)
                }
            };
        }

        private async Task HandleError(IDialogContext context, string error)
        {
            await context.PostAsync($"Oops! I've encountered an error ({error}).\nPlease start over.");
            context.Wait(this.MessageReceived);
        }
    }
}
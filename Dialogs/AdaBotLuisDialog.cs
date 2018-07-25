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
    using System.Text.RegularExpressions;

    using Microsoft.Bot.Connector;

    using DataProvider;

    [Serializable]
    public class AdaBotLuisDialog : LuisDialog<object>
    {
        private string mediumType;

        private int age;

        private LevelType level;

        private DomainType domain;

        private static readonly List<LevelType> levels = Enum.GetValues(typeof(LevelType)).Cast<LevelType>().ToList();

        private static readonly IEnumerable<DomainType> interests = Enum.GetValues(typeof(DomainType)).Cast<DomainType>().ToList();

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
            await context.PostAsync("Hmmm...\nI don't have anything smart to say about that.");
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
            try
            {
                EntityRecommendation inputAge;
                EntityRecommendation inputMedium;

                if (result.TryFindEntity("builtin.age", out inputAge))
                {
                    var match = Regex.Match(inputAge.Entity, "\\d+");
                    this.age = Convert.ToInt32(match.Value);

                    if (result.TryFindEntity("medium", out inputMedium))
                    {
                        this.mediumType = inputMedium.Entity;

                        PromptDialog.Choice(
                            context,
                            LevelSelectedAsync,
                            levels,
                            "Great! What is your kid's level of knowledge?",
                            "Didn't get that!");
                    }
                    else
                    {
                        await this.HandleError(context, "couldn't understand what you are looking for");
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

        public async Task LevelSelectedAsync(IDialogContext context, IAwaitable<LevelType> argument)
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
                await this.HandleError(context, $"couldn't find level {selectedLevel}");
            }
        }

        public async Task InterestSelectedAsync(IDialogContext context, IAwaitable<DomainType> argument)
        {
            var selectedInterest = await argument;

            if (interests.Contains(selectedInterest))
            {
                // valid level selected
                this.domain = selectedInterest;

                // get input from user
                var query = new Query(this.age, this.level.ToString(), this.domain.ToString(), this.mediumType);

                var provider = new FileProvider();
                // get results accourding to the input
                var results = provider.GetResults(query);
                
                IMessageActivity reply;
                if (results.Any())
                {
                    reply = GetReply(context, results.First());
                }
                else
                {
                    reply = GetCannedReply(context);
                }

                await context.PostAsync(reply);

            }
            else
            {
                await context.PostAsync("Please start over.");
            }

            context.Wait(this.MessageReceived);
        }       

        private IMessageActivity GetReply(IDialogContext context, Result suggestedResult)
        {
            CardAction getStartedCardAction = new CardAction(ActionTypes.OpenUrl, "Get Started", value: suggestedResult.Link);
            CardAction getMoreCardAction = new CardAction(ActionTypes.PostBack, "Get More", value: "GetMore");

            var thumbnailCard = new ThumbnailCard
            {
                Title = suggestedResult.DisplayName,
                Text = suggestedResult.Description,
                Images = new List<CardImage> { new CardImage("https://www.google.com/url?sa=i&source=images&cd=&cad=rja&uact=8&ved=2ahUKEwiHuJWk0rfcAhWRCOwKHSZvCT4QjRx6BAgBEAU&url=http%3A%2F%2Fwww.edutechpost.com%2Fcodemonkey-coding-children%2F&psig=AOvVaw3VQGIaT364jlPrZWbZN5_S&ust=1532518456232233") },
                Buttons = new List<CardAction> { getStartedCardAction, }
            };

            IMessageActivity reply = context.MakeMessage();
            reply.Attachments.Add(thumbnailCard.ToAttachment());

            return reply;
        }

        private IMessageActivity GetCannedReply(IDialogContext context)
        {
            Result suggestedResult = new Result()
            {
                DisplayName = "Code Monkey- Python chatbot",
                Link = "https://www.playcodemonkey.com/python-chatbot/",
                Description = "Create a chatbot using python!"
            };

            CardAction getStartedCardAction = new CardAction(ActionTypes.OpenUrl, "Get Started", value: suggestedResult.Link);

            var thumbnailCard = new ThumbnailCard
            {
                Title = suggestedResult.DisplayName,
                Text = suggestedResult.Description,
                Images = new List<CardImage> { new CardImage("https://www.google.com/url?sa=i&source=images&cd=&cad=rja&uact=8&ved=2ahUKEwiHuJWk0rfcAhWRCOwKHSZvCT4QjRx6BAgBEAU&url=http%3A%2F%2Fwww.edutechpost.com%2Fcodemonkey-coding-children%2F&psig=AOvVaw3VQGIaT364jlPrZWbZN5_S&ust=1532518456232233") },
                Buttons = new List<CardAction> { getStartedCardAction }
            };

            IMessageActivity reply = context.MakeMessage();
            reply.Text = "I didn't find content to match your request, Here is a suggestion that I really like!";
            reply.Attachments.Add(thumbnailCard.ToAttachment());

            return reply;
        }

        private async Task HandleError(IDialogContext context, string error)
        {
            await context.PostAsync($"Oops! I've encountered an error ({error}).\nPlease start over.");
            context.Wait(this.MessageReceived);
        }
    }
}
using System;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using Provider;
using System.Collections.Generic;
using AdaptiveCards;

namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    [Serializable]
    public class SuggestionDialog : IDialog<object>
    {
        protected int count = 1;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {

            Result result = new Result()
            {
                DisplayName = "Code Monkey- Python chatbot",
                Link = "https://www.playcodemonkey.com/python-chatbot/",
                Description = "I'm the description of code monkey, if you want to know about it ask someone"
            };

            IList<Result> resultsList = new List<Result>() { result };

            Result suggestedResult = resultsList?[0];

            var thumbnailCard = new ThumbnailCard
            {
                Title = suggestedResult.DisplayName,
                Text = suggestedResult.Description,
                Images = new List<CardImage> { new CardImage("https://www.google.com/url?sa=i&source=images&cd=&cad=rja&uact=8&ved=2ahUKEwiHuJWk0rfcAhWRCOwKHSZvCT4QjRx6BAgBEAU&url=http%3A%2F%2Fwww.edutechpost.com%2Fcodemonkey-coding-children%2F&psig=AOvVaw3VQGIaT364jlPrZWbZN5_S&ust=1532518456232233") },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Get Started", value: suggestedResult.Link) }
            };

            IMessageActivity reply = context.MakeMessage();
            reply.Attachments.Add(thumbnailCard.ToAttachment());

            await context.PostAsync(reply);

            context.Wait(MessageReceivedAsync);
        }

        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }

    }
}
namespace Microsoft.Bot.Sample.SimpleEchoBot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Builder.Dialogs;

    using Provider;

    [Serializable]
    public class EchoDialog : IDialog<object>
    {
		private const int RawMediumTypeIndex = 4;
	    private const int RawAgeIndex = 7;

        protected int count = 1; // temp
	    private string mediumType;
	    private int age;
	    private string level;
	    private string domain;

	    private static readonly IEnumerable<string> levels = new[] {"Begginer", "Intermediate", "Advanced"};
	    private static readonly IEnumerable<string> interests = new[] {"Games", "Mobile", "Web", "Anything"};

        private readonly IProvider provider = new FileProvider();
        private List<Result> results;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(ConversationStartAsync);
        }

	    public async Task ConversationStartAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
	    {
		    var msg = await argument;
			
		    if (IntentVerifier(msg.Text, "looking for a ", "year old kid"))
		    {
				// valid start to the conversation
			    var msgArr = msg.Text.Split(' ');

			    this.mediumType = msgArr[RawMediumTypeIndex];

			    try
			    {
				    this.age = int.Parse(msgArr[RawAgeIndex]);
					PromptDialog.Choice(
						context,
						LevelSelectedAsync,
						levels,
						"Great! What is your kid's level?",
						"Didn't get that!");
			    }
			    catch (FormatException)
			    {
					// invalid age
				    await context.PostAsync("Please type in the message again with a valid age.");
				    context.Wait(ConversationStartAsync);
				}
			    
		    }
		    else
		    {
				// invalid start to the conversation
			    await context.PostAsync("Please type in the message again correctly.");
			    context.Wait(ConversationStartAsync);
			}
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
			    context.Wait(ConversationStartAsync);
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

                // get results accourding to the input
		        results = provider.GetResults(query);

                // post replay to user
				await context.PostAsync($"Details: Medium={this.mediumType}, Age={this.age}, Level={this.level}, Interest={this.domain}");
		    }
		    else
		    {
			    await context.PostAsync("Please start over.");
		    }

		    context.Wait(ConversationStartAsync);
		}

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;

            if (message.Text == "reset")
            {
                PromptDialog.Confirm(
                    context,
                    AfterResetAsync,
                    "Are you sure you want to reset the count?",
                    "Didn't get that!",
                    promptStyle: PromptStyle.Auto);
            }
            else
            {
                await context.PostAsync($"{this.count++}: You said {message.Text}");
                context.Wait(MessageReceivedAsync);
            }
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

	    private static bool IntentVerifier(string rawMessage, params string[] importantParts)
	    {
		    var msg = rawMessage.Trim().ToLower();
		    return importantParts.All(part => msg.Contains(part));
	    }

    }
}
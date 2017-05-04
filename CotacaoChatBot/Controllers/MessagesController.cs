using CotacaoChatBot.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace CotacaoChatBot.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            if (activity != null)
            {

                switch (activity.GetActivityType())
                {
                    case ActivityTypes.Message:
                        await Conversation.SendAsync(activity, () => new RootDialog());
                        break;

                    case ActivityTypes.ConversationUpdate:
                    case ActivityTypes.ContactRelationUpdate:
                    case ActivityTypes.Typing:
                    case ActivityTypes.DeleteUserData:
                    default:
                        Trace.TraceError($"Unknown activity type ignored: {activity.GetActivityType()}");
                        break;
                }
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }
    }
}
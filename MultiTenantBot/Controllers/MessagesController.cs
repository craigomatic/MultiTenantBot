using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;

namespace MultiTenantBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            }
            else if (activity.Type == ActivityTypes.Event && activity.Name == "token")
            {
                var tenant = activity.Value.ToString();
                
                //store the tenant this user is associated with in the state
                var stateClient = activity.GetStateClient();

                var privateData = await stateClient.BotState.GetPrivateConversationDataAsync(activity.ChannelId, activity.Conversation.Id, activity.From.Id);

                if (privateData.GetProperty<string>("tenant") != null)
                {
                    //ignore, only allow the tenant to be specified once
                }
                else
                {
                    privateData.SetProperty<string>("tenant", tenant);

                    await stateClient.BotState.SetPrivateConversationDataAsync(activity.ChannelId, activity.Conversation.Id, activity.From.Id, privateData);

                    var connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                    var reply = (activity as Activity).CreateReply();
                    reply.Type = ActivityTypes.Message;
                    reply.Text = $"User from tenant {tenant} joined.";

                    await connector.Conversations.ReplyToActivityAsync(reply);
                }
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}
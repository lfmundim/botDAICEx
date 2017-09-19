using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lime.Protocol;
using Takenet.MessagingHub.Client.InfoEntertainmentExtensions.Navigation.NavigationExtensions;
using Takenet.MessagingHub.Client.Extensions.EventTracker;
using NLog;
using Lime.Messaging.Contents;
using Newtonsoft.Json;
using System.Threading;

namespace DAICEx
{
    public class EventNotificator : IEventNotificator
    {
        private readonly IEventTrackExtension _eventTrack;

        public EventNotificator(
            IEventTrackExtension eventTrack
            )
        {
            _eventTrack = eventTrack;
        }
        public async Task<Document> RegisterEvent(Document eventDocument, Message originatorMessage)
        {
            if(eventDocument != null)
            {
                var data = (eventDocument as PlainText).Text;
                var ev = JsonConvert.DeserializeObject<BotEvent>(data);

                for (int i = 0; i < Convert.ToInt32(ev.EventQuantity); i++)
                {
                    await _eventTrack.AddAsync(ev.EventName, ev.ActionName);
                }
            }
           
            return null;
        }

    }

    internal class BotEvent
    {
        public string EventName { get; set; }
        public string ActionName { get; set; }
        public string EventQuantity { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;
using Twilio.AspNet.Core;
using Twilio.TwiML;
using Twilio.TwiML.Messaging;

namespace Dalle2ImageSmsApi.Controllers;

[ApiController]
[Route("[controller]")]
public class IncomingSmsController : TwilioController
{
    [HttpPost]
    public async Task<TwiMLResult> Index()
    {
        var form = await Request.ReadFormAsync();
        var incomingText = form["Body"];
        
        var message = new Message();
        message.Body($"Here's the image for your query: {incomingText}");
        message.Media(new Uri("https://picsum.photos/1024/1024"));

        var messagingResponse = new MessagingResponse();
        messagingResponse.Message(message);
        
        return TwiML(messagingResponse);
    }
}
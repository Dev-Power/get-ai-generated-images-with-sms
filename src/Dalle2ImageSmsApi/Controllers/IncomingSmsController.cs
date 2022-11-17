using Microsoft.AspNetCore.Mvc;
using OpenAI.GPT3.Interfaces;
using OpenAI.GPT3.ObjectModels.RequestModels;
using Twilio.AspNet.Core;
using Twilio.TwiML;
using Twilio.TwiML.Messaging;

namespace Dalle2ImageSmsApi.Controllers;

[ApiController]
[Route("[controller]")]
public class IncomingSmsController : TwilioController
{
    private readonly IOpenAIService _openAiService;

    public IncomingSmsController(IOpenAIService openAiService)
    {
        _openAiService = openAiService;
    }
    
    [HttpPost]
    public async Task<TwiMLResult> Index()
    {
        var form = await Request.ReadFormAsync();
        var incomingText = form["Body"];

        var createImageRequest = new ImageCreateRequest
        {
            Size = "1024x1024",
            N = 1,
            Prompt = incomingText,
            ResponseFormat = "url"
        };

        var createImageResponse = await _openAiService.Image.CreateImage(createImageRequest);
        var image = createImageResponse.Results.First();

        var message = new Message();
        message.Body($"Here's the image for your query: {incomingText}");
        message.Media(new Uri(image.Url));

        var messagingResponse = new MessagingResponse();
        messagingResponse.Append(message);
        
        return TwiML(messagingResponse);
    }
}
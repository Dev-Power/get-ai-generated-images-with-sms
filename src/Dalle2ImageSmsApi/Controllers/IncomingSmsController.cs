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
    private readonly ILogger<IncomingSmsController> _logger;
    private readonly IOpenAIService _openAiService;
 
    public IncomingSmsController(
        ILogger<IncomingSmsController> logger, 
        IOpenAIService openAiService
    )
    {
        _logger = logger;
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
        if(!createImageResponse.Successful)
        {
            var errorMessage = "An error occurred trying to create OpenAI image." +
                               $" {createImageResponse.Error.Code}: {createImageResponse.Error.Message}.";
            _logger.LogError(errorMessage);
            
            return new MessagingResponse()
                .Message("An unexpected error occurred. Try again later.")
                .ToTwiMLResult();
        }
 
        var image = createImageResponse.Results.First();
 
        var message = new Message();
        message.Body($"Here's the image for your query: {incomingText}");
        message.Media(new Uri(image.Url));
 
        return new MessagingResponse()
            .Append(message)
            .ToTwiMLResult();
    }
}
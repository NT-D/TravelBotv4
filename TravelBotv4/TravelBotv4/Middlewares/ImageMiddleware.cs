using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder;
using TravelBotv4.Services;
using TravelBotv4.Models;

namespace TravelBotv4.Middlewares
{
    public class ImageMiddleware : IMiddleware
    {
        public const string ImageRecognizerResultKey = "ImageRecognizerResult";
        private readonly float RecognizeThreshold;
        private readonly IImageRecognizer ComputerVisionRecognizer;
        private readonly IImageRecognizer CustomVisionRecognizer;
        private readonly IImageRecognizer BingImageRecognizer;

        public ImageMiddleware(float recognizeThreshold)
        {
            RecognizeThreshold = recognizeThreshold;
            ComputerVisionRecognizer = new ComputerVisionRecognizer();
            CustomVisionRecognizer = new CustomVisionRecognizer();
            BingImageRecognizer = new BingImageRecognizer();
        }

        public async Task OnProcessRequest(IBotContext context, MiddlewareSet.NextDelegate next)
        {
            //Check whether message contains images. If it doens't contain, pass process to later middleware.
            Attachment imageAttachment = context.Request.Attachments?.FirstOrDefault(attachment => attachment.ContentType.Contains("image"));
            if (imageAttachment == null) await next();

            using (var stream = await GetImageStream(imageAttachment))
            {
                ImageRecognizeResult result;
                #region Check randmark with computer vision API
                await context.SendActivity("Used Computer Vision Service");
                result = await ComputerVisionRecognizer.DetectImage(stream);
                //TODO: Check result and make conditional jump. If the result is under threshold, pass the process to next middleware. Else we continue other services.
                #endregion

                #region Check custom vision service
                await context.SendActivity("Also used Custom Vision Service");
                result = await CustomVisionRecognizer.DetectImage(stream);
                //TODO: Check result and make conditional jump. If the result is under threshold, pass the process to next middleware. Else we continue other services.
                #endregion

                #region  Check Bing Image Search
                await context.SendActivity("Also used Bing Image Search");
                result = await BingImageRecognizer.DetectImage(stream);
                //TODO: Check result and make conditional jump. If the result is under threshold, pass the process to next middleware. Else we continue other services.
                #endregion
            }
            await next();//Will change it. Because we will handle it after each detect.
        }

        private async Task<Stream> GetImageStream(Attachment attachedImage)
        {
            throw new NotImplementedException();
        }
    }
}

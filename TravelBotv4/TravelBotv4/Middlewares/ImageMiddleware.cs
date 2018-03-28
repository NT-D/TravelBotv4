﻿using System;
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
        private readonly ComputerVisionRecognizer ComputerVisionRecognizer;
        private readonly CustomVisionRecognizer CustomVisionRecognizer;
        private readonly BingImageRecognizer BingImageRecognizer;

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
            if (imageAttachment == null)
            {
                await next();
            }
            else
            {
                using (var stream = await GetImageStream(imageAttachment))
                {
                    ImageRecognizeResult result = new ImageRecognizeResult();

                    var computerVisionResult = await ComputerVisionRecognizer.DetectImage(stream, this.RecognizeThreshold) as ComputerVisionResult;
                    if (computerVisionResult.IsSure)
                    {
                        result.RecognizedServiceType = ImageServiceType.ComputerVisionService;
                        result.ComputerVisionResult = computerVisionResult;
                        context.Set(ImageRecognizerResultKey, result);
                        await context.SendActivity($"Computer Vision: This picture is {result.ComputerVisionResult.Result.Landmarks?[0].Name}");
                        await next();
                    }
                    else
                    {
                        var customVisionResult = await CustomVisionRecognizer.DetectImage(stream, this.RecognizeThreshold) as CustomVisionResult;
                        if (customVisionResult.IsSure)
                        {
                            result.RecognizedServiceType = ImageServiceType.CustomVisionService;
                            result.CustomVisionResult = customVisionResult;
                            context.Set(ImageRecognizerResultKey, result);
                            await context.SendActivity($"Custom Vision: This picture is {result.CustomVisionResult.Predictions?[0].Tag}");
                            await next();
                        }
                        else
                        {
                            await context.SendActivity("Middleware debug: Also used Bing Image Search");
                            var bingImageResult = await BingImageRecognizer.DetectImage(stream, this.RecognizeThreshold);
                            //TODO: Check result and make conditional jump. If the result is under threshold, pass the process to next middleware. Else we continue other services.
                        }
                    }
                }
                await next();//Will change it. Because we will handle it after each detect.
            }
        }

        private async Task<Stream> GetImageStream(Attachment attachedImage)
        {
            using (var httpClient = new HttpClient())
            {
                //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(attachedImage.ContentType));
                var response = await httpClient.GetAsync(attachedImage.ContentUrl);
                if (response.IsSuccessStatusCode)
                {
                    Stream stream = await response.Content.ReadAsStreamAsync();
                    return stream;
                }
                else
                {
                    throw new Exception();
                }
            }
        }
    }
}

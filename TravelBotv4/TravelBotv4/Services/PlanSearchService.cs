﻿using System;
using System.Threading.Tasks;
using System.Net.Http;
using TravelBotv4.Services.Models;
using Newtonsoft.Json;

namespace TravelBotv4.Services
{
    public class PlanSearchService : ISearchService
    {
        private static string Url = "https://dv-jtb-content.azurewebsites.net/plans";

        private static HttpClient HttpClient = new HttpClient();

        public async Task<BaseSearchResult> SearchAsync(BaseSearchRequest req)
        {
            return await Search(req);
        }

        public async Task<BaseSearchResult> Search(BaseSearchRequest req)
        {
            try
            {
                var response = await HttpClient.GetStringAsync($"{Url}?{req.QueryString}");

                return JsonConvert.DeserializeObject<PlansResult>(response);
            }
            catch (Exception)
            {
                return new PlansResult();
            }
        }
    }
}

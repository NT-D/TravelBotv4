using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelBotv4.Services.Model
{
    [Serializable]
    public class PlansRequest : BaseSearchRequest
    {
        public int count { get; set; }
        public string token { get; set; }
        public string area_name { get; set; }
        public string category_name { get; set; }
        public string season { get; set; }

        public PlansRequest()
        {
            count = 1;
        }

        public override string QueryString
        {
            get
            {
                var query = $"count={count}";
                if (!string.IsNullOrEmpty(area_name)) query += $"&area_name={area_name}";
                if (!string.IsNullOrEmpty(category_name)) query += $"&category_name={category_name}";
                if (!string.IsNullOrEmpty(season)) query += $"&season={season}";
                if (!string.IsNullOrEmpty(token)) query += $"&token={token}";
                return query;
            }
        }

        public override void Modify(BaseSearchResult result)
        {
            var plansResult = result as PlansResult;
            token = plansResult.token;
        }

        public override void ModifyForNext() { }

        public override void ModifyForPrev() { }
    }

    public class PlansResult : BaseSearchResult
    {
        public Plan[] items { get; set; }
        public string token { get; set; }

        public override string[] Ids
        {
            get
            {
                return items.Select(x => x.plan.id).ToArray();
            }
        }

        public override List<Attachment> Attachments
        {
            get
            {
                return items.Select(x => new HeroCard
                {
                    Title = x.plan.name,
                    Text = x.plan.summary,
                    Images = x.plan.images.Select(y => new CardImage
                    {
                        Url = y.url
                    }).ToList()
                }.ToAttachment()).ToList();
            }
        }

        public override Entity ChannelData()
        {
            return new Entity()
            {
            };
        }

        public override bool IsValid
        {
            get
            {
                return items?.Length > 0;
            }
        }

        public override Geo[] Geos
        {
            get
            {
                // TODO(shinichi-tanabe) what should we return?
                return null;
            }
        }
    }

    public class Plan
    {
        public Item plan { get; set; }

        public class Item
        {
            public string id { get; set; }
            public string name { get; set; }
            public string summary { get; set; }
            public string[] tips { get; set; }
            public Season season { get; set; }
            public Start_Point[] start_point { get; set; }
            public Image[] images { get; set; }
            public Spot[] spots { get; set; }
            public string[] keywords { get; set; }
            public int lang_code { get; set; }
        }


        public class Season
        {
            public int code { get; set; }
            public string name { get; set; }
            public string remark { get; set; }
        }

        public class Image
        {
            public int id { get; set; }
            public int order { get; set; }
            public string url { get; set; }
            public string credit { get; set; }
        }

        public class Start_Point
        {
            public string code { get; set; }
            public string name { get; set; }
            public int level { get; set; }
        }

        public class Homen
        {
            public string code { get; set; }
            public string name { get; set; }
        }

        public class Ken
        {
            public string code { get; set; }
            public string name { get; set; }
        }

        public class Area
        {
            public string code { get; set; }
            public string name { get; set; }
        }

        public class Mesh
        {
            public string code { get; set; }
            public string name { get; set; }
        }

        public class Chiku
        {
            public string code { get; set; }
            public string name { get; set; }
        }

        public class Shiku
        {
            public string code { get; set; }
            public string name { get; set; }
        }

        public class Spot
        {
            public int order { get; set; }
            public bool is_main { get; set; }
            public string start_time { get; set; }
            public int required_time { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string ruby { get; set; }
            public Category[] categories { get; set; }
            public string tel_to { get; set; }
            public string access { get; set; }
            public string tel_no { get; set; }
            public Area[] area { get; set; }
            public Geo geo { get; set; }
            public Image[] images { get; set; }
            public string[] keywords { get; set; }
            public int lang_code { get; set; }
            public int interval_time { get; set; }
            public string[] interval_means { get; set; }
            public string summary { get; set; }

            public class Geo
            {
                public float[] coordinates { get; set; }
                public string type { get; set; }
            }

            public class Category
            {
                public CategoryDetail large { get; set; }
                public CategoryDetail middle { get; set; }
                public CategoryDetail small { get; set; }
            }

            public class CategoryDetail
            {
                public int code { get; set; }
                public string name { get; set; }
                public int level { get; set; }
            }

            public class Area
            {
                public string id { get; set; }
                public string code { get; set; }
                public string name { get; set; }
                public int level { get; set; }
            }

            public class Image
            {
                public int id { get; set; }
                public int order { get; set; }
                public string url { get; set; }
                public string credit { get; set; }
            }
        }
    }
}

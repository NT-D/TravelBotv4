using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TravelBotv4.Services.Model
{
    [Serializable]
    public class SpotsRequest : BaseSearchRequest
    {
        public int count { get; set; }
        public int offset { get; set; }
        public string address { get; set; }
        public string keyword { get; set; }
        public string options { get; set; }
        public string search_fields { get; set; }
        public string service_provideds { get; set; }
        public string option_recommendeds { get; set; }
        public string business_hour_type { get; set; }

        public SpotsRequest()
        {
            offset = 0;
            count = 1;
        }

        public override void Modify(BaseSearchResult res) { }

        public override void ModifyForNext()
        {
            offset++;
        }

        public override void ModifyForPrev()
        {
            if (offset > 0) offset--;
        }

        public override string QueryString
        {
            get
            {
                var query =
                    $"count={count}" +
                    $"&offset={offset}" +
                    $"&keyword={keyword}" +
                    $"&address={address}" +
                    $"&search_fields={search_fields}" +
                    $"&service_provideds={service_provideds}" +
                    $"&option_recommendeds={option_recommendeds}" +
                    $"&business_hour_type={business_hour_type}" +
                    $"{options}";

                if (IsValidLocation())
                {
                    query += $"&lat={lat}&lon={lon}&distance={distance}";
                }

                return query;
            }
        }

        public void AddSearchFields(List<string> target, string field)
        {
            if (target == null || target.Count() == 0) return;

            if (!string.IsNullOrEmpty(search_fields))
            {
                if (search_fields.Contains(field)) return;

                search_fields += ",";
            }

            search_fields += field;
        }
    }

    public class SpotsResult : BaseSearchResult
    {
        public Spot[] spots { get; set; }

        public override string[] Ids
        {
            get
            {
                return spots.Select(x => x.id).ToArray();
            }
        }

        public override List<Attachment> Attachments
        {
            get
            {
                return spots.Select(x => new HeroCard //ThumbnailCard
                {
                    Title = x.name,
                    Subtitle = x.address,
                    Text = x.summary,
                    Images = x.images.Select(y => new CardImage
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
                return spots?.Length > 0;
            }
        }

        public override Geo[] Geos
        {
            get
            {
                return spots.Select(x => new Geo
                {
                    Lon = x.geo.coordinates[0],
                    Lat = x.geo.coordinates[1]
                }).ToArray();
            }
        }
    }

    public class Spot
    {
        public string id { get; set; }
        public string name { get; set; }
        public string ruby { get; set; }
        public string summary { get; set; }
        public Category[] categories { get; set; }
        public string time_remark { get; set; }
        public string closed_remark { get; set; }
        public string address { get; set; }
        public string access { get; set; }
        public string tel_no { get; set; }
        public Area[] area { get; set; }
        public Geo geo { get; set; }
        public Option[] options { get; set; }
        public Image[] images { get; set; }
        public string[] keywords { get; set; }
        public int lang_code { get; set; }
        public int required_time { get; set; }

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

            public bool IsValid()
            {
                return large != null && middle != null && small != null;
            }
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

        public class Option
        {
            public int code { get; set; }
            public string code_name { get; set; }
            public int level { get; set; }
            public string level_name { get; set; }
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

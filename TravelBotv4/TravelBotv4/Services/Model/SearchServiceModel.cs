using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;

namespace TravelBotv4.Services.Model
{
    public interface ISearchRequest
    {
        string QueryString { get; }

        void Modify(BaseSearchResult res);
        void ModifyForNext();
        void ModifyForPrev();
    }

    public interface ISearchResult
    {
        string[] Ids { get; }
        bool IsValid { get; }
        List<Attachment> Attachments { get; }
        Entity ChannelData();
        Geo[] Geos { get; }
    }

    [Serializable]
    public abstract class BaseSearchRequest : ISearchRequest
    {
        public static readonly int DefaultDistanceKiloMeter = 5; //Km
        public double lat { get; set; }
        public double lon { get; set; }
        public int distance { get; set; }

        public abstract string QueryString { get; }

        public abstract void Modify(BaseSearchResult res);
        public abstract void ModifyForNext();
        public abstract void ModifyForPrev();

        public bool IsValidLocation()
        {
            return !double.IsNaN(lat) && !double.IsNaN(lon);
        }

        public BaseSearchRequest()
        {
            distance = DefaultDistanceKiloMeter;
            lat = double.NaN;
            lon = double.NaN;
        }
    }

    [Serializable]
    public abstract class BaseSearchResult : ISearchResult
    {
        public abstract string[] Ids { get; }
        public abstract bool IsValid { get; }
        public abstract List<Attachment> Attachments { get; }
        public abstract Entity ChannelData();
        public abstract Geo[] Geos { get; }
    }

    [Serializable]
    public class Geo
    {
        public float Lon { get; set; }
        public float Lat { get; set; }
    }
}

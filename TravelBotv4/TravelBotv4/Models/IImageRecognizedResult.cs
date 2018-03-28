using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelBotv4.Models
{
    public interface IImageRecognizedResult
    {
        bool IsSure { get; set; }
    }
}

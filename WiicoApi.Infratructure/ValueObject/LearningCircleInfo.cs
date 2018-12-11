using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ValueObject
{
    public class LearningCircleInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("circleKey")]
        public string CircleKey { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("googleDriveFielUrl")]
        public string GoogleDriveFielUrl { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("created")]
        public Property.TimeData Created { get; set; }
        [JsonProperty("createUser")]
        public int? CreateUser { get; set; }
    }
}

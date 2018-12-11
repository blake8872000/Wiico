using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.Entity
{
    public class ExternalResource : Base.EntityBase
    {
        [JsonProperty("name")]
        public override string Name { get; set; }
        [JsonProperty("orgId")]
        public int? OrgId { get; set; }
        [JsonProperty("externalResTypeId")]
        public int? ExternalResTypeId { get; set; }
        //  public virtual ExternalResType ExtResType { get; set; }
        [JsonProperty("uri")]
        public string Uri { get; set; }
        [JsonProperty("uriPath")]
        public string UriPath { get; set; }

        //public string Uri { get; set; }
        //public string UriPath { get; set; }
        [JsonProperty("enable")]
        public bool Enable { get; set; }
        [JsonProperty("disabled")]
        [NotMapped]
        public bool Disabled { get { return !Enable; } }
        [JsonProperty("status")]
        public bool Status { get; set; }
        [JsonProperty("lastModifyUtc")]
        public DateTime? LastModifyUtc { get; set; }
        [JsonProperty("local")]
        [NotMapped, Display(Name = "")]
        public DateTime? Local
        {
            get { return LastModifyUtc?.ToLocalTime(); }
            set { LastModifyUtc = value?.ToUniversalTime(); }
        }
    }
}

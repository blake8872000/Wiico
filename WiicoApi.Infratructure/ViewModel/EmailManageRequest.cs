using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace WiicoApi.Infrastructure.ViewModel
{
    public class EmailManageRequest
    {
      [Required,JsonProperty("fromAddress")]
        public string FromAddress { get; set; }
        [Required, JsonProperty("sendAddress")]
        public List<string> SendAddress { get; set; }
        [Required, JsonProperty("messages")]
        public string Messages { get; set; }
        [Required, JsonProperty("title")]
        public string Title { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WiicoApi.Service.SchoolApi
{
    public class UDollarService
    {
        private readonly string uDollarOtherKey = ConfigurationManager.AppSettings["uDollarOtherKey"].ToString();
        private readonly string uDollarWebService = ConfigurationManager.AppSettings["uDollarWebService"].ToString();

        public string GetuDollarData(string method, string account, bool? needOtherKey = true)
        {
            using (var client = new HttpClient())
            {
                var parameterXMLString = needOtherKey.Value ? string.Format("<soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12 =\"http://www.w3.org/2003/05/soap-envelope\"><soap12:Body><{2} xmlns = \"http://uDollar/\"><strAccountID>{0}</strAccountID><strOther>{1}</strOther></{2}></soap12:Body></soap12:Envelope>",
                        account,
                        uDollarOtherKey,
                        method) :
                        string.Format("<soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12 =\"http://www.w3.org/2003/05/soap-envelope\"><soap12:Body><{1} xmlns = \"http://uDollar/\"><strAccountID>{0}</strAccountID></{1}></soap12:Body></soap12:Envelope>",
                        account,
                        method)
                        ;
                var httpContent = new StringContent(parameterXMLString, Encoding.UTF8, "text/xml");
                var requestUri = string.Format("{0}{1}", uDollarWebService, method);
                var response = client.PostAsync(requestUri, httpContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(responseContent);
                    return doc.InnerText;
                }
                else
                    return null;
            }
        }

        public string GetuDollarDatas(string method, string account, bool? needOtherKey = true)
        {
            using (var client = new HttpClient())
            {
                var parameterXMLString = needOtherKey.Value ? string.Format("<soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12 =\"http://www.w3.org/2003/05/soap-envelope\"><soap12:Body><{2} xmlns = \"http://uDollar/\"><strAccountID>{0}</strAccountID><strOther>{1}</strOther></{2}></soap12:Body></soap12:Envelope>",
                        account,
                        uDollarOtherKey,
                        method) :
                        string.Format("<soap12:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap12 =\"http://www.w3.org/2003/05/soap-envelope\"><soap12:Body><{1} xmlns = \"http://uDollar/\"><strAccountID>{0}</strAccountID></{1}></soap12:Body></soap12:Envelope>",
                        account,
                        method)
                        ;
                var httpContent = new StringContent(parameterXMLString, Encoding.UTF8, "text/xml");
                var requestUri = string.Format("{0}{1}", uDollarWebService, method);
                var response = client.PostAsync(requestUri, httpContent).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(responseContent);
                    var json = JsonConvert.SerializeXmlNode(doc);
                    var obj = JsonConvert.DeserializeObject<object>(json);
                    return json;
                }
                else
                    return null;
            }
        }

    }
}

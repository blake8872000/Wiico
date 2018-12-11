using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WiicoApi.Service.Utility
{
    public class MailService
    {

        //private string emailDomain = ConfigurationManager.AppSettings["MailDomain"].ToString();
        //private string emailSMTPPort =ConfigurationManager.AppSettings["MailSMTPPort"].ToString();
        //private string emailAdminAddress = ConfigurationManager.AppSettings["MailAdminAddress"].ToString();
        private string emailAdminPwd = ConfigurationManager.AppSettings["MailAdminPassword"].ToString();

        public async Task<bool> SendMail(string emailSMTPDomain, int smtpPort, string fromAddress, List<string> sendAddresses, string content, string title, HttpFileCollection files)
        {

            var message = new MailMessage();

            foreach (var sendAddress in sendAddresses)
            {
                message.To.Add(new MailAddress(sendAddress));  // replace with valid value 
            }
            message.From = new MailAddress(fromAddress);  // replace with valid value
            message.Subject = title;
            message.Body = content;
            message.IsBodyHtml = true;
            message.Priority = MailPriority.High;
            if (files != null && files.Count > 0)
            {
                for (var fileIndex = 0; fileIndex < files.Count; fileIndex++)
                {
                    var file = files[fileIndex];
                    message.Attachments.Add(new Attachment(file.InputStream, Path.GetFileName(file.FileName)));
                }
            }
            try
            {
                using (var smtp = new SmtpClient(emailSMTPDomain, smtpPort))
                {
                    var credential = new NetworkCredential
                    {
                        UserName = fromAddress,
                        Password = emailAdminPwd
                        /*   UserName = "yushuchen@g.sce.pccu.edu.tw",
                           Password = "Bls5926125"*/
                    };
                    //var x509 = X509Certificate.CreateFromCertFile(@"C:\OpenSSL\bin\hm.crt");
                    //smtp.ClientCertificates.Add(x509);
                    smtp.EnableSsl = true;
                    smtp.Credentials = credential;
                    //    smtp.Host = emailDomain;
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    // smtp.Port = smtpPort;
                    //smtp.UseDefaultCredentials = false;
                    smtp.Send(message);
                    // smtp.SendCompleted += new SendCompletedEventHandler(client_SendCompleted);
                    smtp.Dispose();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }
    }
}

using AE.Net.Mail;
using DigitalIsraelFund_System.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Hosting;

namespace DigitalIsraelFund_System.DataBase.Managers
{
    public class EmailManager
    {
        private static EmailManager instance = new EmailManager();

        public static EmailManager Manager { get { return instance; } }

        private EmailManager() { }

        public void CheckForNewData()
        {
            Settings sett = Settings.GetSettings();

            string lastUID;
            string lastSeenUID = (long.Parse(sett.LastUIDSeen) + 1).ToString();
            using (ImapClient ic = new ImapClient("imap.gmail.com", "israel.digital.system@gmail.com", "wwe123654789",
                            AuthMethods.Login, 993, true))
            {
                ic.SelectMailbox("INBOX");

                lastUID = ic.GetMessage(ic.GetMessageCount() - 1, true).Uid;

                AE.Net.Mail.MailMessage[] mm = ic.GetMessages(lastUID, lastSeenUID, true, false, false);

                foreach (AE.Net.Mail.MailMessage m in mm)
                {
                    string title = m.Subject;
                    ICollection<AE.Net.Mail.Attachment> attachments = m.Attachments;
                    var request_id = Regex.Match(title, @"[0-9]+").Value;
                    if (request_id != null && request_id != "" && attachments.Count > 0)
                    {
                        // a file for a request
                        foreach (AE.Net.Mail.Attachment att in attachments)
                        {
                            var saveTo = HostingEnvironment.MapPath("~/App_Data/RequestFiles/" + request_id + "_" + att.Filename);
                            att.Save(saveTo);
                            Dictionary<string, string> values = new Dictionary<string, string>();
                            values["file_number"] = request_id;
                            values["path"] = att.Filename;
                            MySqlCommands.Insert("files", values);
                        }
                    }
                    else if ((request_id == null || request_id == "") && attachments.Count == 1
                        && title.Contains("בקשות"))
                    {
                        // a new requests excel
                        var file = new List<AE.Net.Mail.Attachment>(attachments)[0];
                        // save the excel file to app data
                        if (file.Filename.EndsWith(".xls") || file.Filename.EndsWith(".xlsx"))
                        {
                            var saveTo = HostingEnvironment.MapPath("~/App_Data/Excels/" + file.Filename);
                            file.Save(saveTo);
                            // read the excel
                            var table = ExcelManager.Manager.LoadTableFromExcel(saveTo);
                            // attempt to add or update the requests data base with the excel table
                            RequestManager.Manager.AddOrUpdate(table, sett);
                        }
                    }
                }
            }

            if (sett.LastUIDSeen != lastUID)
            {
                sett.LastUIDSeen = lastUID;
                sett.Save();
            }
        }

        public void SendMail(string email, string title, string content)
        {
            var fromAddr = new MailAddress("israel.digital.system@gmail.com");
            var toAddr = new MailAddress(email);

            const string fromPass = "wwe123654789";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddr.Address, fromPass)
            };
            using (var msg = new System.Net.Mail.MailMessage(fromAddr, toAddr)
            {
                Subject = title,
                Body = content
            })
            {
                try
                {
                    smtp.Send(msg);
                }
                catch
                {

                }
            }
        }
    }
}
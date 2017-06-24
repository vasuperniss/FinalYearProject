using AE.Net.Mail;
using DigitalIsraelFund_System.Models;
using System.Collections.Generic;
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
            Settings sett = Settings.LoadJson(System.IO.File.ReadAllText(
                HostingEnvironment.MapPath("~/App_Data/Settings.json")));

            string lastUID;
            string lastSeenUID = (long.Parse(sett.LastUIDSeen) + 1).ToString();
            using (ImapClient ic = new ImapClient("imap.gmail.com", "israel.digital.system@gmail.com", "wwe123654789",
                            AuthMethods.Login, 993, true))
            {
                ic.SelectMailbox("INBOX");

                lastUID = ic.GetMessage(ic.GetMessageCount() - 1, true).Uid;

                MailMessage[] mm = ic.GetMessages(lastUID, lastSeenUID, true, false, false);

                foreach (MailMessage m in mm)
                {
                    string title = m.Subject;
                    ICollection<Attachment> attachments = m.Attachments;
                    var request_id = Regex.Match(title, @"[0-9]+").Value;
                    if (request_id != null && request_id != "" && attachments.Count > 0)
                    {
                        // a file for a request
                        foreach (Attachment att in attachments)
                        {
                            var saveTo = HostingEnvironment.MapPath("~/App_Data/RequestFiles/" + request_id + "_" + att.Filename);
                            att.Save(saveTo);
                            Dictionary<string, string> values = new Dictionary<string, string>();
                            values["file_number"] = request_id;
                            values["path"] = "_" + att.Filename;
                            MySqlCommands.Insert("files", values);
                        }
                    }
                    else if ((request_id == null || request_id == "") && attachments.Count > 1)
                    {
                        // a new requests excel

                    }
                }
            }

            sett.LastUIDSeen = lastUID;
            var json = sett.GetJson();
            System.IO.File.WriteAllText(HostingEnvironment.MapPath("~/App_Data/Settings.json"), json);
        }
    }
}
using ESCS.COMMON.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;

namespace ESCS.COMMON.Common
{
    public class EmailHandler
    {
        public string SendMessage(EmailConfig config, string sendTo, string sendSubject, string sendMessage)
        {
            try
            {
                bool bTest = ValidateEmailAddress(sendTo);
                if (bTest == false)
                    return "Invalid recipient email address: " + sendTo;
                MailMessage message = new MailMessage(
                   config.send_from,
                   sendTo,
                   sendSubject,
                   sendMessage);
                if (!string.IsNullOrEmpty(config.bcc_email))
                {
                    if (config.bcc_email.Contains(","))
                    {
                        string[] arrbcc = config.bcc_email.Split(',');
                        for (int i = 0; i <= arrbcc.Length - 1; i++)
                        {
                            if (!string.IsNullOrEmpty(arrbcc[i].ToString()))
                            {
                                MailAddress bcc = new MailAddress(arrbcc[i].ToString());
                                message.Bcc.Add(bcc);
                            }
                        }
                    }
                    else
                    {
                        message.Bcc.Add(config.bcc_email);
                    }
                }
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient(config.smtp_server, config.smtp_port);
                client.EnableSsl = true;
                client.Credentials = new System.Net.NetworkCredential(config.send_from, config.pass_mail);
                client.Send(message);
                return "Message sent to " + sendTo + " at " + DateTime.Now.ToString() + ".";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public string SendMessageWithNoBCC(EmailConfig config, string sendTo, string sendSubject, string sendMessage)
        {
            try
            {
                bool bTest = ValidateEmailAddress(sendTo);
                if (bTest == false)
                    return "Invalid recipient email address: " + sendTo;
                MailMessage message = new MailMessage(
                   config.send_from,
                   sendTo,
                   sendSubject,
                   sendMessage);
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient(config.smtp_server, config.smtp_port);
                client.Credentials = new System.Net.NetworkCredential(config.send_from, config.pass_mail);
                client.Send(message);
                return "Message sent to " + sendTo + " at " + DateTime.Now.ToString() + ".";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public string SendMessageWithAttachment(EmailConfig config, string sendTo, string sendSubject, string sendMessage, ArrayList attachments, List<LinkedResource> linkSource = null)
        {
            try
            {
                bool bTest = ValidateEmailAddress(sendTo);
                if (bTest == false)
                    return "Invalid recipient email address: " + sendTo;

                MailAddress from = new MailAddress(config.send_from, config.display_name);
                MailAddress to = new MailAddress(sendTo);
                // Create the basic message
                MailMessage message = new MailMessage(from,to);
                message.Subject = sendSubject;
                //message.Body = sendMessage;
                if (!string.IsNullOrEmpty(config.bcc_email))
                {
                    if (config.bcc_email.Contains(","))
                    {
                        string[] arrbcc = config.bcc_email.Split(',');
                        for (int i = 0; i <= arrbcc.Length - 1; i++)
                        {
                            if (!string.IsNullOrEmpty(arrbcc[i].ToString()))
                            {
                                MailAddress bcc = new MailAddress(arrbcc[i].ToString());
                                message.Bcc.Add(bcc);
                            }
                        }

                    }
                    else
                    {
                        message.Bcc.Add(config.bcc_email);
                    }
                }
                message.IsBodyHtml = true;
                AlternateView alternateView = AlternateView.CreateAlternateViewFromString(sendMessage, null, MediaTypeNames.Text.Html);
                if (linkSource!= null && linkSource.Count >0)
                {
                    foreach (var item in linkSource)
                    {
                        alternateView.LinkedResources.Add(item);
                    }
                }
                message.AlternateViews.Add(alternateView);

                foreach (string attach in attachments)
                {
                    Attachment attached = new Attachment(attach, MediaTypeNames.Application.Octet);
                    message.Attachments.Add(attached);
                }
                SmtpClient client = new SmtpClient(config.smtp_server, config.smtp_port);
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(config.send_from, config.pass_mail);
                client.EnableSsl = true;
                client.Send(message);
                return "SUCCESS";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public string SendMessageWithAttachmentConfig(EmailConfig config, string sendSubject, string sendMessage, List<System.Net.Mail.Attachment> attachments, List<LinkedResource> linkSource = null)
        {
            try
            {
                MailAddress from = new MailAddress(config.send_from, config.display_name);
                MailMessage message = new MailMessage();
                message.From = from;
                foreach (var mail_to in config.to)
                {
                    bool bTest = ValidateEmailAddress(mail_to.username);
                    if (bTest == false)
                        return "Invalid recipient email address:"+ mail_to.username;

                    message.To.Add(mail_to.username);
                }
                if (config.cc != null)
                {
                    foreach (var mail_cc in config.cc)
                    {
                        message.CC.Add(mail_cc.username);
                    }
                }
                if (config.bcc!=null)
                {
                    foreach (var mail_bcc in config.bcc)
                    {
                        message.Bcc.Add(mail_bcc.username);
                    }
                }
                message.Subject = sendSubject;
                message.IsBodyHtml = true;
                message.Body = sendMessage;
                foreach (var attach in attachments)
                {
                    message.Attachments.Add(attach);
                }
                SmtpClient client = new SmtpClient(config.smtp_server, config.smtp_port);
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(config.send_from, config.pass_mail);
                client.EnableSsl = true;
                client.Send(message);
                return "SUCCESS";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public string SendMessageWithBCC(EmailConfig config, string sendTo, string sendSubject, string sendMessage)
        {
            try
            {
                bool bTest = ValidateEmailAddress(sendTo);
                if (bTest == false)
                    return "Invalid recipient email address: " + sendTo;
                MailMessage message = new MailMessage(
                   config.send_from,
                   sendTo,
                   sendSubject,
                   sendMessage);
                if (config.bcc_email.Contains(","))
                {
                    string[] arrbcc = config.bcc_email.Split(',');
                    for (int i = 0; i <= arrbcc.Length - 1; i++)
                    {
                        if (!string.IsNullOrEmpty(arrbcc[i].ToString()))
                        {
                            MailAddress bcc = new MailAddress(arrbcc[i].ToString());
                            message.Bcc.Add(bcc);
                        }
                    }
                }
                else
                {
                    message.Bcc.Add(config.bcc_email);
                }
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient(config.smtp_server, config.smtp_port);
                client.EnableSsl = true;
                client.Credentials = new System.Net.NetworkCredential(config.send_from, config.pass_mail);
                client.Send(message);
                return "Message sent to " + sendTo + " at " + DateTime.Now.ToString() + ".";
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public static bool ValidateEmailAddress(string emailAddress)
        {
            try
            {
                string TextToValidate = emailAddress;
                Regex expression = new Regex(@"\w+@[a-zA-Z_]+?\.[a-zA-Z]{2,3}");
                if (expression.IsMatch(TextToValidate))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
    public class EmailConfig
    {
        public string smtp_server { get; set; }
        public int smtp_port { get; set; }
        public string display_name { get; set; }


        public string is_email_local { get; set; }
        public string send_from { get; set; }
        public string pass_mail { get; set; }
        public string bcc_email { get; set; }
        

        public List<MailInfo> to { get; set; }
        public List<MailInfo> cc { get; set; }
        public List<MailInfo> bcc { get; set; }
    }
}

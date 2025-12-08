using NT_AirPollution.Model.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace NT_AirPollution.MailTask
{
    public class MailHelper
    {
        /// <summary>
        /// 寄送郵件
        /// </summary>
        /// <param name="mail"></param>
        public void SendMail(MailModel mail)
        {
            string strMailFrom = "recycle@chuangjing.com.tw";
            string strMailFromShowName = "南投縣政府環境保護局";
            string strSmtpServer = "smtp.gmail.com";
            string strAccount = "recycle@chuangjing.com.tw";
            string strPassword = "vbqw ryow uxld eidk";
            int intPort = 587;

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(strMailFrom, strMailFromShowName);
            foreach (var item in mail.To)
            {
                msg.To.Add(item);
            }

            //郵件標題 
            msg.Subject = mail.Subject;
            //郵件內容
            msg.Body = mail.Body;
            // 郵件附檔
            if (mail.Attachment != null && mail.Attachment.Count() > 0)
            {
                foreach (var item in mail.Attachment)
                {
                    System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(item);
                    msg.Attachments.Add(attachment);
                }
            }
            msg.IsBodyHtml = true;

            SmtpClient MySmtp = new SmtpClient(strSmtpServer, intPort);
            //設定你的帳號密碼
            MySmtp.Credentials = new NetworkCredential(strAccount, strPassword);
            //Gmial 的 smtp 使用 SSL
            MySmtp.EnableSsl = true;
            MySmtp.Send(msg);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.ComponentModel;
using fastCSharp.threading;

namespace fastCSharp.net
{
    /// <summary>
    /// SMTP与用户信息
    /// </summary>
    public struct email
    {
        /// <summary>
        /// SMTP信息
        /// </summary>
        public struct smtp
        {
            /// <summary>
            /// 默认SMTP服务端口
            /// </summary>
            public const int DefaultServerPort = 25;

            /// <summary>
            /// 发件SMTP,如"smtp.163.com"
            /// </summary>
            public string Server;
            /// <summary>
            /// SMTP服务端口
            /// </summary>
            public int Port;
            /// <summary>
            /// 是否SSL
            /// </summary>
            public bool IsSsl;
        }
        /// <summary>
        /// 发送邮件信息
        /// </summary>
        public sealed class content
        {
            /// <summary>
            /// 收件人邮箱
            /// </summary>
            public string SendTo;
            /// <summary>
            /// 邮件主题
            /// </summary>
            public string Subject;
            /// <summary>
            /// 邮件内容
            /// </summary>
            public string Body;
            /// <summary>
            /// 邮件内容是否HTML代码
            /// </summary>
            public bool IsHtml;
            /// <summary>
            /// 附件文件名集合
            /// </summary>
            public string[] Attachments;
        }
        /// <summary>
        /// 发件人邮箱
        /// </summary>
        public string From;
        /// <summary>
        /// 发件人密码
        /// </summary>
        public string Password;
        /// <summary>
        /// SMTP信息
        /// </summary>
        public smtp Smtp;
        /// <summary>
        /// 简单检测邮件信息
        /// </summary>
        /// <param name="content">邮件信息</param>
        /// <returns>是否合法</returns>
        private bool check(content content)
        {
            return content != null && content.Subject != null && content.Body != null && content.SendTo != null
                && From != null && Password != null && Smtp.Server != null
                && content.Subject.Length != 0 && content.Body.Length != 0
                && Password.Length != 0 && Smtp.Server.Length != 0
                && content.SendTo.IndexOf('@') > 0 && From.IndexOf('@') > 0;
        }
        /// <summary>
        /// 获取STMP客户端
        /// </summary>
        /// <param name="message">邮件信息</param>
        /// <param name="content">邮件信息</param>
        /// <returns>STMP客户端</returns>
        private SmtpClient getSmtp(MailMessage message, content content)
        {
            message.IsBodyHtml = content.IsHtml;
            //send.ReplyTo = new MailAddress(from, "我的接收邮箱", Encoding.GetEncoding(936));//对方回复邮件时默认的接收地址
            //send.CC.Add("a@163.com,b@163.com,c@163.com");//抄送者
            //send.Bcc.Add("a@163.com,b@163.com,c@163.com");//密送者
            //send.SubjectEncoding = Encoding.GetEncoding(936);
            //send.BodyEncoding = Encoding.GetEncoding(936);
            //send.Headers.Add("Disposition-Notification-To", from);//要求回执的标志
            //send.Headers.Add("X-Website", "http://www.showjim.com/");//自定义邮件头
            //send.Headers.Add("ReturnReceipt", "1");//针对 LOTUS DOMINO SERVER，插入回执头
            //send.Priority = MailPriority.Normal;//优先级
            message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(content.Body, null, "text/plain"));//普通文本邮件内容，如果对方的收件客户端不支持HTML，这是必需的
            #region 嵌入图片资源
            //AlternateView htmlBody = AlternateView.CreateAlternateViewFromString(@"<img src=""cid:weblogo"">", null, "text/html");
            //LinkedResource lrImage = new LinkedResource(@"d:\logo.gif", "image/gif");
            //lrImage.ContentId = "weblogo";
            //htmlBody.LinkedResources.Add(lrImage);
            //send.AlternateViews.Add(htmlBody);
            #endregion
            if (content.Attachments != null)
            {
                foreach (string fileName in content.Attachments) message.Attachments.Add(new Attachment(fileName));
            }
            SmtpClient smtpClient = new SmtpClient(Smtp.Server, Smtp.Port);
            if (Smtp.IsSsl) smtpClient.EnableSsl = true;
            //smtpClient.Credentials = CredentialCache.DefaultNetworkCredentials;
            //smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Credentials = new NetworkCredential(From, Password);
            return smtpClient;
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="content">电子邮件内容</param>
        /// <returns>邮件是否发送成功</returns>
        public bool Send(content content)
        {
            bool isSend = false;
            if (check(content))
            {
                using (MailMessage message = new MailMessage(From, content.SendTo, content.Subject, content.Body))
                {
                    try
                    {
                        getSmtp(message, content).Send(message);
                        isSend = true;
                    }
                    catch (Exception error)
                    {
                        log.Default.Add(error, "邮件发送失败 : " + content.SendTo, false);
                    }
                }
            }
            return isSend;
        }
        /// <summary>
        /// 电子邮件发送器
        /// </summary>
        private sealed class sender : threading.callbackPool<sender, bool>
        {
            /// <summary>
            /// 邮件信息
            /// </summary>
            private MailMessage message;
            ///// <summary>
            ///// STMP客户端
            ///// </summary>
            //private SmtpClient smtpClient;
            /// <summary>
            /// 邮件发送回调
            /// </summary>
            public SendCompletedEventHandler OnSend;
            /// <summary>
            /// 电子邮件发送器
            /// </summary>
            public sender()
            {
                OnSend = send;
            }
            /// <summary>
            /// 邮件发送回调
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void send(object sender, AsyncCompletedEventArgs e)
            {
                try
                {
                    //smtpClient = null;
                    pub.Dispose(ref message);
                    if (e.Error != null) log.Default.Add(e.Error, null, true);
                }
                finally
                {
                    push(this, e.Error == null);
                }
            }
            /// <summary>
            /// 获取电子邮件发送器
            /// </summary>
            /// <param name="message">邮件信息</param>
            /// <param name="smtpClient">STMP客户端</param>
            /// <param name="onSend">邮件发送回调</param>
            /// <returns>电子邮件发送器</returns>
            public void Send(MailMessage message, SmtpClient smtpClient, Action<bool> onSend)
            {
                this.message = message;
                //this.smtpClient = smtpClient;
                Callback = onSend;
                smtpClient.SendCompleted += new SendCompletedEventHandler(OnSend);
                smtpClient.SendAsync(message, this);
            }
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="content">电子邮件内容</param>
        /// <param name="onSend">邮件发送回调</param>
        public void Send(content content, Action<bool> onSend)
        {
            if (check(content))
            {
                MailMessage message = null;
                try
                {
                    message = new MailMessage(From, content.SendTo, content.Subject, content.Body);
                    message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;//如果发送失败，SMTP 服务器将发送 失败邮件告诉我
                    (typePool<sender>.Pop() ?? new sender()).Send(message, getSmtp(message, content), onSend);
                    return;
                }
                catch (Exception error)
                {
                    log.Default.Add(error, "邮件发送失败 : " + content.SendTo, false);
                }
                pub.Dispose(ref message);
            }
            if (onSend != null) onSend(false);
        }
    }
}

using TrueSight.Common;
using TrueCareer.Helpers;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TrueCareer.Config;
using TrueCareer.Entities;
using TrueCareer.Repositories;
using System.Net;
using MailKit.Net.Imap;
using MailKit;

namespace Utils.Service
{
    public interface IMailService : IServiceScoped
    {
        Task Authenticate(Mail Mail);
        Task Create(Mail mail);
        Task Send(Mail mail);
        Task ReSend();
    }
    public class MailService : IMailService
    {
        private readonly EmailConfig emailConfig;
        private readonly IUOW UOW;
        private readonly IFileService FileService;
        private ILogging Logging;
        public MailService(EmailConfig _emailConfig, IUOW UOW, IFileService FileService, ILogging Logging)
        {
            emailConfig = _emailConfig;
            this.UOW = UOW;
            this.FileService = FileService;
            this.Logging = Logging;
        }

        public async Task Authenticate(Mail Mail)
        {
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(emailConfig.SmtpServer, emailConfig.Port, SecureSocketOptions.StartTls);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                if (string.IsNullOrWhiteSpace(Mail.Username) == false || string.IsNullOrWhiteSpace(Mail.Password) == false)
                    await client.AuthenticateAsync(Mail.Username, Mail.Password);
                await client.DisconnectAsync(true);
                client.Dispose();
            }
        }

        public async Task Create(Mail mail)
        {
            try
            {
                foreach (Attachment attachment in mail.Attachments)
                {
                    Entities.File File = new Entities.File
                    {
                        Path = $"/attachment/{StaticParams.DateTimeNow.ToString("yyyyMMdd")}/{Guid.NewGuid()}{attachment.Extension}",
                        Content = attachment.Content,
                    };
                    File = await FileService.Create(File);
                    attachment.Url = File.Path;
                }
                await UOW.MailRepository.Create(mail);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                    throw new MessageException(ex);
                else
                    throw new MessageException(ex.InnerException);
            }
        }

        public async Task Send(Mail mail)
        {
            var mailMessage = await CreateMail(mail);
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(emailConfig.SmtpServer, emailConfig.Port, SecureSocketOptions.StartTls);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    if (string.IsNullOrWhiteSpace(mail.Username) == false || string.IsNullOrWhiteSpace(mail.Password) == false)
                        await client.AuthenticateAsync(mail.Username, mail.Password);
                    else
                        await client.AuthenticateAsync(emailConfig.UserName, emailConfig.Password);
                    await client.SendAsync(mailMessage);
                }
                catch
                {
                    try
                    {
                        await UOW.MailRepository.Create(mail);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }

        public async Task ReSend()
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    List<Mail> mails = await UOW.MailRepository.List(new MailFilter
                    {
                        Skip = 0,
                        Take = 10,
                        RetryCount = new LongFilter { LessEqual = StaticParams.RetryCount },
                    });

                    var removeIds = mails.Where(x => x.Recipients == null || x.Recipients.Count == 0 || x.Recipients.Contains(null))
                        .Select(x => x.Id)
                        .ToList();

                    if (removeIds.Count > 0)
                        await UOW.MailRepository.BulkDelete(removeIds);

                    var sendMails = mails.Where(x => !removeIds.Contains(x.Id)).ToList();
                    if (sendMails.Count > 0)
                    {
                        await client.ConnectAsync(emailConfig.SmtpServer, emailConfig.Port, SecureSocketOptions.StartTlsWhenAvailable);
                        client.AuthenticationMechanisms.Remove("XOAUTH2");
                        await client.AuthenticateAsync(emailConfig.UserName, emailConfig.Password);

                        foreach (var mail in mails)
                        {
                            try
                            {
                                if (!string.IsNullOrWhiteSpace(mail.Username) && !string.IsNullOrWhiteSpace(mail.Password))
                                {
                                    await client.DisconnectAsync(true);
                                    await client.ConnectAsync(emailConfig.SmtpServer, emailConfig.Port, SecureSocketOptions.StartTlsWhenAvailable);
                                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                                    await client.AuthenticateAsync(mail.Username, mail.Password);
                                }

                                var mailMessage = await CreateMail(mail);
                                await client.SendAsync(mailMessage);
                                await UOW.MailRepository.Delete(mail.Id);
                            }
                            catch (Exception ex)
                            {
                                mail.RetryCount++;
                                mail.Error = ex.Message;
                                try
                                {
                                    await UOW.MailRepository.Update(mail);
                                }
                                catch { }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.CreateSystemLog(ex, nameof(MailService));
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }

        private async Task<MimeMessage> CreateMail(Mail mail)
        {
            var mailMessage = new MimeMessage();
            if(!string.IsNullOrWhiteSpace(emailConfig.DisplayName))
                mailMessage.From.Add(new MailboxAddress(emailConfig.DisplayName, emailConfig.From));
            else
                mailMessage.From.Add(new MailboxAddress(emailConfig.From, emailConfig.From));

            if (mail.Recipients != null)
                foreach (string recipient in mail.Recipients)
                {
                    mailMessage.To.Add(new MailboxAddress(recipient, recipient));
                }
            if (mail.BccRecipients != null)
                foreach (string recipient in mail.BccRecipients)
                {
                    mailMessage.Bcc.Add(new MailboxAddress(recipient, recipient));
                }
            if (mail.CcRecipients != null)
                foreach (string recipient in mail.CcRecipients)
                {
                    mailMessage.Cc.Add(new MailboxAddress(recipient, recipient));
                }

            mailMessage.Subject = mail.Subject;
            var bodyBuilder = new BodyBuilder { HtmlBody = mail.Body };

            if (mail.Attachments != null)
            {
                foreach (var attachment in mail.Attachments)
                {
                    string url = attachment.Url;
                    url = url.Replace("/rpc/utils/file/download", "");

                    Entities.File file = (await FileService.List(new FileFilter
                    {
                        Path = new StringFilter { Equal = url },
                    })).FirstOrDefault();

                    if (file != null)
                    {
                        file = await FileService.Download(file.Id);
                        attachment.Content = file.Content;
                        bodyBuilder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(file.MimeType));
                    }
                }
            }
            mailMessage.Body = bodyBuilder.ToMessageBody();
            return mailMessage;
        }

    }
}

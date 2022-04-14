// using Utils.Entities;
// using Utils.Service;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using MimeKit;
// using System.IO;
// using TrueSight.Common;
// using Google.Cloud.PubSub.V1;
// using System.Threading;
// using TrueCareer.Models;
// using Newtonsoft.Json;
// using TrueCareer.Helpers;
// using Google.Apis.Gmail.v1;
// using Google.Apis.Gmail.v1.Data;
// using MailKit.Net.Imap;
// using MailKit;
// using MailKit.Net.Pop3;
// // using TrueCareer.Services.MEmailConfiguration;

// namespace TrueCareer.Rpc.mail
// {
//     public class MailController : RpcController
//     {
//         private readonly Service.IMailService MailService;
//         private readonly IEmailConfigurationService EmailConfigurationService;
//         private readonly DataContext DataContext;
//         public MailController(
//             Service.IMailService MailService,
//             IEmailConfigurationService EmailConfigurationService,
//             DataContext DataContext)
//         {
//             this.MailService = MailService;
//             this.EmailConfigurationService = EmailConfigurationService;
//             this.DataContext = DataContext;
//         }

//         [Route(MailRoute.Authenticate), HttpPost]
//         public async Task<ActionResult> Authenticate([FromBody] Mail_MailDTO MailDTO)
//         {
//             if (!ModelState.IsValid)
//                 throw new BindException(ModelState);

//             var Mail = new Mail();
//             Mail.Username = MailDTO.Username;
//             Mail.Password = MailDTO.Password;

//             try
//             {
//                 await MailService.Authenticate(Mail);
//                 return Ok();
//             }
//             catch (Exception ex)
//             {
//                 if (ex.InnerException != null)
//                     ex = ex.InnerException;
//                 return BadRequest(ex.ToString());
//             }
//         }

//         [Route(MailRoute.Create), HttpPost]
//         public async Task<bool> Create([FromForm] Mail_MailDTO MailDTO)
//         {
//             if (!ModelState.IsValid)
//                 throw new BindException(ModelState);

//             var files = Request.Form.Files.Any() ? Request.Form.Files : new FormFileCollection();

//             var Mail = new Mail();
//             Mail.Recipients = MailDTO.Recipients;
//             Mail.Subject = MailDTO.Subject;
//             Mail.Body = MailDTO.Content;
//             Mail.Attachments = new List<Attachment>();
//             foreach (var file in files)
//             {
//                 MemoryStream stream = new MemoryStream();
//                 file.CopyTo(stream);

//                 Attachment Attachment = new Attachment
//                 {
//                     Content = stream.ToArray(),
//                     FileName = file.FileName,
//                     ContentType = file.ContentType,
//                 };
//                 Mail.Attachments.Add(Attachment);
//             }
//             await MailService.Create(Mail);

//             return true;
//         }

//         [Route(MailRoute.Resend), HttpPost]
//         public async Task<bool> ReSend()
//         {
//             if (!ModelState.IsValid)
//                 throw new BindException(ModelState);
//             await MailService.ReSend();

//             return true;
//         }

//     }
// }

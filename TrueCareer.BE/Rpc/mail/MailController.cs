using TrueCareer.Entities;
using TrueCareer.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using System.IO;
using TrueSight.Common;
using Google.Cloud.PubSub.V1;
using System.Threading;
using TrueCareer.BE.Models;
using Newtonsoft.Json;
using TrueCareer.Helpers;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using MailKit.Net.Imap;
using MailKit;
using MailKit.Net.Pop3;
// using TrueCareer.Services.MEmailConfiguration;

namespace TrueCareer.Rpc.mail
{
    public class MailController : RpcController
    {
        private readonly Service.IMailService MailService;
        
        private readonly DataContext DataContext;
        public MailController(
            Service.IMailService MailService,
            
            DataContext DataContext)
        {
            this.MailService = MailService;

            this.DataContext = DataContext;
        }

        [Route(MailRoute.Authenticate), HttpPost]
        public async Task<ActionResult> Authenticate([FromBody] Mail_MailDTO MailDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var Mail = new Mail();
            Mail.Username = MailDTO.Username;
            Mail.Password = MailDTO.Password;

            try
            {
                await MailService.Authenticate(Mail);
                return Ok();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    ex = ex.InnerException;
                return BadRequest(ex.ToString());
            }
        }

        [Route(MailRoute.Create), HttpPost]
        public async Task<bool> Create([FromForm] Mail_MailDTO MailDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var Mail = new Mail();
            Mail.Recipients = MailDTO.Recipients;
            Mail.Subject = MailDTO.Subject;
            Mail.Body = MailDTO.Content;

            await MailService.Create(Mail);

            return true;
        }

        [Route(MailRoute.Resend), HttpPost]
        public async Task<bool> ReSend()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            await MailService.ReSend();

            return true;
        }

    }
}

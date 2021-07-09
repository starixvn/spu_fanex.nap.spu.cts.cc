namespace Fanex.NAP.SPU.CTS.Features.Evidence.CustomerEvidenceInsert
{
    using Fanex.MailDelivery.Client;
    using Fanex.MailDelivery.Core.Models;
    using Fanex.NAP.SPU.CTS.Shares;
    using Sentry;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface ICustomerEvidenceSendMailServices
    {
        void HandleLogError(IEnumerable<CustomerEvidenceFailedToProcessEntity> processErrorList, bool isSendMailToSPU, string envỉonmentName, int mailingToSPU);
    }

    public class CustomerEvidenceSendMailServices : ICustomerEvidenceSendMailServices
    {
        private readonly ISentryClient sentryClient;
        private const string mailInformTemplate = @"MailTemplate\MailInformTemplate.cshtml";

        public CustomerEvidenceSendMailServices(ISentryClient sentryClient)
        {
            this.sentryClient = sentryClient;
        }

        public void HandleLogError(IEnumerable<CustomerEvidenceFailedToProcessEntity> processErrorList, bool isSendMailToSPU, string envỉonmentName, int mailingToSPU)
        {
            if (processErrorList.Any())
            {
                Utilities.LogSentryError("CustomerEvidenceInsert", "Insert Evidence failed", processErrorList, new Exception(), sentryClient);

                if (isSendMailToSPU)
                {
                    var table = "";
                    table += "<table style='" + CssStyleTable(400) + "'>";
                    table += "<tr>";
                    table += "<td style='" + CssStyleTh() + "'>Key</td>";
                    table += "<td style='" + CssStyleTh() + "'>CTSCustID</td>";
                    table += "<td style='" + CssStyleTh() + "'>EvidenceID</td>";
                    table += "<td style='" + CssStyleTh() + "'>ErrorContent</td>";
                    table += "</tr>";
                    var index = 1;

                    foreach (var item in processErrorList)
                    {
                        table += "<tr>";
                        table += "<td style='" + CssStyleTd() + "'>" + item.FileKey + "</td>";
                        table += "<td style='" + CssStyleTd() + "'>" + item.CTSCustID + "</td>";
                        table += "<td style='" + CssStyleTd() + "'>" + item.EvidenceID + "</td>";
                        table += "<td style='" + CssStyleTd() + "'>" + item.ErrorContent + "</td>";
                        table += "</tr>";

                        index++;

                        if (index == 100)
                            break;
                    }

                    table += "</table>";

                    var body = GenerateMailBody(table);

                    SendMailInternal(10278938, body, envỉonmentName, mailingToSPU);
                }
            }
        }

        private static string CssStyleTable(int width)
        {
            var css = "border-collapse: collapse;";
            css += "width: " + width + "px";

            return css;
        }

        private static string CssStyleTh()
        {
            var css = "border: 1px solid black;";
            css += "font-weight:bold;";

            return css;
        }

        private static string CssStyleTd()
        {
            var css = "border: 1px solid black";

            return css;
        }

        public string GenerateMailBody(string errContent)
        {
            try
            {
                var mailTemplatePath = AppDomain.CurrentDomain.BaseDirectory + mailInformTemplate;

                var body = Utilities.ReadTextFile(mailTemplatePath);

                return string.Format(body, errContent);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("CustomerEvidenceInsert", "Send Mail - GenerateMailBody failed", errContent, ex, sentryClient);
            }

            return string.Empty;
        }

        public void SendMailInternal(int usertId, string body, string envỉontmentName, int mailingToSPU)
        {
            var subject = $"CTS service Customer-Evidence failed to process ({ envỉontmentName }), UserId={ usertId } at { DateTime.Now }";

            try
            {
                var mail = new Mail
                {
                    Subject = subject,
                    Title = subject,
                    Body = body,
                    MailingListId = mailingToSPU,
                    Attachments = new List<MailAttachment>()
                };

                SendMail(mail);
            }
            catch (Exception ex)
            {
                Utilities.LogSentryError("CustomerEvidenceInsert", "Send Mail - SendMailInternal failed", string.Empty, ex, sentryClient);
            }
        }

        public static void SendMail(Mail mail)
        {
            MailClientHandler.Start();

            MailClientHandler.SendFunctionMail(mail);

            MailClientHandler.Stop();
        }
    }
}
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace MyPortfolio.Controllers
{
    public class ContactController : Controller
    {
        // GET: ContactController
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendMail()
        {
            try
            {
                MailMessage mailMessage = new MailMessage();

                mailMessage.From = new MailAddress("보내는 계정 주소", "표시 이름", System.Text.Encoding.UTF8);
                // 받는이 메일 주소
                mailMessage.To.Add("yyy@naver.com");
                // 참조 메일 주소
                mailMessage.CC.Add("zzz@naver.com");
                // 비공개 참조 메일 주소
                mailMessage.Bcc.Add("kkk@naver.com");
                // 제목
                mailMessage.Subject = "메일 제목";
                // 메일 제목 인코딩 타입(UTF-8) 선택
                mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
                // 본문
                mailMessage.Body = "메일 본문";
                // 본문의 포맷에 따라 선택
                mailMessage.IsBodyHtml = false;
                // 본문 인코딩 타입(UTF-8) 선택
                mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                // 파일 첨부
                mailMessage.Attachments.Add(new Attachment(new FileStream(@"D:\test.zip", FileMode.Open, FileAccess.Read), "test.zip"));
                // SMTP 서버 주소
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                // SMTP 포트
                SmtpServer.Port = 587;
                // SSL 사용 여부
                SmtpServer.EnableSsl = true;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                SmtpServer.Credentials = new System.Net.NetworkCredential("아이디", "패스워드");

                SmtpServer.Send(mailMessage);
            }
            catch (Exception ex)
            {
                // 에러메시지 HTML
            }

            return RedirectToAction("Index", "Home");
        }
    }
}

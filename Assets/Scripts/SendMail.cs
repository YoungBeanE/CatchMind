using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class SendMail : MonoBehaviour
{
    public void Send(string email, int PACode)
    {
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress("dldudqls7733@naver.com");
        mail.To.Add(email);
        mail.Subject = "[CatchMind] Sending the authentication Code";

        string mailBody = string.Format("Oh, you forgot your password."
                + Environment.NewLine + "Enter the authentication number and check your password."
                + Environment.NewLine + "Good luck." + Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine
                + Environment.NewLine + "[ Authentication Number ] : " + "{0}", PACode);
        mail.Body = mailBody;

        //SMTP - simple 메일 전용 프로토콜(사용자의 IMAP/SMTP계정에서 사용을 허용해두어야 함)
        SmtpClient smtpServer = new SmtpClient("smtp.naver.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new System.Net.NetworkCredential("dldudqls7733@naver.com", "qlqjs") as ICredentialsByHost; //자격확인
        smtpServer.EnableSsl = true; //SSL - 교환할 데이터를 암호화하여 이용하는 통신 프로토콜(인터넷 메시지 전송의 보안을 관리하는 표준)
        ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        };
        smtpServer.Send(mail);
    }
    //ServicePointManager.ServerCertificateValidationCallback - 인증에 사용되는 원격 SSL(Secure Sockets Layer) 인증서를 확인함.
    //반환형식bool - 지정된 인증서를 인증에 사용할 수 있는지 여부를 반환
    //object sender - 유효성 검사에 대한 상태 정보가 들어 있는 개체
    //X509Certificate certificate - 원격측을 인증하는 데 사용되는 인증서
    //X509Chain chain - 원격 인증서와 연결된 인증 기관의 체인
    //SslPolicyErrors sslPolicyErrors - 원격 인증서와 관련된 오류
    //유효성 검사 오류가 있는 경우 오류를 표시하고 false를 반환하여 인증되지 않은 서버와의 통신을 방지할 수 있다.
}

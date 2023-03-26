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

        //SMTP - simple ���� ���� ��������(������� IMAP/SMTP�������� ����� ����صξ�� ��)
        SmtpClient smtpServer = new SmtpClient("smtp.naver.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new System.Net.NetworkCredential("dldudqls7733@naver.com", "spdlqj26!!") as ICredentialsByHost; //�ڰ�Ȯ��
        smtpServer.EnableSsl = true; //SSL - ��ȯ�� �����͸� ��ȣȭ�Ͽ� �̿��ϴ� ��� ��������(���ͳ� �޽��� ������ ������ �����ϴ� ǥ��)
        ServicePointManager.ServerCertificateValidationCallback = delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        };
        smtpServer.Send(mail);
    }
    //ServicePointManager.ServerCertificateValidationCallback - ������ ���Ǵ� ���� SSL(Secure Sockets Layer) �������� Ȯ����.
    //��ȯ����bool - ������ �������� ������ ����� �� �ִ��� ���θ� ��ȯ
    //object sender - ��ȿ�� �˻翡 ���� ���� ������ ��� �ִ� ��ü
    //X509Certificate certificate - �������� �����ϴ� �� ���Ǵ� ������
    //X509Chain chain - ���� �������� ����� ���� ����� ü��
    //SslPolicyErrors sslPolicyErrors - ���� �������� ���õ� ����
    //��ȿ�� �˻� ������ �ִ� ��� ������ ǥ���ϰ� false�� ��ȯ�Ͽ� �������� ���� �������� ����� ������ �� �ִ�.
}

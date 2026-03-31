using MailKit.Net.Smtp;
using MailKit.Security;
using System;

namespace SutesFozes.Infrastructure;

public class Mail {
        
        public SmtpClient CreateSMTPClient()
        {
                var client = new SmtpClient();

                string hostname = Environment.GetEnvironmentVariable("SMTP_HOSTNAME");
                int hostport = Convert.ToInt32(Environment.GetEnvironmentVariable("SMTP_HOSTPORT"));

                string username = Environment.GetEnvironmentVariable("SMTP_USERNAME");
                string password = Environment.GetEnvironmentVariable("SMTP_PASSWORD");


                client.Connect(hostname, hostport, SecureSocketOptions.SslOnConnect);
                client.Authenticate(username, password);

                return client;
        }
}
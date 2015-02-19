using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Caroline.Persistence.Redis
{
    class LogUtility
    {
        public static async Task LogMessage(string message)
        {
            var mail = new MailMessage("tristynstimpson@gmail.com", "tristynstimpson@gmail.com", "Log", message);
            var googleIp = await Dns.GetHostAddressesAsync("pop.gmail.com");
            var client = new SmtpClient(googleIp.First().ToString(), 995);
            await client.SendMailAsync(mail);
        }
    }
}

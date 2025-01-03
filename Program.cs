using System;
using System.Collections.Generic;

namespace SimplifiedKerberos
{
    class Program
    {
        // baza de date pt utilizatori: { username, password }
        static Dictionary<string, string> userDatabase = new Dictionary<string, string>
        {
            { "user1", "password1" }
        };

        class TicketGrantingTicket
        {
            public string UserName { get; set; }
            public DateTime Expiry { get; set; }
        }

        class ServiceTicket
        {
            public string UserName { get; set; }
            public string ServiceName { get; set; }
            public DateTime Expiry { get; set; }
        }

        static TicketGrantingTicket AuthenticationServer(string username, string password)
        {
            if (userDatabase.ContainsKey(username) && userDatabase[username] == password)
            {
                Console.WriteLine("AS: Autentificare reușită.");
                return new TicketGrantingTicket
                {
                    UserName = username,
                    // Expiry = DateTime.Now.AddSeconds(5)
                    Expiry = DateTime.Now.AddMinutes(2)
                };
            }
            else
            {
                Console.WriteLine("AS: Autentificare eșuată.");
                return null;
            }
        }

        // serverul pentru acordarea biletelor de serviciu
        static ServiceTicket TicketGrantingServer(TicketGrantingTicket tgt, string serviceName)
        {
            if (tgt != null && tgt.Expiry > DateTime.Now)
            {
                Console.WriteLine("TGS: Bilet acordat pentru serviciul " + serviceName);
                return new ServiceTicket
                {
                    UserName = tgt.UserName,
                    ServiceName = serviceName,
                    Expiry = DateTime.Now.AddMinutes(2)
                };
            }
            else
            {
                Console.WriteLine("TGS: TGT invalid sau expirat.");
                return null;
            }
        }

        // serverul pt servicii
        static void ServiceServer(ServiceTicket ticket)
        {
            if (ticket != null && ticket.Expiry > DateTime.Now)
            {
                Console.WriteLine($"SS: Acces permis pentru {ticket.UserName} la serviciul {ticket.ServiceName}.");
            }
            else
            {
                Console.WriteLine("SS: Acces refuzat. Bilet invalid sau expirat.");
            }
        }

        // clientul
        static void Client()
        {
            Console.Write("Introdu numele de utilizator: ");
            string username = Console.ReadLine();

            Console.Write("Introdu parola: ");
            string password = Console.ReadLine();

            // se face autentificarea
            var tgt = AuthenticationServer(username, password);

            if (tgt == null)
            {
                Console.WriteLine("Client: Autentificare eșuată. Ieșire.");
                return;
            }

            // dupa autentificare, se cere un serviciu
            Console.Write("Introdu numele serviciului dorit: ");
            string serviceName = Console.ReadLine();

            var serviceTicket = TicketGrantingServer(tgt, serviceName);

            // accesarea serviciului
            ServiceServer(serviceTicket);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("=== Simplified Kerberos Protocol ===");
            Client();
        }
    }
}

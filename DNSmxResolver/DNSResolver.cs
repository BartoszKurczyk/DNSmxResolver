using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace DNSmxResolver
{
    public class DNSResolver
    {
        protected RaportHeader _raportHeader;
        protected List<RaportBody> _raportBodies;
        private string _hostToResolve;
        private string _dnsServerIP;
        public string Raport = String.Empty;

        public DNSResolver(string hostToResolve, string dnsServerIP = "8.8.8.8")
        {
            _raportHeader = new RaportHeader();
            _raportBodies = new List<RaportBody>();
            _hostToResolve = hostToResolve;
            _dnsServerIP = dnsServerIP;
        }
        public void Resolve()
        {
            //Console.WriteLine($"Current Thread ID: {Thread.CurrentThread.ManagedThreadId}");
            Socket socket = new Socket(SocketType.Dgram, ProtocolType.Udp);

            IPEndPoint dnsServer = new IPEndPoint(IPAddress.Parse(_dnsServerIP), 53);
            socket.Connect(dnsServer);

            SendRequest(_hostToResolve, ref socket);

            GetResponse(ref socket);
            
            socket.Close();

            Raport = CreateRaport();
        }

        private string CreateRaport()
        {
            StringBuilder raport = new StringBuilder();

            raport.AppendLine(String.Format("Request for {0}, type {1}, class {2} for DNS Server {3}", _raportHeader.DomainName, _raportHeader.Type, _raportHeader.Class, _dnsServerIP));

            raport.AppendLine();
            raport.AppendLine(String.Format("|{0,-15}|{1,-5}|{2,-5}|{3,-5}|{4,-8}|{5,-50}|", "Domain", "Type", "Class", "TTL", "Priority", "Target"));

            foreach (var raportBody in _raportBodies.OrderBy(x=>x.Priority))
            {
                raport.AppendLine(String.Format("|{0,-15}|{1,-5}|{2,-5}|{3,-5}|{4,-8}|{5,-50}|", raportBody.DomainName, raportBody.Type, raportBody.Class, raportBody.TTL,
                    raportBody.Priority, raportBody.HostName));
            }
            return raport.ToString();
        }

        private void SendRequest(string host, ref Socket socket)
        {
            byte[] query = { 0x46, 0x5A };
            byte[] flags = { 0x01, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            query = query.Concat(flags).ToArray();

            var splittedHost = host.Split('.');

            foreach (var hostPart in splittedHost)
            {
                byte[] hostPartBytes = System.Text.ASCIIEncoding.Default.GetBytes(hostPart);
                byte[] hostPartBytesLenght = { (byte)hostPartBytes.Length };
                query = query.Concat(hostPartBytesLenght).Concat(hostPartBytes).ToArray();
            }

            byte[] endHost = { 0x00 };
            query = query.Concat(endHost).ToArray();

            byte[] qType = { 0x00, 0x0f };
            byte[] qClass = { 0x00, 0x01 };
            query = query.Concat(qType).Concat(qClass).ToArray();

            socket.Send(query);
        }

        private void GetResponse(ref Socket socket)
        {
            byte[] rBuffer = new byte[1000];
            int receivedLength = socket.Receive(rBuffer);

            int actualIterator = 12;

            CreateHeader(rBuffer, ref actualIterator);

            while (receivedLength > actualIterator)
            {
                CreateRaportBody(rBuffer, ref actualIterator);
            }
        }

        private void CreateRaportBody(byte[] rBuffer, ref int actualIterator)
        {
            var raportBody = new RaportBody();
            var domain = ReadTillZero(rBuffer, ref actualIterator);
            raportBody.DomainName = string.Join(".", domain);

            var typeAndClass = GetTypeAndClass(rBuffer, ref actualIterator);

            raportBody.Type = typeAndClass[0];
            raportBody.Class = typeAndClass[1];

            raportBody.TTL = FourBytesToInt(rBuffer, ref actualIterator);
            actualIterator += 2;

            raportBody.Priority = TwoBytesToInt(rBuffer, ref actualIterator);

            var host = ReadTillZero(rBuffer, ref actualIterator);
            raportBody.HostName = string.Join(".", host);
            _raportBodies.Add(raportBody);
        }

        private void CreateHeader(byte[] rBuffer, ref int i)
        {
            var domain = ReadTillZero(rBuffer, ref i);
            _raportHeader.DomainName = string.Join(".", domain);

            var typeAndClass = GetTypeAndClass(rBuffer, ref i);

            _raportHeader.Type = typeAndClass[0];
            _raportHeader.Class = typeAndClass[1];
        }

        List<string> ReadTillZero(byte[] rBuffer1, ref int i)
        {
            var word = new List<string>();
            var wordLength = rBuffer1[i];

            if (wordLength == 0xc0)
            {
                i++;
                int readFrom = Convert.ToInt32(rBuffer1[i]);
                word = ReadTillZero(rBuffer1, ref readFrom);
                i++;
                return word;
            }

            var wordBytes = rBuffer1.ToList().GetRange(i + 1, wordLength).ToArray();
            i += wordLength + 1;

            if (rBuffer1[i] == 0)
            {
                word.Insert(0, GetWord(wordBytes));
                i++;
                return word;
            }

            word = ReadTillZero(rBuffer1, ref i);
            word.Insert(0, GetWord(wordBytes));
            return word;
        }

        List<string> GetTypeAndClass(byte[] rBuffer1, ref int i)
        {
            byte[] MX = { 0x00, 0x0f };
            byte[] IN = { 0x00, 0x01 };
            byte[] qTypeBytes = rBuffer1.ToList().GetRange(i, 2).ToArray();
            i += 2;
            var qClassBytes = rBuffer1.ToList().GetRange(i, 2).ToArray();
            i += 2;
            List<String> typeAndClass = new List<String>();
            if (qTypeBytes.SequenceEqual(MX))
                typeAndClass.Add("MX");
            else
                typeAndClass.Add("UNK");

            if (qClassBytes.SequenceEqual(IN))
                typeAndClass.Add("IN");
            else
                typeAndClass.Add("UNK");

            return typeAndClass;
        }

        string GetWord(byte[] bytes)
        {
            return System.Text.ASCIIEncoding.Default.GetString(bytes);
        }

        int FourBytesToInt(byte[] rBuffer1, ref int i)
        {
            var ttlBytes = rBuffer1.ToList().GetRange(i, 4).ToArray();
            i += 4;
            Array.Reverse(ttlBytes);
            return BitConverter.ToInt32(ttlBytes, 0);
        }

        int TwoBytesToInt(byte[] rBuffer1, ref int i)
        {
            var toRead = rBuffer1.ToList().GetRange(i, 2).ToArray();
            i += 2;
            Array.Reverse(toRead);
            return BitConverter.ToInt16(toRead, 0); ;
        }
    }
}
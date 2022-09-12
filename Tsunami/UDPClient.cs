using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    public class UDPClient
    {
        private int file_size;
        private int packet_size;
        private int paket_num;
        private Dictionary<int, byte[]> packets;
        public UDPClient(string command)
        {
            if (command.Contains("Send"))
            {
                file_size = int.Parse(command.Split()[1]);
                packet_size = int.Parse(command.Split()[2]);
                if(file_size > packet_size)
                {
                    paket_num = (int)file_size / packet_size;
                }
                else
                {
                    paket_num = 1;
                }
                packets = new Dictionary<int, byte[]>();
                Console.WriteLine($"Number of packets: {paket_num}");
            }
        }

        public void Start()
        {
            
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
            UdpClient udpClient = new UdpClient(12345);

            byte[] buf = new byte[4];
            while (true)
            {
                byte[] packet = udpClient.Receive(ref ip);

                for (int i = 0; i < 4; i++)
                    buf[i] = packet[i];

                int nr = BitConverter.ToInt32(buf, 0);
                packets[nr] = packet.Skip(4).ToArray();
            }
        }

        public void SaveFile(string fileName)
        {
            List<byte> file = new List<byte>();
            packets.OrderBy(key => key.Key).ToList().ForEach(
                pair =>
                {
                    file.AddRange(pair.Value);
                }
            );
           
            File.WriteAllBytes(fileName, file.ToArray());
        }

        public string IsAllPacketRecieved()
        {
            string lostPacketNum = String.Empty;


            for(int i = 0; i < paket_num; i++)
            {
                if (!packets.Keys.Contains(i)) 
                {
                    lostPacketNum += i + ";";
                }
            }
            return lostPacketNum;
        }

    }
}

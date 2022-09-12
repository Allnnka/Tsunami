using System.Net;
using System.Net.Sockets;

namespace Server
{
    public class UDP
    {
        Packet packet;
        public UDP(string data)    
        {
            packet = new Packet(data);
            LostPackage(data.Split()[2]);
        }

        public void LostPackage(string lostNum)
        {
            if (lostNum == "all")
            {
                packet.packetUnits.Add(new PacketUnit(0, (int)packet.file_size / packet.pack_size));
            }
            else
            {
                packet.isAllPacketInFile = false;
                string[] num  = lostNum.Split(";");
                for (int i = 0; i < num.Length; i++)
                {
                    if (num[i].Length > 0)
                    {
                        packet.packetUnits.Add(new PacketUnit(Int32.Parse(num[i]), Int32.Parse(num[i])));
                    }
                }
            }
        }
        public void UDPSentFile()
        {
            UdpClient client = new UdpClient();
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);
            client.Connect(ip);

            byte[] bytes = new byte[packet.pack_size+4];
            FileStream plik = new FileStream(packet.file_name, FileMode.Open);
            using (BinaryReader br = new BinaryReader(plik))
            {
                for (int i = 0; i < packet.packetUnits.Count(); i++)
                    for (int j = packet.packetUnits[i].Start; j <= packet.packetUnits[i].End; j++)
                    {
                        for (int k = 0; k < 4; k++) bytes[k] = BitConverter.GetBytes(j)[k];
                        plik.Seek(j * packet.pack_size, SeekOrigin.Begin);
                        int size = br.Read(bytes, 4, packet.pack_size);

                        client.Send(bytes, size + 4);
                    }
            }
            client.Close();
        }
        public string UDPAnswer()
        {
            if(packet.isAllPacketInFile == false)
            {
                return "sending lost packets";
            }
            return $"Send: {packet.file_size} {packet.pack_size}";
        }
    }
}

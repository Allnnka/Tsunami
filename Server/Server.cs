using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class Server
    {
        static void Main(string[] args)
        {
            TcpListener tcpListener = new TcpListener(IPAddress.Any, 12345);
            TcpClient tcpClient = null;
            tcpListener.Start();
            Console.WriteLine("Server strart");

            while (true)
            {
                TcpClient newTcpClient = tcpListener.AcceptTcpClient();
                if(newTcpClient != tcpClient)
                {
                    tcpClient = newTcpClient;
                    NetworkStream networkStream = tcpClient.GetStream();
                    byte[] answer;
                    byte[] bytes = new byte[256];
                    string data = string.Empty;
                    UDP udp = null;

                    while (tcpClient.Connected)
                    {
                        Console.WriteLine();
                        try
                        {
                            int len = networkStream.Read(bytes, 0, bytes.Length);
                            data += Encoding.ASCII.GetString(bytes, 0, len);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        Console.WriteLine($"CLIENT send: {data}");


                        if (data.Split()[0] == "get")
                        {
                            udp = new UDP(data);
                            Console.WriteLine(udp.UDPAnswer());
                            answer = Encoding.ASCII.GetBytes(udp.UDPAnswer());
                            networkStream.Write(answer, 0, answer.Length);
                            data = null;
                        }
                        if (data == "ok")
                        {
                            udp.UDPSentFile();
                            Thread.Sleep(2000);
                            answer = Encoding.ASCII.GetBytes("Server end sending");
                            networkStream.Write(answer, 0, answer.Length);
                            data = null;
                        }
                        else if (data == "stop")
                        {
                            break;
                        }
                    }

                    networkStream.Close();
                    tcpClient.Close();
                    Console.WriteLine("Server end work.\nAll packet are send");
                }
            }
        }
    }
}

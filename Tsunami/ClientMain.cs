using System.Net.Sockets;
using System.Text;

namespace Client
{
    public class ClientMain
    {
        static void Main()
        {
            Console.WriteLine("Start TCP Client");
            TcpClient tcpClient = new TcpClient("localhost", 12345);
            NetworkStream networkStream = tcpClient.GetStream();

            Console.WriteLine("Write command for download file from server. E.g. get test.txt");
            string command = Console.ReadLine();
            byte[] data = Encoding.ASCII.GetBytes(command+" all\n");
            networkStream.Write(data, 0, data.Length);

            Console.WriteLine($"Send answer for file {command}");

            byte[] response = new byte[256];
            int bytes;
            string responseStr = string.Empty;

            UDPClient Udpclient = new UDPClient("tmp");

            Thread receive_thread = new Thread(Udpclient.Start);
            while (true)
            {
                try
                {
                    bytes = networkStream.Read(response, 0, response.Length);
                    responseStr += Encoding.ASCII.GetString(response, 0, bytes);
                } catch(Exception e)
                {
                    Console.WriteLine(e);
                }
                Console.WriteLine($"$$$ Server send answer: {responseStr}");
                if (responseStr.Contains("Send") || responseStr.Contains("sending lost packets"))
                {
                    if (responseStr.Contains("Send")) {
                        Udpclient = new UDPClient(responseStr);
                        receive_thread = new Thread(Udpclient.Start);
                        receive_thread.Start();
                    }
                    data = Encoding.ASCII.GetBytes("ok");
                    networkStream.Write(data, 0, data.Length);
                    responseStr = string.Empty;

                }
                else if (responseStr.Contains("Server end sending"))
                {
                    string lost = Udpclient.IsAllPacketRecieved();
                    if (lost != String.Empty)
                    {
                        data = Encoding.ASCII.GetBytes(command +" "+ lost + "\n");
                        networkStream.Write(data, 0, data.Length);
                        responseStr = string.Empty;
                    }
                    else
                    {
                        data = Encoding.ASCII.GetBytes("stop");
                        networkStream.Write(data, 0, data.Length);
                        Udpclient.SaveFile(command.Split()[1]);
                        Console.WriteLine("\nFile download and save on disk!");
                        break;
                    }
                }
            }
            

        }
    }
}

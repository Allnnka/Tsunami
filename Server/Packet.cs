
namespace Server
{
    public class PacketUnit
    {
        public int Start { get; set; }
        public int End { get; set; }
        public PacketUnit(int start, int end)
        {
            Start = start;
            End = end;
        }
        public PacketUnit() { }
    }
    public class Packet
    {
        public string file_name { get; set; }
        public int pack_size = 256; //32 bajty
        public long file_size { get; set; }
        public List<PacketUnit> packetUnits { get; set; }
        public bool isAllPacketInFile { get; set; }

        public Packet(string data)
        {
            isAllPacketInFile = true;
            file_name = @"c:\temp\"+data.Split()[1];
            //file_name = @data.Split()[1];
            if (File.Exists(file_name))
            {
                file_size = new FileInfo(file_name).Length;
                packetUnits = new List<PacketUnit>();
            }
        }
    }
}

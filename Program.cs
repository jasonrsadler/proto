using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
namespace proto
{
    class Program
    {
        static void Main(string[] args)
        {
            Double totalDebits = 0.0f;
            Double totalCredits = 0.0f;
            Int32 numAutoPaysStarted = 0;
            Int32 numAutoPaysEnded = 0;
            Double balanceOfSpecifiedUser = 0.0f;
            const UInt64 specifiedUser = 2456938384156277127;
            Console.WriteLine("Reading File.....");
            BinaryReader reader = new BinaryReader(File.Open("./proto/txnlog.dat", FileMode.Open));            
            //read in the first 'line' of bytes for header            
            String inputType = Convert.ToString(Encoding.ASCII.GetString(reader.ReadBytes(4).ToArray())); //magic, don't need to resolve
            String version = BitConverter.ToString(reader.ReadBytes(1).ToArray()); //version            
            UInt32 numRecords = (UInt32)ResolveNetworkBytes(reader, true);          
            //read in records
            for (int ix = 0; ix < numRecords; ix++) {
                String recordType = BitConverter.ToString(reader.ReadBytes(1).ToArray()); //1 byte just read in
                UInt32 timeStamp = (UInt32)ResolveNetworkBytes(reader, true);
                UInt64 userID = ResolveNetworkBytes(reader, false);
                Double amount = 0.0f;
                if (recordType == "00" || recordType == "01") {
                    //read in more
                    amount = ResolveNetworkBytes(reader);
                    if (recordType == "00")

                }
                
                //print results to stdout
                Console.WriteLine(inputType + "\n" + version + "\n" + numRecords.ToString());
                Console.WriteLine("Record is type: " + recordType + " timestamp: " + timeStamp.ToString() + 
                    " userID: " + userID.ToString() + " amount: " + String.Format("{0:0.00}", Convert.ToDecimal(amount.ToString())));
                Console.WriteLine("=====================================\nAnswers To Questions:\n=====================================");
            }
        }
        static UInt64 ResolveNetworkBytes(BinaryReader reader, bool IsUnsigned32) {
            byte[] bytes = IsUnsigned32 ? reader.ReadBytes(4) : reader.ReadBytes(8);
            ResolveEndianNess(ref bytes);            
            return IsUnsigned32 ? BitConverter.ToUInt32(bytes, 0) : BitConverter.ToUInt64(bytes, 0);
        }
        static Double ResolveNetworkBytes(BinaryReader reader) {
            byte[] bytes = reader.ReadBytes(8);
            ResolveEndianNess(ref bytes);            
            return BitConverter.ToDouble(bytes, 0);
        }
        static void ResolveEndianNess(ref byte[] valueBytes) {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(valueBytes);
        }
    }
}

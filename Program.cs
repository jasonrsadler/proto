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
            BinaryReader reader = new BinaryReader(File.Open("./txnlog.dat", FileMode.Open));            
            //read in the first 'line' of bytes for header            
            String inputType = Convert.ToString(Encoding.ASCII.GetString(reader.ReadBytes(4).ToArray())); //magic, don't need to resolve
            String version = BitConverter.ToString(reader.ReadBytes(1).ToArray()); //version            
            UInt32 numRecords = (UInt32)ResolveNetworkBytes(reader, true);          
            
            Console.WriteLine(inputType + "\n" + version + "\n" + numRecords.ToString());
                
            //read in records
            for (int ix = 0; ix < numRecords; ix++) {
                RecordTypes recordType = (RecordTypes)Convert.ToInt32
                    (BitConverter.ToString(reader.ReadBytes(1).ToArray())); //1 byte just read in
                UInt32 timeStamp = (UInt32)ResolveNetworkBytes(reader, true);
                UInt64 userID = ResolveNetworkBytes(reader, false);                
                Double amount = 0.0f;
                //read in additional bytes of debit or credit
                    
                switch(recordType) {
                    case RecordTypes.Credit: {
                        amount = ResolveNetworkBytes(reader);
                        totalCredits += amount;
                        if (userID == specifiedUser) {
                            balanceOfSpecifiedUser = amount;
                        }
                        break;
                    } 
                    case RecordTypes.Debit: {
                        amount = ResolveNetworkBytes(reader);
                        totalDebits += amount;
                        if (userID == specifiedUser) {
                            balanceOfSpecifiedUser = amount;
                        }
                        break;
                    } 
                    case RecordTypes.StartAutoPay: {
                        numAutoPaysStarted++;
                        break;
                    }
                    case RecordTypes.EndAutoPay: {
                        numAutoPaysEnded++;
                        break;
                    }
                }

                
                
                //print results to stdout
                Console.WriteLine("Record is type: " + recordType + " timestamp: " + timeStamp.ToString() + 
                    " userID: " + userID.ToString() + " amount: " + String.Format("{0:0.00}", Convert.ToDecimal(amount.ToString())));                
            }
            Console.WriteLine("=====================================\nAnswers To Questions:\n=====================================");
            Console.WriteLine("Total Debits: $" + String.Format("{0:0.00}", Convert.ToDecimal(totalDebits)));
            Console.WriteLine("Total Credits: $" + String.Format("{0:0.00}", Convert.ToDecimal(totalCredits)));
            Console.WriteLine("AutoPays Started: " + numAutoPaysStarted.ToString());
            Console.WriteLine("AutoPays Ended: " + numAutoPaysEnded.ToString());
            Console.WriteLine("Balance of UserID " + specifiedUser.ToString() + ": " + String.Format("{0:0.00}", Convert.ToDecimal(balanceOfSpecifiedUser)));
            Console.WriteLine("=====================================\nEND\n=====================================");
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

    enum RecordTypes {
        Debit = 0, Credit = 1, StartAutoPay = 2, EndAutoPay = 3
    };
}

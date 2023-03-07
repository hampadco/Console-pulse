using System;
using System.IO.Ports;
using System.Threading;

namespace srial_port_aref {
    class Program {
        static SerialPort serialPort1 = new SerialPort();
        private delegate void DisplayDelegate(int value);

        static void Main(string[] args) {
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(Program.SerialPort1_DataReceived);
            serialPort1.PortName = SerialPort.GetPortNames()[0];
            serialPort1.BaudRate = 19200;
            serialPort1.Parity = Parity.Odd;
            serialPort1.DataBits = 8;
            serialPort1.StopBits = StopBits.One;

            try {
                serialPort1.Open();
                byte[] tx_Data = new byte[10];
                tx_Data[0]= 0xFA;
                tx_Data[1] = 0x0A;
                tx_Data[2] = 0x03;
                tx_Data[3] = 0x01;
                tx_Data[4] = 0x01;
                tx_Data[5] = 0x00;
                tx_Data[6] = 0x00;
                tx_Data[7] = 0x00;
                tx_Data[8] = 0x00;
                tx_Data[9] = 0x0F;
                serialPort1.Write(tx_Data, 0, tx_Data.Length);
                System.Console.WriteLine("Send data to device");
                Console.WriteLine("Serial port opened." + serialPort1.PortName);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return;
            }

            while (true) {
                Thread.Sleep(500);
            }
        }

       private static void SerialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
{
    byte[] buffer = new byte[17];
   
    while (serialPort1.BytesToRead > 14)
    {
        buffer[1] = (byte)serialPort1.ReadByte();
        //print buffer[1] just byte[1] 
       
            Console.WriteLine( " buffer[1] : " + buffer[1] );
        
        // 250 => 0xFA
        // 255 => 0xFF
       
       
        if (buffer[1] == 0xFA) //250
        {
            System.Console.WriteLine("******************************buffer[1] == 0xFA*********************************");

            buffer[2] = (byte)serialPort1.ReadByte();    
            buffer[3] = (byte)serialPort1.ReadByte();
            serialPort1.Read(buffer, 4, 10);

            //print bufer[2] to buffer[13]
            for (int i = 2; i < 14; i++)
            {
                Console.WriteLine("buffer[" + i + "] : " + buffer[i]);
            }


            if (buffer[2] == 13 && buffer[4] == 4 && buffer[5] == 132)
            {
                System.Console.WriteLine("******************************buffer[2] == 13 && buffer[4] == 4 && buffer[5] == 132*********************************");
                Task.Run(() =>
                {
                    if (buffer[10] == 255) Display2(30);
                    else Display2(buffer[10] * 3);
                }).Wait();
            }
            if (buffer[2] == 17 && buffer[4] == 4 && buffer[5] == 133)
            {
                System.Console.WriteLine("******************************buffer[2] == 17 && buffer[4] == 4 && buffer[5] == 133*********************************");
                int pulse = (buffer[11] << 8) | buffer[10];
                int spo = buffer[12];
                if (pulse == 511) Task.Run(() => DisplayPulse(null)).Wait();
                else Task.Run(() => DisplayPulse(pulse)).Wait();
                if (spo == 127) Task.Run(() => DisplaySpO(null)).Wait();
                else Task.Run(() => DisplaySpO(spo)).Wait();
            }
        }
    }
}

        private static void Display2(int? wave) {
           // Console.WriteLine($"Wave: {wave}");
           if (wave==null || wave<0)
           {
                Console.WriteLine("Wave: --");
            
           }
              else
              {
                 Console.WriteLine($"Wave: {wave}");
              }
        }

        private static void DisplayPulse(int? pulse) {
          //  Console.WriteLine($"Pulse: {pulse < 0 ? "--" : pulse.ToString()}");
            if (pulse==null || pulse<0)
             {
                    Console.WriteLine("Pulse: --");
                
             }
                else
                {
                     Console.WriteLine($"Pulse: {pulse}");
                }
        }

        private static void DisplaySpO(int? spo) {
           // Console.WriteLine($"SpO2: {spo < 0 ? "--" : spo.ToString()}");
            if (spo==null || spo<0)
             {
                    Console.WriteLine("SpO2: --");
                
             }
                else
                {
                     Console.WriteLine($"SpO2: {spo}");
                }
        }
    }
}

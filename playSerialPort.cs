using System;
using System.IO;
using System.IO.Ports;
public class Program{
static SerialPort serialPort;
public static void Main(string[] args){
    serialPort=new SerialPort("COM5",9600,Parity.None);
    serialPort.ReadTimeout = 500;
    serialPort.WriteTimeout = 500;
    serialPort.Open();

    if ((args==null)||(args.Length==0))
    {
        Console.WriteLine("No Files are loaded!");
        return;
    }
    //Loop through al files
                    Console.WriteLine("Hi1");
    string docPath =Environment.CurrentDirectory;
    foreach(string fileName in args){
        string fullFileName= Path.Combine(docPath,fileName);
        using(StringReader moveFile= new StringReader( fullFileName))
        {
            int counter=0;
            Console.WriteLine("Hi2");
            string linePosition=string.Empty;
            while(true){
                try{
                    linePosition=moveFile.ReadLine();
                    if (linePosition==null) break;
                    //serialPort.WriteLine(linePosition);
                    Console.WriteLine(linePosition.ToString());
                    //Console.WriteLine("Hi3");
                    counter++;
                }
                catch(TimeoutException){
                    Console.WriteLine("Time out exception! ");
                }                
            }
        }
    }
    serialPort.Close();
}
public byte[] ReadByteArrayFromFile(string fileName)
{
    byte[] buff = null;
    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
    BinaryReader br = new BinaryReader(fs);
    long numBytes = new FileInfo(fileName).Length;
    buff = br.ReadBytes((int)numBytes);
    return buff;
}

}

using System;
using System.Threading;
using System.IO;
using System.IO.Ports;
using System.Linq;
public class Program{
     const int delayTime=100;
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
    string docPath =Environment.CurrentDirectory;
    foreach(string fileName in args){
        string fullFileName= Path.Combine(docPath,fileName);
        int fileLineCount=CountLinesLINQ(fullFileName);
        Console.WriteLine("exists:" + File.Exists(fullFileName)+ ","+fileLineCount);
        using(StringReader moveFile= new StringReader(fullFileName))
        {
            
            int counter=0;
            string linePosition=string.Empty;
            Console.WriteLine(linePosition);
            string wholeText=moveFile.ReadToEnd();
            Console.WriteLine(wholeText);
            while(counter<fileLineCount){
                
                try{
                    linePosition=moveFile.ReadLine();
                    Console.WriteLine(linePosition);
                    counter++;
                    Thread.Sleep(delayTime);  
                }
                catch(TimeoutException){
                    Console.WriteLine("Time out exception! ");
                }                
            }
            moveFile.Close();
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
public static int CountLinesLINQ(string fileName)  
    => File.ReadLines(fileName).Count();

}

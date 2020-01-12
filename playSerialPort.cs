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
        string[] data=File.ReadAllLines(fullFileName);
        foreach (var item in data)
        {
            serialPort.WriteLine(item);
            Console.WriteLine(item);
            Thread.Sleep(delayTime);  
        }
        
    }
    serialPort.Close();
}

public static int CountLinesLINQ(string fileName)  
    => File.ReadLines(fileName).Count();

}

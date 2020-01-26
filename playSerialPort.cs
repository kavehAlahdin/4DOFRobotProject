using System;
using System.Threading;
using System.IO;
using System.IO.Ports;
using System.Linq;
public class Program{
     const int defaultDelayTime=100;
     static int delayTime=defaultDelayTime;
static SerialPort serialPort;
public static void Main(string[] args){
    serialPort=new SerialPort();
    serialPort.PortName =SetPortName("COM7");// SetPortName(_serialPort.PortName);
    serialPort.BaudRate = 9600; //SetPortBaudRate(_serialPort.BaudRate);
    serialPort.Parity = Parity.None;// SetPortParity(_serialPort.Parity);
        
    serialPort.ReadTimeout = 500;
    serialPort.WriteTimeout = 500;
    serialPort.Open();
    if ((args==null)||(args.Length==0))
    {
        Console.WriteLine("No Files are loaded!");
        return;
    }
    delayTime=SetDelayinMillis();
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

public static string SetPortName(string defaultPortName)
    {
        string portName;
        Console.WriteLine("Available Ports:");
        foreach (string s in SerialPort.GetPortNames())
        {
            Console.WriteLine("   {0}", s);
        }

        Console.Write("Enter COM port value (Default: {0}): ", defaultPortName);
        portName = Console.ReadLine();

        if (portName == "" || !(portName.ToLower()).StartsWith("com"))
        {
            portName = defaultPortName;
        }
        return portName;
    }
    public static  int  SetDelayinMillis(){
        int delayTime;
        Console.Write("Set the delay between command lines in millisecond(Default:{0})",defaultDelayTime);
        bool validEntry=Int32.TryParse( Console.ReadLine(),out delayTime);
        if(!validEntry)
            Console.WriteLine("There was an error with the daly time.");
        Console.WriteLine("The {0} millisecond will be used",defaultDelayTime);
            
        return delayTime;
    }


}
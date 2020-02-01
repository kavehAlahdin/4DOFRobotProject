using System;
using System.IO.Ports;
using System.Threading;
using System.IO;
using System.Collections.Generic;
public class RobotControl{
static bool  endProgram=false;
static bool _continue;
    static SerialPort _serialPort;
    static StreamWriter _streamWriter;
 enum MenuState{Firstmenu=1,Kinematic=2,KinematicSendCommands=21,KinematicSaveCommands=22,InversKinematic=3,Robotstate=4};
static  Dictionary<MenuState,String> MenuDictionary=new  Dictionary<MenuState,String>{
    {MenuState.Firstmenu,@"Menu: 
            (1) Setup serial port
            (2) Close the open serial port
            (3) Change Robot state
            (4) Kinematic
            (5) Inverse Kinematic
            (x) Exit
             "},
    {MenuState.Kinematic,@"Kinematic:
            (1) Save Robot Position
            (2) Send Command to robot
                (b) Back    (x) Exit"},
    {MenuState.InversKinematic,@"(To be implemented)
            (b) Back (x)"},
    {MenuState.Robotstate,@"Robot state:
            (1) Control per potentionmeters
            (2) Control perPotentiometer + serial port output of the servo positions
            (3) Control via serial port (expects absolute values)
            (4) Control vis Serial Port (Expects relative values)
            (b) back (x) Exit"},
    {MenuState.KinematicSaveCommands,@"Kinematic - Save:
            (1) Save One Position
            (2) Save Until stopped
            (b) Back (s) Stop (x) Exit"},
    {MenuState.KinematicSendCommands,@"Kinematic - Send:
            (1) Send One Command
            (2) Send multiple from file
            (b) Back    (s) Stop    (x) Exit"}
    };
static MenuState currentMenu=MenuState.Firstmenu;
    public static void Main(string[] args){
        Console.WriteLine("Control 4DoF Robot through serial port");
        for (int i = 0; i < 100; i++)
            Console.Write("-");
        Console.WriteLine();
        String menuMessage=MenuDictionary[currentMenu];
        while(!endProgram){
            Console.WriteLine(menuMessage);
            ConsoleKeyInfo inputValue= Console.ReadKey();
            manageFirstmenu(inputValue);

            Thread.Sleep(100);
        }
    }

    public static void manageFirstmenu(ConsoleKeyInfo inputValue){
        if(inputValue.Key==ConsoleKey.D1) setupSerialPOrt(); 
        else if(inputValue.Key==ConsoleKey.D2)
            if(_serialPort.IsOpen) _serialPort.Close();
        else if(inputValue.Key==ConsoleKey.D3)
            if(!_serialPort.IsOpen())
                Console.WriteLine("The serial port is not open!");
            else
                
        if (inputValue.Key==ConsoleKey.X) 
                endProgram=true; 
    }
    private static void setupSerialPOrt(){
        _serialPort = new SerialPort();
        // Allow the user to set the appropriate properties.
        _serialPort.PortName =SetPortName(_serialPort.PortName);
        _serialPort.BaudRate = SetPortBaudRate(_serialPort.BaudRate);
        _serialPort.Parity =  SetPortParity(_serialPort.Parity);
        _serialPort.DataBits = SetPortDataBits(_serialPort.DataBits);
        _serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);
        _serialPort.Handshake = SetPortHandshake(_serialPort.Handshake);

        // Set the read/write timeouts
        //_serialPort.ReadTimeout = 500;
        //_serialPort.WriteTimeout = 500;

    }    
    // Display Port values and prompt user to enter a port.
    private static string SetPortName(string defaultPortName)
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
    // Display BaudRate values and prompt user to enter a value.
    private static int SetPortBaudRate(int defaultPortBaudRate)
    {
        string baudRate;

        Console.Write("Baud Rate(default:{0}): ", defaultPortBaudRate);
        baudRate = Console.ReadLine();

        if (baudRate == "")
        {
            baudRate = defaultPortBaudRate.ToString();
        }

        return int.Parse(baudRate);
    }

    // Display PortParity values and prompt user to enter a value.
    private static Parity SetPortParity(Parity defaultPortParity)
    {
        string parity;

        Console.WriteLine("Available Parity options:");
        foreach (string s in Enum.GetNames(typeof(Parity)))
        {
            Console.WriteLine("   {0}", s);
        }

        Console.Write("Enter Parity value (Default: {0}):", defaultPortParity.ToString(), true);
        parity = Console.ReadLine();

        if (parity == "")
        {
            parity = defaultPortParity.ToString();
        }

        return (Parity)Enum.Parse(typeof(Parity), parity, true);
    }
    // Display DataBits values and prompt user to enter a value.
    private static int SetPortDataBits(int defaultPortDataBits)
    {
        string dataBits;

        Console.Write("Enter DataBits value (Default: {0}): ", defaultPortDataBits);
        dataBits = Console.ReadLine();

        if (dataBits == "")
        {
            dataBits = defaultPortDataBits.ToString();
        }

        return int.Parse(dataBits.ToUpperInvariant());
    }

    // Display StopBits values and prompt user to enter a value.
    private static StopBits SetPortStopBits(StopBits defaultPortStopBits)
    {
        string stopBits;

        Console.WriteLine("Available StopBits options:");
        foreach (string s in Enum.GetNames(typeof(StopBits)))
        {
            Console.WriteLine("   {0}", s);
        }
        Console.Write("Enter StopBits value (None is not supported and \n" +
         "raises an ArgumentOutOfRangeException. \n (Default: {0}):", defaultPortStopBits.ToString());
        stopBits = Console.ReadLine();
       
        if (stopBits == "" )
        {
            stopBits = defaultPortStopBits.ToString();
        }

        return (StopBits)Enum.Parse(typeof(StopBits), stopBits, true);
    }
    private static Handshake SetPortHandshake(Handshake defaultPortHandshake)
    {
        string handshake;

        Console.WriteLine("Available Handshake options:");
        foreach (string s in Enum.GetNames(typeof(Handshake)))
        {
            Console.WriteLine("   {0}", s);
        }

        Console.Write("Enter Handshake value (Default: {0}):", defaultPortHandshake.ToString());
        handshake = Console.ReadLine();

        if (handshake == "")
        {
            handshake = defaultPortHandshake.ToString();
        }

        return (Handshake)Enum.Parse(typeof(Handshake), handshake, true);
    }




    ///


    public void manageKinematicMenu(){}
    public void manageInverseKinematicMenu(){}
    public void manageKinematicSendCommandsMenu(){}
    public void manageKinematicSaveCommandsmenu(){}
    public void manageRobotStatemenu(){}


}

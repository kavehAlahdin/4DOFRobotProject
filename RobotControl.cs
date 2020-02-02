using System;
using System.IO.Ports;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing;
public class RobotControl{
static bool  endProgram=false;
static bool _continue;
    static SerialPort _serialPort;
    static StreamWriter _streamWriter;
         const int defaultDelayTime=100;

    static string toSendFile;
    static string toSaveFile;
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
            (b) Back (x) Exit"},
    {MenuState.Robotstate,@"Robot state:
            (1) Control per potentionmeters
            (2) Control perPotentiometer + serial port output of the servo positions
            (3) Control via serial port (expects absolute values)
            (4) Control vis Serial Port (Expects relative values)
            (b) back (x) Exit"},
    {MenuState.KinematicSaveCommands,@"Kinematic - Save:
            (1) Save One Position
            (2) Save Until stopped
            (3) Set the File name
            (b) Back (s) Stop (x) Exit"},
    {MenuState.KinematicSendCommands,@"Kinematic - Send:
            (1) Send One Command
            (2) Send multiple from file
            (3) Load a new File
            (b) Back    (s) Stop    (x) Exit"}
    };
static MenuState currentMenu=MenuState.Firstmenu;
    [STAThread]

    public static void Main(string[] args){
        Application.SetCompatibleTextRenderingDefault(false);
        Application.EnableVisualStyles();
        Console.WriteLine("Control 4DoF Robot through serial port");
        for (int i = 0; i < 50; i++)
            Console.Write("-");
        Console.WriteLine();
        String menuMessage;
        while(!endProgram){
            menuMessage=MenuDictionary[currentMenu];
            Console.WriteLine(menuMessage);
            ConsoleKeyInfo inputValue= Console.ReadKey();
            Console.WriteLine();
            switch (currentMenu)
            {
                case MenuState.Firstmenu:
                    manageFirstmenu( inputValue);
                    break;
                case MenuState.Robotstate:
                    manageRobotStatemenu( inputValue);
                    break;
                case MenuState.Kinematic:
                    manageKinematicMenu( inputValue);
                    break;
                case MenuState.InversKinematic:
                    manageInverseKinematicMenu( inputValue);
                    break;
                case MenuState.KinematicSaveCommands:
                    manageKinematicSaveCommandsMenu( inputValue);
                    break;
                case MenuState.KinematicSendCommands:
                    manageKinematicSendCommandsMenu( inputValue);
                    break;
                
            }
            for (int i = 0; i < 50; i++)
                Console.Write("-");
                Console.WriteLine();
            //Thread.Sleep(100);
        }
    }

    public static void manageFirstmenu(ConsoleKeyInfo inputValue){
        if(inputValue.Key==ConsoleKey.D1) setupSerialPort(); 
        else if(inputValue.Key==ConsoleKey.D2)
        {
            if(_serialPort!=null||_serialPort.IsOpen) {
                _serialPort.Close();
                Console.WriteLine("serial port is closed");
            }
            else
            {
                Console.WriteLine("Serial port is already closed!");
            }
        }
        else if(inputValue.Key==ConsoleKey.D3)
            if(_serialPort==null ||!_serialPort.IsOpen)
                Console.WriteLine("The serial port is not open!");
            else
            {
                Console.WriteLine("Change to Robot state menu");
                currentMenu=MenuState.Robotstate;
            }
        else if (inputValue.Key==ConsoleKey.D4)
        {
            Console.WriteLine("Change to kinematic menu ");
            currentMenu=MenuState.Kinematic;
        }
        else if(inputValue.Key==ConsoleKey.D5)
        {
            currentMenu=MenuState.InversKinematic;
            Console.WriteLine("Change to Inverse kinematic menu");
        }
        if (inputValue.Key==ConsoleKey.X) 
            {
                endProgram=true; 
                Console.WriteLine("Exiting the program");
            }
    }
    
    public static void manageKinematicMenu(ConsoleKeyInfo inputValue)
    {
        if (inputValue.Key==ConsoleKey.D1)
            currentMenu=MenuState.KinematicSaveCommands;
        else if(inputValue.Key==ConsoleKey.D2)
            currentMenu=MenuState.KinematicSendCommands;
        else if(inputValue.Key==ConsoleKey.B)
            currentMenu=MenuState.Firstmenu;
        else if(inputValue.Key==ConsoleKey.X)
        {
            endProgram=true; 
            Console.WriteLine("Exiting the program");
        }
    }
    public static void manageKinematicSendCommandsMenu(ConsoleKeyInfo inputValue){
        if(_serialPort==null ||!_serialPort.IsOpen){
            Console.WriteLine("Serial port is not available!");
            currentMenu=MenuState.Kinematic;
            return;
        }
        if(inputValue.Key ==ConsoleKey.D1){
            //send one command
            Console.WriteLine("Please enter the robot position:[expects 4 number seperated with \",\"] ");
            String enteredPosition=Console.ReadLine();
            _serialPort.WriteLine (enteredPosition);           
        }
        else if (inputValue.Key==ConsoleKey.D2)
        {
            //send multiple from a file
            if(_serialPort==null ||!_serialPort.IsOpen){
                Console.WriteLine("Serial port is not available!");
                currentMenu=MenuState.Kinematic;
                return;
            }
             if (toSendFile==null||toSendFile.Length==0)
            {
                Console.WriteLine("No Files are loaded!");
                return;
            }
            int delayTime=SetDelayinMillis("Set the delay between command lines in millisecond");
            //Loop through al files
            string[] data=File.ReadAllLines(toSendFile);
            foreach (var item in data)
            {
                _serialPort.WriteLine(item);
                Console.WriteLine(item);
                Thread.Sleep(delayTime);  
            }
        }
        else if (inputValue.Key==ConsoleKey.D3)
        {
               //Load a file
               openFile();
        }
        if (inputValue.Key==ConsoleKey.S)
        {
            //stop the file playing
        }
         if(inputValue.Key==ConsoleKey.B)
            currentMenu=MenuState.Kinematic;
        else if(inputValue.Key==ConsoleKey.X)
        {
            endProgram=true; 
            Console.WriteLine("Exiting the program");
        }

    }
    static OpenFileDialog openFileDialog;
    private static void openFile(){
            using (openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory =@"C:\Users\admin\Documents\Arduino\RobotArm\071-Robot-Arm-Kit-Potentiometers";// "c:\\";
                openFileDialog.Filter ="txt files (*.txt)|*.txt|All files (*.*)|*.*";  
                openFileDialog.FilterIndex = 2;  
                openFileDialog.CheckFileExists = true;  
                openFileDialog.CheckPathExists = true;  
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect=false;
                openFileDialog.Title="Select the file";
                
                DialogResult dResult= openFileDialog.ShowDialog();
                if (dResult== DialogResult.OK)
                {
                    //Get the path of specified file
                    toSendFile = openFileDialog.FileName;
                    //Read the contents of the file into a stream
                    Console.WriteLine("File "+ toSendFile + " is opened!");                   
                }else{
                    Console.WriteLine("No File was opened!");
                }
            }
    }
    private static  int  SetDelayinMillis(String message){
        int delayTime;
        Console.Write(message+ " (Default:{0})",defaultDelayTime);
        bool validEntry=Int32.TryParse( Console.ReadLine(),out delayTime);
        if(!validEntry)
            Console.WriteLine("There was an error with the input time.");
        Console.WriteLine("The {0} millisecond will be used",defaultDelayTime);            
        return delayTime;
    }
    ///
    public static void manageKinematicSaveCommandsMenu(ConsoleKeyInfo inputValue){

        if(inputValue.Key==ConsoleKey.D1)
        {
            if(_serialPort==null||!_serialPort.IsOpen){
                currentMenu=MenuState.Kinematic;
                return;
            }
            if(toSaveFile==null) {
                Console.WriteLine("The Target file name is not set");
                return;
            }
            string message=_serialPort.ReadLine();
            if (_streamWriter==null)
                {_streamWriter=new StreamWriter(toSaveFile,true);
            _streamWriter.WriteLine(message);
            _streamWriter.Close();
        }
        else if(inputValue.Key==ConsoleKey.D2){
            //save multiple position unless stopped
             //save one position
            if(_serialPort==null||!_serialPort.IsOpen){
                currentMenu=MenuState.Kinematic;
                return;
            }
            if(toSaveFile==null) {
                Console.WriteLine("The Target file name is not set");
                return;
            }
            Thread readThread = new Thread(ReadSerialPort);
            _streamWriter=new StreamWriter(toSaveFile,true);
            try
            {
                readThread.Start();
            }
            catch (System.Exception)
            {
                readThread.Join();
            }
            _continue = true;
            Console.Write("Name: ");
            ConsoleKeyInfo inputKey;
            String name = Console.ReadLine();
            Console.WriteLine("Type S to Stop");
            while (_continue)
            {
                inputKey = Console.ReadKey();
                if (inputValue.Key==ConsoleKey.S)
                {
                    _continue = false;
                }
                else
                {
                    _serialPort.WriteLine(
                    String.Format("<{0}>: {1}", name, inputKey.ToString()));
                }
            }
            readThread.Join();
            _streamWriter.Close();
        }
        else if(inputValue.Key==ConsoleKey.D3){
            setFileToSave();
            //set the file name to save
        }
        if (inputValue.Key==ConsoleKey.S)
        {
            //stop the file playing
        }
         if(inputValue.Key==ConsoleKey.B)
            currentMenu=MenuState.Kinematic;
        else if(inputValue.Key==ConsoleKey.X)
        {
            endProgram=true; 
            Console.WriteLine("Exiting the program");
        }

    }
    }
    static SaveFileDialog saveFileDialog;
    private static void setFileToSave(){
            using (saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"  ;
                saveFileDialog.FilterIndex = 2 ;
                saveFileDialog.RestoreDirectory = true ;
                saveFileDialog.Title="save file";
                DialogResult dResult= saveFileDialog.ShowDialog();
                if (dResult== DialogResult.OK)
                {
                    //Get the path of specified file
                    toSaveFile = saveFileDialog.FileName;
                    //Read the contents of the file into a stream
                    Console.WriteLine("The File "+ toSaveFile + " will be appended!");                   
                }else{
                    Console.WriteLine("No File was set!");
                }
            }
        }
    
    public static void manageInverseKinematicMenu(ConsoleKeyInfo inputValue){
         if(inputValue.Key==ConsoleKey.B)
            currentMenu=MenuState.Firstmenu;
        else if(inputValue.Key==ConsoleKey.X)
        {
            endProgram=true; 
            Console.WriteLine("Exiting the program");
        }
    }
    public static void manageRobotStatemenu(ConsoleKeyInfo inputValue){
        if(_serialPort==null ||!_serialPort.IsOpen){
            Console.WriteLine("Serial port is not openyet!");
            currentMenu=MenuState.Firstmenu;
            return;
        }
        if(inputValue.Key==ConsoleKey.D1)
            _serialPort.WriteLine("1");
        else if(inputValue.Key==ConsoleKey.D2)
            _serialPort.WriteLine("2");
        else if(inputValue.Key==ConsoleKey.D3)
            _serialPort.WriteLine("3");
        else if(inputValue.Key==ConsoleKey.D4)
            _serialPort.WriteLine("4");
        else if(inputValue.Key==ConsoleKey.B)
            currentMenu=MenuState.Firstmenu;
        else if(inputValue.Key==ConsoleKey.X)
        {
            endProgram=true; 
            Console.WriteLine("Exiting the program");
        }
    }
    private static void setupSerialPort(){
        _serialPort = new SerialPort();
        //Thread readThread = new Thread(ReadSerialPort);
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
        try
        {
            _serialPort.Open();
         //   readThread.Start();
        }
        catch (System.Exception)
        {
            Console.WriteLine("Error in openning the serial port");
           // readThread.Join();
            _serialPort.Close();    
        }
    } 
    private static void ReadSerialPort()
    {
        _serialPort.WriteLine("2");
        while (_continue)
        {
            try
            {
                string message = _serialPort.ReadLine();
                if ((message!=null)||(message.Length!=0)){
                    Console.WriteLine(message);
                    _streamWriter.WriteLine(message);
                }
            }
            catch (TimeoutException) { 
                Console.WriteLine("Error!");
                _streamWriter.Close();
                _streamWriter.Dispose();
            }
        }
        _streamWriter.Close();
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

    private static void quit(){

    }


    ///


}

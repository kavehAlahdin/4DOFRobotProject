 //add servo library
#include <Servo.h>
#include <string.h>
#include <stdio.h>
#include <string.h>
//define our servos
Servo servo1;
Servo servo2;
Servo servo3;
Servo servo4;

//define our potentiometers
int pot1 = A0;
int pot2 = A1;
int pot3 = A2;
int pot4 = A3;

//variable to read the values from the analog pin (potentiometers)
int valPot1;
int valPot2;
int valPot3;
int valPot4;
String separator=", ";
String message="";
char delimiter[]=",\n";

int valueList[]={0,0,0,0};
/*set the state: 
  1-use only potentiometer values:potentioRead
  2-send the servo state to serial port:SerialSend
  3-read data from Serial port :SerialRead
*/
enum RobotStateEnum{Potentiometer,SerialSend,SerialRead};
RobotStateEnum RobotState=Potentiometer;
//set the initial settings
void setup()
{
  //attaches our servos on pins PWM 11-10-9-6 to the servos
  servo1.attach(11);
  servo2.attach(10);
  servo3.attach(9);
  servo4.attach(6);
  Serial.begin(9600);
  Serial.println("Serial port connected!");
}
//Repeating part of the code
void loop()
{
  readSerialPortSetState();
  switch (RobotState)
  {
  case Potentiometer:
    usePotentiometerValues();
    break;
  case SerialSend:
    sendDataToSerialPort();
    break;
  case SerialRead:
    useSerialportValues();
    break;
    }  
  //
  servo1.write(valPot1);                      //set the servo position according to the scaled value
  delay(25);
  servo2.write(valPot2);
  delay(25);
  servo3.write(valPot3);
  delay(25);
  servo4.write(valPot4);
  delay(5);
  Serial.flush();
}

// Sets the value of the servos according to potentionmeter values
void usePotentiometerValues(){
  //reads the value of potentiometers (value between 0 and 1023)
  valPot1 = analogRead(pot1);
  valPot1 = map (valPot1, 0, 1023, 0, 180);   //scale it to use it with the servo (value between 0 and 180)
  valPot2 = analogRead(pot2);
  valPot2 = map (valPot2, 0, 1023, 0, 180);
  valPot3 = analogRead(pot3);
  valPot3 = map (valPot3, 0, 1023, 0, 180);
  valPot4 = analogRead(pot4);
  valPot4 = map (valPot4, 0, 1023, 0, 180);
}

//Reads the data from the serial port, process them and set the servo position values based on them
void readSerialPortSetState(){
  if(Serial.available()){

    message=Serial.read();
    message ="110,120,40,30";   
    if ((message==NULL)) return;
    if(message.equals("1\n")==0){
      RobotState=Potentiometer;
      Serial.write("Potentiometer\n");
    }
    else if(message.equals("2\n")==0)
      {
        RobotState=SerialSend;
        Serial.write("SerialSend\n");
      }
    else if(message.equals("3\n")==0)
      {
        RobotState=SerialRead;
        Serial.write("SerialRead\n");
      }
  }
}

// use serial port values to set servo motors
void useSerialportValues(){
  char* token;
  //get the first token
  int index = 0;
  int messageLength=message.length();
  char charBuf[messageLength + 1];
  message.toCharArray(charBuf,messageLength);
  token=strtok(charBuf, delimiter);
  bool error;
  while (token != NULL)
  {
    error=false;
    if(is_number(token)){
      valueList[index]=atoi(token);
      Serial.write(valueList[index]);
      index++;
    }
    else
    {
      error=true;
      break;
    }
    token = strtok (NULL, delimiter);
  }
  if (error) 
  {
    //Serial.write("error");
    return;
  }
  
  valPot1=valueList[0];
  valPot2=valueList[1];
  valPot3=valueList[2];
  valPot4=valueList[3];
}

// Reads the servo position and send them to the serial port
void sendDataToSerialPort(){
  usePotentiometerValues();
  Serial.println(valPot1 + separator+valPot2+separator+valPot3+separator+valPot4);
}
bool is_number(const char* str)
{
  bool result=true;
  for(char* it = str; *it; ++it) {
    if(!isdigit(*it)) 
    {
      result=false;
      break;
    }
  }
  return result;
}

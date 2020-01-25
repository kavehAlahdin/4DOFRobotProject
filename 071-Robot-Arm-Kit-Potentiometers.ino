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
String serialResponse="";
char delimiter[]=",\n ";

int valueList[]={0,0,0,0};
/*set the state: 
  1-use only potentiometer values:potentioRead
  2-send the servo state to serial port:SerialSend
  3-read data from Serial port :SerialRead
*/
enum RobotStateEnum{Potentiometer=1,SerialSend=2,SerialRead=3};
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
    setServoMotos();
    break;
  case SerialSend:
    sendDataToSerialPort();
    break;
  case SerialRead:
    useSerialportValues();
    setServoMotos();
    break;
    } 
     
  //
  //Serial.println((is_number("2000"))?"Dorost":"Ghalat");
  Serial.flush();
}
void setServoMotos(){
  servo1.write(valPot1);//set the servo position according to the scaled value
  delay(25);
  servo2.write(valPot2);
  delay(25);
  servo3.write(valPot3);
  delay(25);
  servo4.write(valPot4);
  delay(25);
  serialResponse="";
  //Serial.flush();
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
 if ( Serial.available()) {
    serialResponse = String(Serial.readStringUntil('\r\n'));
    if ((serialResponse==NULL)||(serialResponse.length()>1)) return;
    if(serialResponse=="1")
      RobotState=Potentiometer;
    else if(serialResponse=="2")
        RobotState=SerialSend;
    else if(serialResponse=="3")
        RobotState=SerialRead;
    Serial.println(RobotState);
   }
   serialResponse="";
}
bool error=false;

// use serial port values to set servo motors
void useSerialportValues(){
    if (serialResponse.length()==0||serialResponse.length()<7) return;
    char buf[serialResponse.length()+1];
    int index = 0;
    //Serial.println(serialResponse);
    serialResponse.toCharArray(buf, sizeof(buf));
    char *p = buf;
    char *str;
    error=false;
    while ((str = strtok_r(p, delimiter, &p)) != NULL&&index<4)
    {
      //Serial.println(strcat(str, " isNumber: "+(is_number(str))?" true":" false"));
      if(is_number(str)){
        valueList[index]=atoi(str);
        //Serial.println("parsed");     
        Serial.println(valueList[index]);
        index++;     
      }
      else
      {
        error=true;
        //Serial.println("43");
        break;
      }         
    }
    
    if(index!=4){
      error =true;
      //Serial.println("E3,"+index);
    }
    if (error) return;
    
    valPot1=valueList[0];
    valPot2=valueList[1];
    valPot3=valueList[2];
    valPot4=valueList[3];
  
  /*Serial.println("----");
  for(int i=0;i<=3;i++){ 
    Serial.print(valueList[i]);
    Serial.print(",");
  }
  Serial.println("----");
  */
  serialResponse="";
}

// Reads the servo position and send them to the serial port
void sendDataToSerialPort(){
  usePotentiometerValues();
  Serial.println(valPot1 + separator+valPot2+separator+valPot3+separator+valPot4);
}
bool is_number(const char* str)
{
  bool result=true;
  for(size_t i=0; i<strlen(str);i++) {
    
    if(!isdigit(str[i])) 
    {
      //Serial.println(str[i]+ "is not digit");
      result=false;
      break;
    }
  }
  return result;
}

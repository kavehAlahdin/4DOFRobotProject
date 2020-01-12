//add servo library
#include <Servo.h>
#include <string.h>
#include <stdio.h>
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
String separator=", ";
char* message="";
char delimiter[]=",\n";
char **pch;
int valueList[]={0,0,0,0};
void loop()
{
  //reads the value of potentiometers (value between 0 and 1023)
  valPot1 = analogRead(pot1);
  valPot1 = map (valPot1, 0, 1023, 0, 180);   //scale it to use it with the servo (value between 0 and 180)
  
  valPot2 = analogRead(pot2);
  valPot2 = map (valPot2, 0, 1023, 0, 180);

  valPot3 = analogRead(pot3);
  valPot3 = map (valPot3, 0, 1023, 0, 180);

  valPot4 = analogRead(pot4);
  valPot4 = map (valPot4, 0, 1023, 0, 180);
  if(Serial.available()){
    message=Serial.read();    
  }
  
  *pch=strtok(message, delimiter);

  while (*pch != NULL)
  {
    printf ("%s\n",*pch);
    *pch = strtok (NULL, delimiter);
    *pch++;
  }
  for (int i=0;i<4;i++)
  {
      valueList[i]=atoi(pch[i]);
  }
  valPot1=valueList[0];
  valPot2=valueList[1];
  valPot3=valueList[2];
  valPot4=valueList[3];
  //
  servo1.write(valPot1);                      //set the servo position according to the scaled value
  delay(25);
  servo2.write(valPot2);
  delay(25);
  servo3.write(valPot3);
  delay(25);
  servo4.write(valPot4);
  delay(5);
  Serial.println(valPot1 + separator+valPot2+separator+valPot3+separator+valPot4);
  
}

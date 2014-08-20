const int LED_GREEN = A3;
const int LED_YELLOW = A2;
const int LED_RED = A1;
const int LED_BROKEN = A0;

void setup() {
  // set the digital pin as output:
  pinMode(0, OUTPUT);      
  pinMode(1, OUTPUT);
  pinMode(2, OUTPUT);
  pinMode(3, OUTPUT);
  pinMode(4, OUTPUT);
  pinMode(5, OUTPUT);
  pinMode(6, OUTPUT);
  pinMode(7, OUTPUT);
  pinMode(8, OUTPUT);
  pinMode(9, OUTPUT);
  pinMode(10, OUTPUT);
  pinMode(11, OUTPUT);
  pinMode(12, OUTPUT);
  pinMode(13, OUTPUT);
  pinMode(A0, OUTPUT);
  pinMode(A1, OUTPUT);
  pinMode(A2, OUTPUT);
  pinMode(A3, OUTPUT);
  pinMode(A4, OUTPUT);
  pinMode(A5, OUTPUT);
  Serial.begin(9600);
}


long currentProgress = 0;
int lastBuildState = 2;
int currentBuildState = 0;
int GR_STATE = LOW;
int YE_STATE = LOW;
int RE_STATE = LOW;
int broken = 0;
int currentBlinkState = LOW;

void loop()
{
  while(true) {
  Serial.flush();
  Serial.println("Starting loop");
  if(Serial.available()>=4)
  {
    Serial.println("Data avail");
    // reading percentage
    String percentString = String("");
    char inChar;
    for(int j=0;j<2;j++) {
      inChar = Serial.read();
      if(inChar != '0') {
        percentString += inChar;
      }
    }
    if(percentString != "") {
      currentProgress = percentString.toInt();
    } else {
      currentProgress = 0;
    }
    Serial.print(percentString);
    //reading current running build state
    inChar = Serial.read();
    switch(inChar) {
      case '1':
          GR_STATE = HIGH;
          YE_STATE = LOW;
          RE_STATE = LOW;
          lastBuildState = '1';
      break;
      case '2':
          GR_STATE = LOW;
          YE_STATE = HIGH;
          RE_STATE = LOW;
      break;
      case '3':
          GR_STATE = LOW;
          YE_STATE = LOW;
          RE_STATE = HIGH;
          lastBuildState = '3';
      break;
    }
    Serial.print(inChar);
    // no running builds, 
    if(inChar == '2') {
       if(lastBuildState == '1'){
         GR_STATE = HIGH;
       }
       if(lastBuildState == '3') {
         RE_STATE = HIGH;
       }
    }
    
    inChar = Serial.read();
    if(inChar == '1') {
      broken = 1;
    }
    Serial.print(inChar);
  }
  Serial.println("After data"); 
  
  digitalWrite(LED_GREEN, GR_STATE);
  digitalWrite(LED_YELLOW, YE_STATE);
  digitalWrite(LED_RED, RE_STATE);
  
  if(broken != 1) {
    digitalWrite(LED_BROKEN, LOW);
  }
  
  if(currentBlinkState == LOW) {
      currentBlinkState = HIGH;
    } else {
      currentBlinkState = LOW;
  }
  
  
  if(broken == 1) {
    digitalWrite(LED_BROKEN, currentBlinkState);
  }
  Serial.println("Make devision");
  Serial.println(currentProgress);
  for(long i=2;i<13;i++)
  {
    if(i<currentProgress+2) {
      digitalWrite(i,HIGH);
    } else
    if((i==currentProgress+2) && (currentProgress != 0) ) {
      digitalWrite(i,currentBlinkState);
    } else {
      digitalWrite(i,LOW);
    }
    
  }  
  Serial.println("Dealying");
  delay(1000);   
    Serial.println("Done");
}
}




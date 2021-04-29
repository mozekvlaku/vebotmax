import socket
import time
import struct
import sys
import atexit
from easing_functions import *

from adafruit_servokit import ServoKit
kit = ServoKit(channels=16)
from struct import *
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_address = ('192.168.31.221', 13000)
print ('veBot using Siphona, Siphona Server: %s port %s' % server_address)
sock.bind(server_address)
sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR,1)
sock.listen(1)
easing = False

def exit_handler():
    csReset()
    closeLids()
    mouth(3)

mouthphom = [[130,90,30,30,90,80],
[90,0,180,90,90,0],
[90,0,180,90,90,120],
[90,0,180,90,90,120],
[180,90,0,180,90,80],
[90,0,60,90,150,50],
[90,0,90,90,150,50],
[90,0,50,90,150,50],
[90,0,50,90,150,50],
[180,0,180,180,90,90],
[180,0,180,180,90,180],
[90,0,90,90,150,50],
[90,0,90,90,150,50],
[90,20,150,90,150,50],
[90,0,90,90,150,50],
[90,20,30,90,150,90],
[90,20,150,90,150,50],
[90,0,90,90,150,50],
[90,0,90,90,150,50],
[90,90,90,150,0,90],
[90,0,90,90,150,50],
[90,0,90,90,150,50],
[90,20,50,90,150,90],
[90,0,90,90,150,50],
[90,20,50,90,150,90],
[90,90,90,150,0,90],
[90,90,90,150,0,90],
[90,90,90,150,0,90]]

atexit.register(exit_handler)
def directControl(bytes):
    servodeg = extract(bytes,9, 0)
    servonum = extract(bytes,4, 9)
    moveServo(servonum,servodeg)

def actions(bytes):
    incomingByte = extract(bytes,9,0)
    attr = extract(bytes, 4, 9)
    if incomingByte == 1:
        csUsmev()
    elif incomingByte == 19:
        csReset()
    elif incomingByte == 23:
        csNasranost()
    elif incomingByte == 2:
        csSmutek()
    elif incomingByte == 3:
        csPrekvapeni()
    elif incomingByte == 20:
        csStesti()
    elif incomingByte == 21:
        csSexy()
    elif incomingByte == 22:
        csZnechuceni()
    elif incomingByte == 16:
        csSpanek()
    elif incomingByte == 4:
        blinkEyes()
    elif incomingByte == 14:
        smile()
    elif incomingByte == 17:
        resetMouth()
    elif incomingByte == 18:
        resetEyes()
    elif incomingByte == 5:
        closeLids()
    elif incomingByte == 6:
        openLids()
    elif incomingByte == 25:
        envyLook()
    elif incomingByte == 12:
        sonnyBlink()
    elif incomingByte == 24:
        mouth(attr)
    elif incomingByte == 9:
        eyex(attr)
    elif incomingByte == 10:
        eyey(attr)
    elif incomingByte == 7:
        easing = True
    elif incomingByte == 8:
        easing = False
    elif incomingByte == 26:
        eyediagleft(attr)
    elif incomingByte == 27:
        eyediagright(attr)
    elif incomingByte == 13:
        eyelids(attr)
    elif incomingByte == 28:
        browleft(attr)
        browright(attr)
    elif incomingByte == 29:
        browleft(attr)
    elif incomingByte == 30:
        browright(attr)

def extract(num,k,p):
    binary = bin(num)
    binary = binary[2:] 
    end = len(binary) - p 
    start = end - k + 1
    kBitSubStr = binary[start : end+1] 
    return (int(kBitSubStr,2))

def moveServo(servonum,servodegrees):
    print("Servo "+str(servonum)+" to " + str(servodegrees) + " degrees")
    try:
        nowdegrees = int(kit.servo[servonum].angle)
        easetodegrees = servodegrees
        rate = 500000
        if easing:
            if nowdegrees > easetodegrees:
                duration = nowdegrees - easetodegrees
                a = QuadEaseInOut(start=easetodegrees,
                                  end=nowdegrees, duration=duration)
                for x in range(nowdegrees, easetodegrees, -1):
                    kit.servo[servonum].angle = x
                    time.sleep(a(x)/rate)
            else:
                duration = easetodegrees - nowdegrees
                a = QuadEaseInOut(start=nowdegrees,
                                  end=easetodegrees, duration=duration)
                for x in range(nowdegrees, easetodegrees):
                    kit.servo[servonum].angle = x
                    time.sleep(a(x)/rate)
        else:
            kit.servo[servonum].angle = servodegrees
    except Exception as e:
        print(e)
        print("!")

def pronounce(byte):
    incomingByte = extract(byte,9, 0)
    try:
        setServoDegrees(mouthphom[int(incomingByte)][0], 6)
        setServoDegrees(mouthphom[int(incomingByte)][1], 7)
        setServoDegrees(mouthphom[int(incomingByte)][2], 8)
        setServoDegrees(mouthphom[int(incomingByte)][3], 9)
        setServoDegrees(mouthphom[int(incomingByte)][4], 10)
        setServoDegrees(mouthphom[int(incomingByte)][5], 11)
    except:
        print("dfg")

def setServoDegrees(deg,num):
    moveServo(num,deg)

def checkParity(bytes):
    num = extract(bytes,16,0)
    try:
        parity = extract(bytes, 1, 16)
    except:
        parity = 0

    if (num % 2) == 0:
        even = True
    else:
        even = False

    if parity == 1 and even or parity == 0 and even == False:
        return True
    elif parity == 0 and even or parity == 1 and even == False:
        return False


def blinkEyes():
    closeLids()
    time.sleep(0.1)
    openLids()


def lookUp():
    kit.servo[4].angle = 110


def lookDown():
    kit.servo[4].angle = 170


def lookLeft():
    setServoDegrees(140, 5)


def lookRight():
    setServoDegrees(40, 5)


def lookUpLeft():
    lookUp()
    lookLeft()


def lookUpRight():
    lookUp()
    lookRight()


def lookDownLeft():
    lookDown()
    lookLeft()


def lookDownRight():
    lookDown()
    lookRight()


def csUsmev():
    csReset()
    setServoDegrees(180, 7)
    setServoDegrees(0, 10)
    setServoDegrees(150, 11)
    setServoDegrees(40, 13)
    setServoDegrees(160, 14)


def csNasranost():
    csReset()
    setServoDegrees(90, 0)
    setServoDegrees(90, 1)
    setServoDegrees(90, 2)
    setServoDegrees(90, 3)
    setServoDegrees(150, 4)
    setServoDegrees(90, 5)
    setServoDegrees(180, 6)
    setServoDegrees(150, 7)
    setServoDegrees(20, 8)
    setServoDegrees(90, 9)
    setServoDegrees(90, 10)
    setServoDegrees(150, 11)
    setServoDegrees(180, 13)
    setServoDegrees(0, 14)


def csSmutek():
    csReset()
    setServoDegrees(60, 1)
    setServoDegrees(120, 2)
    setServoDegrees(180, 6)
    setServoDegrees(0, 9)


def csPrekvapeni():
    csReset()
    openMouth()
    setServoDegrees(0, 0)
    setServoDegrees(180, 1)
    setServoDegrees(20, 2)
    setServoDegrees(1800, 3)
    setServoDegrees(50, 8)
    setServoDegrees(130, 11)
    setServoDegrees(10, 13)
    setServoDegrees(170, 14)


def csStesti():
    csReset()
    setServoDegrees(180, 7)
    setServoDegrees(0, 13)
    setServoDegrees(180, 14)


def csSexy():
    csReset()
    setServoDegrees(180, 7)
    time.sleep(0.1)
    sonnyBlink()


def csZnechuceni():
    csReset()
    setServoDegrees(50, 1)
    setServoDegrees(120, 2)
    setServoDegrees(0, 8)


def csSpanek():
    csReset()
    setServoDegrees(180, 6)
    closeLids()


def csReset():
    resetEyes()
    resetMouth()
    mouth(0)

def openLids():
    setServoDegrees(10, 0)
    setServoDegrees(50, 2)
    setServoDegrees(120, 1)
    setServoDegrees(110, 3)


def closeLids():
    setServoDegrees(120, 2)
    setServoDegrees(55, 1)
    setServoDegrees(50, 3)
    setServoDegrees(120, 0)


def mouth(attr):
    degs = 12*attr
    setServoDegrees(degs, 15)

def eyex(attr):
    degs = 12*attr
    setServoDegrees(degs, 5)

def eyey(attr):
    degs = 12*attr
    setServoDegrees(degs, 4)

def openMouth():
    setServoDegrees(120, 15)


def slightlyOpenMouth():
    setServoDegrees(60, 15)


def envyLook():
    setServoDegrees(110, 2)
    setServoDegrees(60, 1)
    setServoDegrees(50, 3)
    setServoDegrees(110, 0)


def dementLook():
    setServoDegrees(110, 2)
    setServoDegrees(75, 0)
    setServoDegrees(105, 1)
    setServoDegrees(50, 3)


def sonnyBlink():
    setServoDegrees(60, 1)
    setServoDegrees(120, 0)
    time.sleep(0.300)
    setServoDegrees(10, 0)
    setServoDegrees(120, 1)


def dodgeFace():
    setServoDegrees(60, 1)
    setServoDegrees(120, 2)


def vykulOci():
    setServoDegrees(0, 13)
    setServoDegrees(180, 14)


def resetEyes():
    setServoDegrees(10, 0)
    setServoDegrees(120, 1)
    setServoDegrees(50, 2)
    setServoDegrees(110, 3)
    setServoDegrees(150, 4)
    setServoDegrees(90, 5)
    setServoDegrees(180, 13)
    setServoDegrees(0, 14)


def smile():
    setServoDegrees(40, 10)
    setServoDegrees(120, 7)


def disgust():
    setServoDegrees(180, 8)
    setServoDegrees(20, 9)


def hiGeorgie():
    smile()
    setServoDegrees(180, 11)


def resetMouth():
    setServoDegrees(150, 10)
    setServoDegrees(20, 7)
    setServoDegrees(150, 8)
    setServoDegrees(90, 9)
    setServoDegrees(0, 11)
    setServoDegrees(90, 6)

def eyelids(attr):
    degs0 = 120-(11*attr)
    degs1 = 55+(6.5*attr)
    degs2 = 120-(11*attr)
    degs3 = 50+(7*attr)
    setServoDegrees(degs0, 0)
    setServoDegrees(degs1, 1)
    setServoDegrees(degs2, 2)
    setServoDegrees(degs3, 3)

def eyediagleft(attr):
    degs1 = 0
    degs2 = 12*attr
    setServoDegrees(degs1, 5)
    setServoDegrees(degs2, 4)

def eyediagright(attr):
    degs1 = 180
    degs2 = 12*attr
    setServoDegrees(degs1, 5)
    setServoDegrees(degs2, 4)

def browleft(attr):
    degs = 180-(12*attr)
    setServoDegrees(degs, 13)

def browright(attr):
    degs = 12*attr
    setServoDegrees(degs, 14)

while True:
    print ('Waiting for Siphona connection')
    blinkEyes()
    connection, client_address = sock.accept()
    try:
        print ('Siphona connection from: ', client_address)
        csReset()
        time.sleep(0.300)
        csUsmev()
        time.sleep(0.300)
        blinkEyes()
        time.sleep(0.500)
        csReset()
        # Receive the data in small chunks and retransmit it
        while True:
            s = struct.Struct('>I')
            data = connection.recv(2)
            if data:
                data = s.pack(int.from_bytes(data, "little"))
                printable = bytes(data)
                print ('Received 0x' + printable.hex())
                #print ('In Binary ' + bin(int(printable.hex(),16)))
                byts = int.from_bytes(data, "big")
                action = extract(byts, 2, 13)
                if checkParity(byts):
                    if action == 0b01:
                        directControl(byts)
                    elif action == 0b00:
                        actions(byts)
                    elif action == 0b10:
                        pronounce(byts)
                else:
                    print("Parity error")

            else:
                print ('no more data from', client_address)
                
                setServoDegrees(70,12)
                break
    finally:
        connection.close()


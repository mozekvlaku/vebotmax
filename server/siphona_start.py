from struct import *
import socket
import time
import struct
import sys
import atexit


from adafruit_servokit import ServoKit
kit = ServoKit(channels=16)


def moveServo(servonum, servodegrees):
    print("ψ(｀∇´)ψ Moving servo "+str(servonum) +
          " precisely to " + str(servodegrees) + "° degrees")
    try:
        kit.servo[servonum].angle = servodegrees
    except:
        print("!")




def setServoDegrees(deg, num):
    moveServo(num, deg)



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
    setServoDegrees(40, 3)
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


csReset()
time.sleep(0.5)
csUsmev()
time.sleep(0.5)
csReset()
time.sleep(0.5)
csSmutek()
time.sleep(0.5)
csReset()

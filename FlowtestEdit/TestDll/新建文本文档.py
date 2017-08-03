import clr
clr.AddReferenceToFile("TestDll.dll")
from TestDll import *

a=12
b=16
c=TestDll.Add(a,b)
print c

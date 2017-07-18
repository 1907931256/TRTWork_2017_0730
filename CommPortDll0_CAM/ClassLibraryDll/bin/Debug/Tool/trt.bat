@echo off
adb wait-for-device
@mkdir c:\\trt
@mkdir c:\\trt\\app
@mkdir c:\\trt\\framework
@adb pull /system/app c:\\trt\\app
@adb pull /system/framework c:\\trt\\framework
@echo "请手工压缩 C:\\trt 目录"
@echo "请拷贝trt压缩文件，并删除trt目录"

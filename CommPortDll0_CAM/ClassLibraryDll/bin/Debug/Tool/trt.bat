@echo off
adb wait-for-device
@mkdir c:\\trt
@mkdir c:\\trt\\app
@mkdir c:\\trt\\framework
@adb pull /system/app c:\\trt\\app
@adb pull /system/framework c:\\trt\\framework
@echo "���ֹ�ѹ�� C:\\trt Ŀ¼"
@echo "�뿽��trtѹ���ļ�����ɾ��trtĿ¼"

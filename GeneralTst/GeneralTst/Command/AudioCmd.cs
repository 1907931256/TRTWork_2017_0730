using GeneralTst.SoundCard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;

namespace GeneralTst.Command
{
    public class AudioCmd
    {
        private const string ACTION_RECORD_START = "录音";

        private const string ACTION_RECORD_STOP = "停止录音";

        private const string ACTION_PLAY = "播放声音";

        private const string ACTION_STOP = "停止播放";

        private SoundPlayer soundPlayer_;

        private SoundCardControl SoundCardControl_;

        private static AudioCmd instance_;

        public static AudioCmd Instance
        {
            get
            {
                if (AudioCmd.instance_ == null)
                {
                    AudioCmd.instance_ = new AudioCmd();
                }
                return AudioCmd.instance_;
            }
        }

        private AudioCmd()
        {
            this.SoundCardControl_ = new SoundCardControl();
        }

        public void ExecuteCmd(string action, string param, out string retValue)
        {
            if ("播放声音" == action)
            {
                this.ExecutePlay(param, out retValue);
                return;
            }
            if ("停止播放" == action)
            {
                this.ExecuteStop(param, out retValue);
                return;
            }
            if ("录音" == action)
            {
                this.ExecuteRecordStart(param, out retValue);
                return;
            }
            if ("停止录音" == action)
            {
                this.ExecuteRecordStop(param, out retValue);
                return;
            }
            retValue = "Res=CmdNotSupport";
        }

        private void ExecuteRecordStart(string param, out string retValue)
        {
            this.SoundCardControl_.Start(param);
            retValue = "Res=Pass";
        }

        private void ExecuteRecordStop(string param, out string retValue)
        {
            this.SoundCardControl_.Stop();
            retValue = "Res=Pass";
        }

        private void ExecutePlay(string param, out string retValue)
        {
            try
            {
                this.soundPlayer_ = new SoundPlayer(param);
                this.soundPlayer_.LoadAsync();
                this.soundPlayer_.Play();
                retValue = "Res=Pass";
            }
            catch (UriFormatException)
            {
                retValue = "Res=ArgumentException";
            }
            catch (FileNotFoundException)
            {
                retValue = "Res=FileNotFound";
            }
        }

        private void ExecuteStop(string param, out string retValue)
        {
            this.soundPlayer_.Stop();
            retValue = "Res=Pass";
        }



    }
}

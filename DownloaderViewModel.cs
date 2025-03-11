using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace YoutubeDonloader
{
    public class DownloaderViewModel : BaseViewModel
    {
        YoutubeDL ytdl = new YoutubeDL();
        
        

        ObservableCollection<OrderModel> _orders = new();
        public ObservableCollection<OrderModel> Orders
        { 
            get { return _orders; }
            set
            {
                _orders = value;
                OnPropertyChanged(nameof(Orders));
            }
        }

        public string currentArtist { get; set; } = "Unknown";
        public string currentAlbum { get; set; } = "Unknown";
        public string currentUrl { get; set; } = "";
        public string currentPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
        public string currentPlaylistFrom { get; set; } = "";
        public string currentPlaylistTo { get; set; } = "";
        public bool SetupComplete { get; set; } = false;

        public ICommand downloadCommand { get; set; }
        public ICommand setCurrentPathCommand { get; set; }

        public DownloaderViewModel(){
            this.downloadCommand = new RelayCommand(o => Download(), o => true);
            this.setCurrentPathCommand = new RelayCommand(o => SetPath(), o => true);
            Setup();
           
        }

        private void Setup()
        {
            if(!System.IO.File.Exists(System.IO.Directory.GetCurrentDirectory() + "\\yt-dlp.exe"))
            {
                YoutubeDLSharp.Utils.DownloadYtDlp();
            }
            if (!System.IO.File.Exists(System.IO.Directory.GetCurrentDirectory() + "\\ffmpeg.exe"))
            {
                YoutubeDLSharp.Utils.DownloadFFmpeg();
            }
           
            this.SetupComplete = true;
        }

        private void SetPath()
        {
            var dialog = new OpenFolderDialog(){
                Title = "Select folder to save the music",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                Multiselect = false,
               
            };
            if (dialog.ShowDialog() == true){
                this.currentPath = dialog.FolderName;
                OnPropertyChanged(nameof(currentPath));
                Trace.WriteLine(dialog.FolderName);
            }
        }



        async void Download()
        {


            OrderModel order = new(currentUrl, currentArtist, currentAlbum);
            order.setToFrom(this.currentPlaylistFrom, currentPlaylistTo);
            Orders.Add(order);
            OnPropertyChanged(nameof(Orders));
            string metadataString = $"-metadata artist=\'{currentArtist}\' -metadata album_artist=\'{currentArtist}\' -metadata album=\'{currentAlbum}\'";
            var options = new OptionSet()
            {
                Output = currentPath + "\\%(title)s.%(ext)s",
                MatchFilters = "duration <= 7200",
                EmbedThumbnail = true,
                NoPlaylist = true,
                EmbedMetadata = true,
                PostprocessorArgs = metadataString
            };
            order.path = currentPath;
           

            if (order.type == orderType.song)
            {
                var result = await ytdl.RunAudioDownload(
                    currentUrl,
                    AudioConversionFormat.Mp3,
                    overrideOptions: options);
                order.status = orderStatus.done;
            }
            else
            {
                var result = await ytdl.RunAudioPlaylistDownload(
                    currentUrl,
                    format: AudioConversionFormat.Mp3,
                    overrideOptions: options,
                    start: order.playlistFrom, end: order.playlistTo
                   
                    );
                order.status = orderStatus.done;
               
            }
            OnPropertyChanged(nameof(Orders));
        }

        
    }


}

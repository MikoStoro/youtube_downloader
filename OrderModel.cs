using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDonloader
{
    public enum orderType { song, playlist }
    public enum orderStatus { in_progress, done }
    public class OrderModel
    {
        public string link { get; set; }
        public orderType type { get; set; }
        public string artist { get; set; }
        public string album { get; set; }
        public string path { get; set; }
        public orderStatus status { get; set; } =  orderStatus.in_progress;
        public int playlistFrom { get; set; } = 1;
        public int? playlistTo { get; set; } = null;


        public OrderModel(string link, string artist, string album)
        {
            this.link = link;
            this.artist = artist;
            this.album = album;

            if(this.album == "") { this.album = "Unknown"; }
            if(this.artist == "") { this.artist = "Unknown"; }

            if (link.Contains("watch")){
                this.type = orderType.song;
            }else { this.type = orderType.playlist;}
        }

        public void setToFrom(string currentPlaylistFrom, string currentPlaylistTo)
        {
            if (this.type == orderType.song) { return; }
            else
            {
                try{
                    this.playlistFrom = int.Parse(currentPlaylistFrom);
                }catch (Exception e) {  }

                try{
                    this.playlistTo = int.Parse(currentPlaylistTo);
                }catch (Exception e) { }
            }

        }
    }


}

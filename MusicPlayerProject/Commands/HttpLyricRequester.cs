using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Popups;

namespace MusicPlayerProject.Commands
{
    public class HttpLyricRequester
    {
        public static async Task<string> GetLyrics(string artist, string song)
        {
            try
            {
                bool isNetworkConnected = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
                if (!isNetworkConnected)
                {
                    MessageDialog dlg = new MessageDialog("You are not connected to the internet. Please connect if you want to download lyrics");
                    await dlg.ShowAsync();
                    throw new Exception("No internet connection");
                }
                string artistUri = Uri.EscapeUriString(artist);
                string songUri = Uri.EscapeUriString(song);
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://api.chartlyrics.com/apiv1.asmx/SearchLyricDirect?artist=" + artistUri + "&song=" + songUri);
                var response = await client.GetAsync("");//"artist=" + artistUri + "&song=without%me"); //+ songUri);

                var responseText = await response.Content.ReadAsStringAsync();

                XmlDocument lyricResults = new XmlDocument();
                lyricResults.LoadXml(responseText);
                var lyrics = lyricResults.GetElementsByTagName("Lyric");
                string lyricsText = lyrics.Item(0).InnerText.ToString();
                if (string.IsNullOrEmpty(lyricsText))
                {
                    lyricsText = "No lyrics found for this song";
                }
                return lyricsText;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}

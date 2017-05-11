using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace libNUS.WiiU
{
    public static class NUS
    {
        public struct UrlFilenamePair
        {
            public readonly string URL;
            public readonly string Filename;
            public UrlFilenamePair(string URL, string Filename)
            {
                this.URL = URL;
                this.Filename = Filename;
            }
        }
        public static Byte[] TitleCert
        {
            get
            {
                return Properties.Resources.WiiUTitleCert;
            }
        }
        private static Regex titleKeyValidationPattern = new Regex("^[A-Fa-f0-9]{16}$");
        private static string downloadBase = "http://ccs.cdn.wup.shop.nintendo.net/ccs/download/";

        public static bool IsTitleIDFormatValid(string TitleID)
        {
            return titleKeyValidationPattern.IsMatch(TitleID);
        }
        public static string GetUpdateID(string TitleID)
        {
            if (!IsTitleIDFormatValid(TitleID))
                throw new ArgumentException("Invalid format.", "TitleID");

            char[] chars = TitleID.ToLower().ToCharArray();
            chars[7] = "e"[0];
            return new String(chars);
        }
        public static async Task<bool> TitleExists(string TitleID)
        {
            if (!IsTitleIDFormatValid(TitleID))
                throw new ArgumentException("Invalid format.", "TitleID");

            return await HelperFunctions.FileExistsAtURL(downloadBase + TitleID + "/tmd");
        }
        public static async Task<TMD> DownloadTMD(string TitleID)
        {
            if (!IsTitleIDFormatValid(TitleID))
                throw new ArgumentException("Invalid format.", "TitleID");

            using (WebClient client = new WebClient())
            {
                TMD tmd = new TMD(await client.DownloadDataTaskAsync(downloadBase + TitleID + "/tmd"));
                return tmd;
            }
        }
        public static async Task<bool> TicketExists(string TitleID)
        {
            if (!IsTitleIDFormatValid(TitleID))
                throw new ArgumentException("Invalid format.", "TitleID");

            return await HelperFunctions.FileExistsAtURL(downloadBase + TitleID + "/cetk");
        }
        public static async Task<byte[]> DownloadTicket(string TitleID)
        {
            if (!IsTitleIDFormatValid(TitleID))
                throw new ArgumentException("Invalid format.", "TitleID");

            using (WebClient client = new WebClient())
            {
                byte[] ticket = await client.DownloadDataTaskAsync(downloadBase + TitleID + "/cetk");
                return ticket;
            }
        }
        public static async Task<UrlFilenamePair[]> GetTitleContentURLs(TMD tmd, bool skipTMD = false)
        {
            var URLs = new List<UrlFilenamePair> { };
            if (await TitleExists(tmd.TitleID))
            {
                if(skipTMD == false)
                    URLs.Add(new UrlFilenamePair(downloadBase + tmd.TitleID + "/tmd","title.tmd"));

                if (await TicketExists(tmd.TitleID))
                {
                    URLs.Add(new UrlFilenamePair(downloadBase + tmd.TitleID + "/cetk","title.tik"));
                }
                foreach (var content in tmd.Content)
                {
                    URLs.Add(new UrlFilenamePair(downloadBase + tmd.TitleID + "/" + content.IDString, content.IDString + ".app"));

                    if(content.HasH3)
                        URLs.Add(new UrlFilenamePair(downloadBase + tmd.TitleID + "/" + content.IDString + ".h3", content.IDString + ".h3"));
                }
            }
            return URLs.ToArray();
        }
        public static async Task<UrlFilenamePair[]> GetTitleContentURLs(string TitleID, bool throwError = false)
        {
            if (!IsTitleIDFormatValid(TitleID))
                throw new ArgumentException("Invalid format.", "TitleID");

            try
            {
                TMD tmd = await DownloadTMD(TitleID);
                return await GetTitleContentURLs(tmd);
            } catch(WebException ex)
            {
                if (throwError)
                    throw ex;
            }
            return new UrlFilenamePair[] { };
        }
    }
}

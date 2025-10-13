#region

using db;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace server.app
{
    internal class globalNews : RequestHandler
    {
        protected override async Task HandleRequest()
        {
            string s = "[";
            
            var toSerialize = GetGlobalNews();
            int len = toSerialize.Count;
        
            for (int i = 0; i < len; i++)
            {
                if (toSerialize.Count > 1)
                    s += JsonConvert.SerializeObject(toSerialize[0]) + ",";
                else
                    s += JsonConvert.SerializeObject(toSerialize[0]);
                toSerialize.RemoveAt(0);
            }
            s += "]";

            byte[] buf = Encoding.UTF8.GetBytes(s);
            Context.Response.OutputStream.Write(buf, 0, buf.Length);
            await Task.CompletedTask;
        }

        private List<globalNews_struct> GetGlobalNews()
        {
            return new List<globalNews_struct>();
        }
    }

    public struct globalNews_struct
    {
        public int slot;
        public int linkType;
        public string title;
        public string image;
        public int priority;
        public string linkDetail;
        public string platform;
        public long startTime;
        public long endTime;
    }
}
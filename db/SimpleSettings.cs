#region

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;

#endregion

namespace db
{
    public class SimpleSettings : IDisposable
    {
        private ILogger _logger;

        private readonly string cfgFile;
        private readonly string id;
        private readonly Dictionary<string, string> values;

        public SimpleSettings(string id, ILogger logger)
        {
            _logger = logger;
            _logger.LogInformation("Loading settings for '{ID}'...", id);

            values = new Dictionary<string, string>();
            this.id = id;
            cfgFile = Path.Combine(Environment.CurrentDirectory, id + ".cfg");
            if (File.Exists(cfgFile))
                using (StreamReader rdr = new StreamReader(File.OpenRead(cfgFile)))
                {
                    string line;
                    int lineNum = 1;
                    while ((line = rdr.ReadLine()) != null)
                    {
                        if (line.StartsWith("#")) continue;
                        int i = line.IndexOf(":");
                        if (i == -1)
                        {
                            _logger.LogInformation("Invalid settings at line {LineNumber}.", lineNum);
                            throw new ArgumentException("Invalid settings.");
                        }

                        string val = line.Substring(i + 1);

                        values.Add(line.Substring(0, i),
                            val.Equals("null", StringComparison.InvariantCultureIgnoreCase) ? null : val);
                        lineNum++;
                    }

                    _logger.LogInformation("Settings loaded.");
                }
            else
                _logger.LogInformation("Settings not found.");
        }

        public void Reload()
        {
            _logger.LogInformation("Reloading settings for '{ID}'...", id);
            values.Clear();
            if (File.Exists(cfgFile))
                using (StreamReader rdr = new StreamReader(File.OpenRead(cfgFile)))
                {
                    string line;
                    int lineNum = 1;
                    while ((line = rdr.ReadLine()) != null)
                    {
                        if (line.StartsWith("#")) continue;
                        int i = line.IndexOf(":");
                        if (i == -1)
                        {
                            _logger.LogInformation("Invalid settings at line {LineNumber}.", lineNum);
                            throw new ArgumentException("Invalid settings.");
                        }

                        string val = line.Substring(i + 1);

                        values.Add(line.Substring(0, i),
                            val.Equals("null", StringComparison.InvariantCultureIgnoreCase) ? null : val);
                        lineNum++;
                    }

                    _logger.LogInformation("Settings loaded.");
                }
            else
                _logger.LogInformation("Settings not found.");
        }

        public void Dispose()
        {
            try
            {
                _logger.LogInformation("Saving settings for '{0}'...", id);
                using (StreamWriter writer = new StreamWriter(File.OpenWrite(cfgFile)))
                    foreach (KeyValuePair<string, string> i in values)
                        writer.WriteLine("{0}:{1}", i.Key, i.Value == null ? "null" : i.Value);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error when saving settings.");
            }
        }

        public string GetValue(string key, string def = null)
        {
            string ret;
            if (!values.TryGetValue(key, out ret))
            {
                if (def == null)
                {
                    _logger.LogError("Attempt to access nonexistant settings '{Key}'.", key);
                    throw new ArgumentException(string.Format("'{0}' does not exist in settings.", key));
                }

                ret = values[key] = def;
            }

            return ret;
        }

        public T GetValue<T>(string key, string def = null)
        {
            string ret;
            if (!values.TryGetValue(key, out ret))
            {
                if (def == null)
                {
                    _logger.LogError("Attempt to access nonexistant settings '{Key}'.", key);
                    throw new ArgumentException($"'{key}' does not exist in settings.");
                }

                ret = values[key] = def;
            }

            return (T)Convert.ChangeType(ret, typeof(T));
        }

        public void SetValue(string key, string val)
        {
            values[key] = val;
        }
    }
}
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEngine;
#elif UNITY_WSA
using Windows.Storage;
using Windows.Foundation.Diagnostics;

#endif
namespace Assets.Scripts {
    public class Logs {
        private static readonly Lazy<Logs> lazy = new Lazy<Logs>(() => new Logs());
        public static           Logs       Instance => lazy.Value;

        public void WriteLogLine(string logs) {
        #if UNITY_EDITOR
            Debug.Log(logs);
                                                                        #elif UNITY_WSA
            loggingChannel.LogMessage(logs, LoggingLevel.Information);
            Task.Run(async () => {
                ss.Wait();
                await streamWriter.WriteLineAsync(logs);
                await streamWriter.FlushAsync();
                ss.Release();
            });
        #endif
        }

        public void WriteLogLine(object logs) {
        #if UNITY_EDITOR
            Debug.Log(logs);
                                                                                #elif UNITY_WSA
            loggingChannel.LogMessage(logs.ToString(), LoggingLevel.Information);
            string s = logs.ToString();
            Task.Run(async () => {
                ss.Wait();
                await streamWriter.WriteLineAsync(s);
                await streamWriter.FlushAsync();
                ss.Release();
            });
        #endif
        }

        public void WriteLogLineError(string logs) {
        #if UNITY_EDITOR
            Debug.LogError(logs);
                                        #elif UNITY_WSA
            loggingChannel.LogMessage(logs, LoggingLevel.Error);
            Task.Run(async () => {
                ss.Wait();
                await streamWriter.WriteLineAsync(logs);
                await streamWriter.FlushAsync();
                ss.Release();
            });
        #endif
        }

        public void WriteLogLineError(object logs) {
        #if UNITY_EDITOR
            Debug.LogError(logs);
                                                                                        #elif UNITY_WSA
            string s = logs.ToString();

            loggingChannel.LogMessage(logs.ToString(), LoggingLevel.Error);
            Task.Run(async () => {
                ss.Wait();
                await streamWriter.WriteLineAsync(s);
                await streamWriter.FlushAsync();
                ss.Release();
            });
        #endif
        }

        public void WriteLogLineWarning(string logs) {
        #if UNITY_EDITOR
            Debug.LogWarning(logs);
                                                #elif UNITY_WSA
            loggingChannel.LogMessage(logs, LoggingLevel.Warning);
            Task.Run(async () => {
                ss.Wait();
                await streamWriter.WriteLineAsync(logs);
                await streamWriter.FlushAsync();
                ss.Release();
            });
        #endif
        }

        public void WriteLogLineWarning(object logs) {
        #if UNITY_EDITOR
            Debug.LogWarning(logs);
                                #elif UNITY_WSA
            string s = logs.ToString();
            loggingChannel.LogMessage(logs.ToString(), LoggingLevel.Warning);
            Task.Run(async () => {
                ss.Wait();
                await streamWriter.WriteLineAsync(s);
                await streamWriter.FlushAsync();
                ss.Release();
            });
        #endif
        }

        public void WriteLog(string logs) {
        #if UNITY_EDITOR
            Debug.Log(logs);
                                                                        #elif UNITY_WSA
            loggingChannel.LogMessage(logs, LoggingLevel.Information);
            Task.Run(async () => {
                ss.Wait();
                await streamWriter.WriteAsync(logs);
                await streamWriter.FlushAsync();
                ss.Release();
            });
        #endif
        }

        public void WriteLog(object logs) {
        #if UNITY_EDITOR
            Debug.Log(logs);
                                        #elif UNITY_WSA
            string s = logs.ToString();
            loggingChannel.LogMessage(logs.ToString(), LoggingLevel.Information);
            Task.Run(async () => {
                ss.Wait();
                await streamWriter.WriteAsync(s);
                await streamWriter.FlushAsync();
                ss.Release();
            });
        #endif
        }

    #if !UNITY_EDITOR && UNITY_WSA
        private          StreamWriter       streamWriter;
        private readonly LoggingChannel     loggingChannel;
        private readonly FileLoggingSession fls;

        private readonly SemaphoreSlim ss = new SemaphoreSlim(1, 1);

        private Logs() {
            Create();
            loggingChannel = new LoggingChannel("Holo Protein", null, new Guid("4dc2826e-54a1-4ba9-bf63-92b73ea1ac4a"));
            fls            = new FileLoggingSession("Holo Protein");
            fls.AddLoggingChannel(loggingChannel);
        }

        ~Logs() {
            Task.Run(async () => {
                StorageFile file = await fls.CloseAndSaveToFileAsync();
                await streamWriter.WriteLineAsync($"Logfile: {file.Path}/{file.Name}");
                await streamWriter.FlushAsync();
                streamWriter.Dispose();
            });
        }

        private async void Create() {
            StorageFolder folder = ApplicationData.Current.TemporaryFolder;

            //if (folder.TryGetItemAsync("Logs.log") is IStorageFile tempfile)
            //    await tempfile.DeleteAsync(StorageDeleteOption.PermanentDelete);    

            StorageFile file = await folder.TryGetItemAsync("Logs.log") as StorageFile
                            ?? await folder.CreateFileAsync("Logs.log");
            //todo FIX
            Stream stream = await file.OpenStreamForWriteAsync();
            streamWriter = new StreamWriter(stream);
        }
    #endif
    }
}
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace YoutubeDownloaderWebApp.Classes
{
    public static class ProcessExtensions
    {
        public static Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default)
        {
            if (process.HasExited)
            {
                Debug.WriteLine("Process already exited.");
                return Task.CompletedTask;
            }

            var tcs = new TaskCompletionSource<object>();

            void ProcessExited(object sender, EventArgs e)
            {
                Debug.WriteLine("Process exited.");
                process.Exited -= ProcessExited;
                tcs.TrySetResult(null);
            }

            process.Exited += ProcessExited;

            if (cancellationToken != default)
            {
                cancellationToken.Register(() =>
                {
                    Debug.WriteLine("Cancellation requested.");
                    process.Exited -= ProcessExited;
                    tcs.TrySetCanceled();
                });
            }

            if (process.HasExited)
            {
                Debug.WriteLine("Process exited after event attached.");
                process.Exited -= ProcessExited;
            }

            return tcs.Task;
        }
    }
}

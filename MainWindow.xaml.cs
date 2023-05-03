using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using Path = System.IO.Path;


namespace Lab5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private int NewtonByTask_Up(int n, int k)
        {
            int result = 1;
            Task<int> taskD = Task.Factory.StartNew<int>(
                () => {
                    for (int i = 0; i < k; i++)
                    {
                        result *= (n - i);
                    }
                    return result;
                }
             );
            return taskD.Result;
        }

        private int NewtonByTask_Down(int k)
        {
            int result = 1;
            Task<int> taskD = Task.Factory.StartNew<int>(
                () => {
                    for (int i = 0; i < k; i++)
                    {
                        result *= (i + 1);
                    }
                    return result;
                }
             );
            return taskD.Result;
        }

        private void NewtonByTask(object sender, RoutedEventArgs eArgs)
        {
            int n = 10, k = 2;
            
            Task<int> up = Task.Run(() => NewtonByTask_Up(n, k));
            Task<int> down = Task.Run(() => NewtonByTask_Down(k));

            up.Wait();
            down.Wait();

            Console.WriteLine(up.Result / down.Result);
        }

        private void NewtonByDelegat(object sender, RoutedEventArgs eArgs)
        {
            int n = 10, k = 2;

            Func<int, int, int> op1 = NewtonByTask_Up;
            IAsyncResult result1 = op1.BeginInvoke(n, k, null, null);
            int up = op1.EndInvoke(result1);

            Func<int, int> op2 = NewtonByTask_Down;
            IAsyncResult result2 = op2.BeginInvoke(k, null, null);
            int down = op2.EndInvoke(result2);

            Console.WriteLine(up/down);
        }

        private static async Task<int> NewtonByAvait_Up(int n, int k)
        {
            int result = 1;
            Task<int> taskD = Task.Factory.StartNew<int>(
                () => {
                    for (int i = 0; i < k; i++)
                    {
                        result *= (n - i);
                    }
                    return result;
                }
             );
            return taskD.Result;
        }

        private static async Task<int> NewtonByAvait_Down(int k)
        {
            int result = 1;
            Task<int> taskD = Task.Factory.StartNew<int>(
                () => {
                    for (int i = 0; i < k; i++)
                    {
                        result *= (i + 1);
                    }
                    return result;
                }
             );
            return taskD.Result;
        }

        private async void NewtonByAsync(object sender, RoutedEventArgs eArgs)
        {
            int n = 10, k = 2;
            var up = NewtonByAvait_Up(n, k);
            var down = NewtonByAvait_Down(k);

            Console.WriteLine(await up / await down);
        }

        private void Fibonacci(object sender, RoutedEventArgs eArgs)
        {
            int fib = 1000;
            BackgroundWorker bw = new BackgroundWorker();
            FibonacciProgress.Maximum = fib;

            bw.DoWork += (object s, DoWorkEventArgs args) =>
            {
                BackgroundWorker worker = s as BackgroundWorker;

                int max = (int)args.Argument;

                if (max == 1 || max == 2)
                {
                    worker.ReportProgress(100);
                    args.Result = 1;
                    return;
                }

                int a = 1;
                int b = 1;
                int n = 2;

                for (int i = 3; i <= max; i++)
                {
                    n = a + b;
                    worker.ReportProgress(i+1);

                    b = a;
                    a = n;
                    Thread.Sleep(20);
                }
                args.Result = n;
                return;
            };
            bw.ProgressChanged += (object s, ProgressChangedEventArgs args) =>
            {
                FibonacciProgress.Value = args.ProgressPercentage;
            };
            bw.RunWorkerCompleted += (object s, RunWorkerCompletedEventArgs args) =>
            {
                Console.WriteLine(args.Result);
            };
            bw.WorkerReportsProgress = true;
            bw.RunWorkerAsync(fib);
        }

        private void Compressation(object sender, RoutedEventArgs eArgs)
        {
            var from = new FolderBrowserDialog()
            {
                Description = " !!!! ",
                ShowNewFolderButton = false,
            };
            from.ShowDialog();
            string fromDir = from.SelectedPath;

            if (fromDir.Length > 0)
            {
                DirectoryInfo fromDirInfo = new DirectoryInfo(fromDir);
                FileInfo[] files = fromDirInfo.GetFiles();

                Parallel.ForEach(files, file => {
                    string fileCompressed = $"{fromDir}\\{Path.GetFileNameWithoutExtension(file.FullName)}.gz";

                    FileStream sourceFileStream = File.Open(file.FullName, FileMode.Open);
                    FileStream compressFileStream = File.Create(fileCompressed);
                    var compressor = new GZipStream(compressFileStream, CompressionMode.Compress);

                    sourceFileStream.CopyTo(compressor);

                    compressor.Dispose();
                    compressFileStream.Dispose();
                    sourceFileStream.Dispose();
                });
            }
        }

        private void Decompressation(object sender, RoutedEventArgs eArgs)
        {
            var from = new FolderBrowserDialog();
            from.ShowDialog();
            string fromDir = from.SelectedPath;

            if (fromDir.Length > 0)
            {
                DirectoryInfo fromDirInfo = new DirectoryInfo(fromDir);
                FileInfo[] files = fromDirInfo.GetFiles();

                Parallel.ForEach(files, file => {
                    string fileDecompressed = $"{fromDir}\\{Path.GetFileNameWithoutExtension(file.FullName)}.jpg";

                    FileStream sourceFileStream = File.Open(file.FullName, FileMode.Open);
                    FileStream decompressFileStream = File.Create(fileDecompressed);
                    var decompressor = new GZipStream(sourceFileStream, CompressionMode.Decompress);

                    decompressor.CopyTo(decompressFileStream);

                    decompressor.Dispose();
                    decompressFileStream.Dispose();
                    sourceFileStream.Dispose();
                });
            }
        }

        private void HostNamesToIP(object sender, RoutedEventArgs eArgs)
        {
            string[] hostNames = { "www.microsoft.com", "www.apple.com", "www.google.com", "www.ibm.com",
"www.netacad.com", "www.oracle.com", "www.nokia.com", "www.hp.com", "www.dell.com",
"www.samsung.com", "www.toshiba.com", "www.siemens.com", "www.amazon.com", "www.sony.com",
"www.canon.com", "www.acer.com", "www.motorola.com" };

            hostNames.AsParallel()
                .ForAll(hostName =>
                {
                    IPAddress[] IPAddresses = Dns.GetHostAddresses(hostName);
                    Console.WriteLine($"{hostName} =>");
                    foreach (var address in IPAddresses)
                    {
                        Console.WriteLine(address);
                    }
                });
        }
    }
}

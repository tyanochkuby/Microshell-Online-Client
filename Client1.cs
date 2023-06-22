using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

namespace ClientSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            int max_n = 2048, size = 0, fileLen = 0, fullSize = 0;
            var buf = new byte[max_n];
            var answer = new StringBuilder();

            var tcpEndPoint = new IPEndPoint(IPAddress.Parse("150.254.79.138"), 4785);
            var tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            tcpSocket.Connect(tcpEndPoint);
            #region 
            //tcpSocket.Send(Encoding.ASCII.GetBytes("Hello!"));
            
            //printing help
            //do
            //{
               // size = tcpSocket.Receive(buf);
                //answer.Append(Encoding.UTF8.GetString(buf, 0, size));
            //}
            //while (tcpSocket.Available > 0);
            //Console.WriteLine(answer.ToString());
            #endregion
            //printing help and path
            
            do
            {
                size = tcpSocket.Receive(buf);
                answer.Append(Encoding.UTF8.GetString(buf, 0, size));
            }
            while (tcpSocket.Available > 0);
            Console.WriteLine(answer.ToString());
            
            answer.Clear();
            //Console.WriteLine("entering while");
            //main cycle
            while(true)
            {
                //printing path
                

                //Console.WriteLine("sending command");
                var command = Console.ReadLine();
                
                


                //downloading file
                if(command.StartsWith("download"))
                {
                    if(command.Length <= 9)
                    {
                        System.Console.WriteLine("Enter file name:");
                        command = "download " + Console.ReadLine();
                    }
                    tcpSocket.Send(Encoding.ASCII.GetBytes(command));
                    System.Console.WriteLine("Receiving file length...");
                    do
                    {
                        size = tcpSocket.Receive(buf);
                        answer.Append(Encoding.UTF8.GetString(buf, 0, size));
                    }
                    while (tcpSocket.Available > 0);
                    System.Console.WriteLine(answer.ToString());
                    fileLen = Convert.ToInt32(answer.ToString());
                    fullSize = fileLen;
                    System.Console.WriteLine($"File length: {answer.ToString()}. Would you like to download it? Y/N");
                    answer.Clear();

                    command = Console.ReadLine();
                    tcpSocket.Send(Encoding.ASCII.GetBytes(command));
                    if(String.Equals(command, "y", StringComparison.OrdinalIgnoreCase))
                    {
                        //var fs = new FileStream("file.txt", FileMode.OpenOrCreate);
                        while (fullSize > 0)
                        {
                            buf = new byte[max_n];
                            size = tcpSocket.Receive(buf, SocketFlags.Partial);
                            System.Console.WriteLine("size:" + size);
                            fullSize -= size;
                            System.Console.WriteLine("fullSize:" + fullSize);
                            answer.Append(Encoding.UTF8.GetString(buf, 0, size));
                        }
                        

                        System.Console.WriteLine($"File length: {fileLen - fullSize}\nAnswer: {answer.ToString()}");

                        //saving file
                        File.WriteAllBytes(@"file.txt", Encoding.UTF8.GetBytes(answer.ToString()));
                        //printing path
                        answer.Clear();
                        do
                        {
                            size = tcpSocket.Receive(buf);
                            answer.Append(Encoding.UTF8.GetString(buf, 0, size));
                        }
                        while (tcpSocket.Available > 0);
                        System.Console.WriteLine(answer.ToString());

                    }
                    else
                    {
                        do
                        {
                            size = tcpSocket.Receive(buf);
                            answer.Append(Encoding.UTF8.GetString(buf, 0, size));
                        }
                        while (tcpSocket.Available > 0);
                        System.Console.WriteLine(answer.ToString());
                    }
                }

                //getting answer
                else
                {
                    tcpSocket.Send(Encoding.ASCII.GetBytes(command));
                    do
                    {
                        size = tcpSocket.Receive(buf);
                        answer.Append(Encoding.UTF8.GetString(buf, 0, size));
                    }
                    while (tcpSocket.Available > 0);

                    //printing answer and path
                    if (answer.ToString().StartsWith("bye"))
                        break;
                    if(answer.ToString().StartsWith("void"))
                        Console.WriteLine(answer.ToString().Substring(4));
                    else
                        Console.WriteLine(answer.ToString());
                }

        
                
                answer.Clear();

            }

            tcpSocket.Shutdown(SocketShutdown.Both);
            tcpSocket.Close();
        }
    }
}



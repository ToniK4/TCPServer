using MandatoryTechClassLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;

namespace TCPServer
{
    public class Program
    {
        private static List<Book> books = new List<Book>
        {
            new Book{ISBN13="123", Author = "Toni", PageNumber = 12, Title = "Nice"},
            new Book{ISBN13="133", Author = "Toni", PageNumber = 132, Title = "Nicer"},
        };

        static void Main(string[] args)
        {
            Console.WriteLine("Server ready:");
            TcpListener listener = new TcpListener(System.Net.IPAddress.Loopback, 4646);
            listener.Start();

            while (true)
            {
                TcpClient socket = listener.AcceptTcpClient();
                Console.WriteLine("Client connected");
                Task.Run(() => {
                    HandleClient(socket);
                });
            }


        }
        public static void HandleClient(TcpClient socket)
        {
            NetworkStream ns = socket.GetStream();
            StreamReader reader = new StreamReader(ns);
            StreamWriter writer = new StreamWriter(ns);

            Console.WriteLine("You need to write two lines as some commands have parametres.");
            Console.WriteLine();
            Console.WriteLine("First line:");
            string message1 = reader.ReadLine();
            Console.WriteLine("Second line:");
            string message2 = reader.ReadLine();

            if (message1 == "GetAll")
            {
                string jsonString = JsonSerializer.Serialize(books);
                Console.WriteLine(jsonString);

            }
            else if (message1 == "Get")
            {
                string jsonString = JsonSerializer.Serialize(books.Find(book => book.ISBN13.Contains(message2)));
                Console.WriteLine(jsonString);
            }
            else if (message1 =="Save")
            {
                Book newBook = JsonSerializer.Deserialize<Book>(message2);
                books.Add(newBook);
            }
            Console.WriteLine();
            writer.Flush();
            socket.Close();
        }
    }
}

using client.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace client
{
    class Program
    {

        static string username;
       static Socket ClientSocket;
     
        static void Main(string[] args)
        {
            int port = 13000;
            string Ipaddress = "127.0.0.1";
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(Ipaddress), port);
            ClientSocket.Connect(ep);
            Home();
           

            
        }

            public static void Home()
        {
            Console.WriteLine("*****************************************************************");
            Console.WriteLine("                         MBOS Application                        ");
            Console.WriteLine("*****************************************************************");
            Console.WriteLine("      Login(1)                           ");
            Console.WriteLine("      Register(2)                        ");
            Console.WriteLine("                                                  ");
            Console.Write("          Please enter the choice  - ");
            int n = Convert.ToInt32(Console.ReadLine());
            if (n == 1)
            {
               
                String login=  Login();
                
                ClientSocket.Send(Encoding.ASCII.GetBytes(login), login.Length, SocketFlags.None);
                addClients();
              
            }
            else if (n == 2)
            {
               String register= Register();
               
               ClientSocket.Send(Encoding.ASCII.GetBytes(register), register.Length, SocketFlags.None);
                addClients();

            }
            else
            {
                Console.WriteLine("\t\n\nInvalid Choice!\n\n");
                Home();

            }
        }


        public static void addClients() {
   
        
            while (true)
            {
                byte[] msg = new byte[50000];
                int size = ClientSocket.Receive(msg);
                String name = System.Text.Encoding.UTF8.GetString(msg, 0, size);
                getServer(name);
                
            }
        }
        public static void getServer(String name)
        {
            string[] uList = name.Split('*');
            if (uList[0] == "NotifyRegister")
            {
                Console.WriteLine(uList[1]);

                Home();
            }
           else if (uList[0] == "NotifyLogin")
            {
                Console.WriteLine(uList[1]);
                if (uList[1] == " \n\t  Username or Password error ! ") {
                    Home();
                }
                else { Menu(); }
                
            }
            else if (uList[0] == "NotifyDeposit")
            {
                Console.WriteLine(uList[1]);
                Menu();
            }
            else if (uList[0] == "NotifyWithdraw")
            {
                if (uList.Length == 2)
                {
                    Console.WriteLine(uList[1]);
                    Menu();
                }
                else if (uList.Length > 2)
                {
                    Console.WriteLine(uList[1]);
                    Console.WriteLine(uList[2]);
                    Menu();
                }
            }
            else if (uList[0] == "NotifyBalance")
            {
                Console.WriteLine(uList[1]);
                Menu();
            }
            else if (uList[0] == "NotifyTransfer")
            {
                Console.WriteLine(uList[1]);
                Menu();
            }
            else if (uList[0] == "NotifyUpdate")
            {
                Console.WriteLine(uList[1]);
                pMenu();
            }
            else if (uList[0] == "NotifyV")
            {
                Console.WriteLine("********************  PERSONAL DETAILS  ******************** ");
                Console.WriteLine(uList[1]);
                pMenu();
            }
            else if (uList[0] == "logout")
            {
                Console.WriteLine(uList[1]);
                Home();
            }
            else if (uList[0] == "bankcharges")
            {
                Console.WriteLine("\n\t********************   Notifications ******************** \n");
                if (uList.Length == 2)
                {
                    Console.WriteLine(uList[1]);
                    pMenu();
                }
                else if(uList.Length > 2)
                {
                    Console.WriteLine(uList[1]);
                    Console.WriteLine(uList[2]);
                    pMenu();
                }
                    
             
            }
            else if (uList[0] == "unRegister")
            {
                Console.WriteLine(uList[1]);
                Home();
            }

        }
                public static void Menu()
        {
            int choice = 0; do
            {
                Console.WriteLine("********************    MBOS    ******************** ");

                Console.WriteLine("        choose one option to continue              ");
                Console.WriteLine("                                                    ");
                Console.WriteLine("       1.Cash Deposit           :           ['1']     ");
                Console.WriteLine("       2.Withdrawal             :           ['2']     ");
                Console.WriteLine("       3.Check Balance          :           ['3']     ");
                Console.WriteLine("       4.Transfer Fund          :           ['4']     ");
                Console.WriteLine("       5.Profile                :           ['5']     ");
                Console.WriteLine("       6.Logout                 :           ['6']     ");
                Console.WriteLine("                                                    ");
                Console.Write("    Select your choice  - ");
               choice= Convert.ToInt32(Console.ReadLine());

                switch (choice)
                {

                    case 1:
                        Console.WriteLine("********************    CASH DEPOSIT  ******************** ");
                        Console.Write("  Enter the amount to deposit : ");
                        double amt = Convert.ToDouble(Console.ReadLine());
                        Deposit(amt,username);
                        addClients();
                        break;
                    case 2:
                        Console.WriteLine("********************  WITHDRAWAL ******************** ");
                        Console.Write(" Enter the amount to be withdrawn : ");
                       double amount = Convert.ToDouble(Console.ReadLine());
                      Withdraw(amount, username);
                        addClients();
                        break;
                    case 3:
                        Console.WriteLine("********************   CHECK BALANCE ******************** ");
                        EnquireBalance(username);
                        addClients();
                        break;
                    case 4:
                        Console.WriteLine("********************   TRANSFER MONEY  ******************** ");
                        Console.Write(" Enter the Reciever's Account Number : ");
                        string accNo = Console.ReadLine();
                        Console.Write("Enter the amount to be transfered : ");
                        double tAmount = Convert.ToDouble(Console.ReadLine());
                        Transfer(username, accNo, tAmount);
                        addClients();
                        break;
                    case 5:
                        pMenu();
                        break;
                    case 6:
                        Logout(username);
                        addClients();
                       
                        break;


                }
            }
            while (choice != 6);

            
        }

        public static String Register()
        {
          
            Console.WriteLine("********************    REGISTER    ******************** ");
            Console.WriteLine("Please enter following details : ");
            Console.Write(" First Name - ");
            String fname = Console.ReadLine();
            Console.Write(" Last Name -  ");
            String lname = Console.ReadLine();
            Console.Write(" Address -  ");
            String address = Console.ReadLine();
            String gender;
            do
            {
                Console.Write(" Gender(Female/Male) -  ");
                gender = Console.ReadLine();
            }
            while (gender != "Female" && gender != "Male");
            Console.WriteLine();
            Console.Write(" Email -  ");
            String email = Console.ReadLine();
            Console.WriteLine();
            Console.Write(" Phone -  ");
            int phone = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();
            Console.Write(" User Name -  ");
            username = Console.ReadLine();
            Console.WriteLine();
            Console.Write(" Password - ");
            String password = Console.ReadLine();
            Console.WriteLine(".Enter current date:");
            Console.Write("\t\t Year (yyyy) :");
            String year=Console.ReadLine();
            Console.WriteLine();
            Console.Write("\t\t Month (mm) :");
            String month=Console.ReadLine();
            Console.WriteLine();
            Console.Write("\t\t Day (dd) :");
            String date=Console.ReadLine();
            
            String client = "Register" + "*" + fname + "*" + lname + "*"+ address + "*" + gender + "*" + email + "*"  + username + "*" + password+"*"+year+"*"+month+"*"+date;
        
            return client;
        }

        public static String Login() {
            Console.WriteLine("********************    LOGIN    ******************** ");
            Console.Write("Enter the Username  : ");
             username = Console.ReadLine();
            Console.Write("Enter the password   : ");
            string pwd = Console.ReadLine();

            String  clients = "Login"+"*"+username+"*"+ pwd;
            return clients;

        }
        
        public static void Deposit(double amount,string uname)
        {
            String deposit = "Deposit" + "*" + amount + "*" + uname;
            ClientSocket.Send(Encoding.ASCII.GetBytes(deposit), deposit.Length, SocketFlags.None);

        }
        public static void Withdraw(double amount, string uname)
        {
            String withdraw = "Withdraw" + "*" + amount + "*" + uname;
            ClientSocket.Send(Encoding.ASCII.GetBytes(withdraw), withdraw.Length, SocketFlags.None);

        }
        public static void EnquireBalance(string uname)
        {
            String balance = "Balance" + "*" + uname;
            ClientSocket.Send(Encoding.ASCII.GetBytes(balance), balance.Length, SocketFlags.None);

        }
        public static void Transfer(string uname,string accNo,double amount)
        {
            String balance = "Transfer" + "*" + uname+"*"+accNo+"*"+amount;
            ClientSocket.Send(Encoding.ASCII.GetBytes(balance), balance.Length, SocketFlags.None);

        }
        public static void Update(string uname)
        {
            Console.WriteLine("********************   UPDATE INFORMATION  ******************** ");
            Console.WriteLine(" Enter the following information to be updated : \n");
            Console.Write(" Email: ");
            string email = Console.ReadLine();
            Console.Write(" Address: ");
            string address = Console.ReadLine();
            String update = "Update" + "*" + uname + "*" + email + "*" + address + "*" ;
            ClientSocket.Send(Encoding.ASCII.GetBytes(update), update.Length, SocketFlags.None);
            
        }
        public static void View(string uname)
        {
            String view = "View" + "*" + uname;
            ClientSocket.Send(Encoding.ASCII.GetBytes(view), view.Length, SocketFlags.None);
        }
        public static void Logout(string uname)
        {
            String logout = "Logout" + "*" + uname;
            ClientSocket.Send(Encoding.ASCII.GetBytes(logout), logout.Length, SocketFlags.None);
        }
        public  static void pMenu()
        {
            Console.WriteLine("********************      PROFILE      ******************** ");

            Console.WriteLine("   Select the Option    ");
            Console.WriteLine("1.View                  :                   ");
            Console.WriteLine("2.Update                :                   ");
            Console.WriteLine("3.Notifications         :                   ");
            Console.WriteLine("4.Un-Register           :                  ");
            Console.WriteLine("5.QUIT                  :                  ");
            Console.WriteLine("                                                       ");
            Console.Write("     Enter the choice : ");
            int opt = Convert.ToInt32(Console.ReadLine());
            if (opt == 1)
            {
               
                View(username);
                addClients();
               
            }
            else if (opt == 2)
            {
                Update(username);
                addClients();
                
            }
           else if (opt == 3)
            {
                Notifications(username);
                addClients();

            }
            else if (opt == 4)
            {
                Console.WriteLine(" Do you want to un-register the account ?");
                Console.Write(" 1.If YES press 1 ");
                Console.Write(" 2.If NO  press 2 ");
                Console.Write("YES OR NO  ");
                int prs = Convert.ToInt32(Console.ReadLine());
                if (prs == 1)
                {
                    unRegister(username);
                    addClients();
                }
                else if(prs == 2) {
                    pMenu();
                }
                else
                {
                    Console.WriteLine(" Invalid Option!");
                    pMenu();
                }
               
            }
            else if (opt == 5)
            {
                Menu();

            }
        }
        public static void Notifications(string uname)
        {
            String Notifications = "Notifications" + "*" + uname;
            ClientSocket.Send(Encoding.ASCII.GetBytes(Notifications), Notifications.Length, SocketFlags.None);
        }
        public static void unRegister(string uname)
        {
            String unRegister = "unRegister" + "*" + uname;
            ClientSocket.Send(Encoding.ASCII.GetBytes(unRegister), unRegister.Length, SocketFlags.None);
        }

    }
    
}

using Server.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Server
{
   public class Program
    {
      
        static int  count = 0;
        static int counter = 1;
        static List <Client> clients = new List<Client>();
        static List<Account> account = new List<Account>();

      
       

        static void Main(string[] args)
        {
            int port = 13000;
            string IpAddress = "127.0.0.1";
            Socket ServerListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(IpAddress), port);
            ServerListener.Bind(ep);
            ServerListener.Listen(100);
            Console.WriteLine("Server is Listening......");

            Socket ClientSocket  = default(Socket);
         
            Program p = new Program();

            while (true)
            {
                count++;
                ClientSocket = ServerListener.Accept();//accept request from the client
                Console.WriteLine(" >> " + count + " Client Conneted");
                new Thread(new ThreadStart(() => p.User(ClientSocket))).Start();

            }



        }
        
        public void User(Socket client)
        {
            while (true)
            {
                byte[] msg = new byte[50000];
                int size = client.Receive(msg);
                String name=System.Text.Encoding.UTF8.GetString(msg, 0, size);
                 
                Thread t=new Thread(() => serverMsg(name,client));
                t.Start();
              
             
            }
           
        }


        public void serverMsg(String name, Socket ClientSocket)
        {
            
                    string[] uList = name.Split('*');

                    if (uList[0] == "Register")
                    {
                              string reg = null;
                              Boolean flag = true;
                               Client c = new Client(uList[1], uList[2], uList[3], uList[4], uList[5], uList[6], uList[7], uList[8], uList[9],uList[10]);
                              if (clients != null)
                              {
                                   foreach (Client c1 in clients)
                                   {
                                       if (c1.uName != c.uName)
                                       {
                         
                                       }
                                       else
                                       {
                                            flag = false;
                           
                                       }

                                   }

                              }
                              else
                              {
                                     clients.Add(c);
                                     Account a = new Account("A00" + counter, 100.00, 100.00, 0.08, c);
                                     account.Add(a);

                                     Console.WriteLine(" >> "+uList[1] + " you are successfully registered ");
                                     reg = "NotifyRegister" + "*" + "\n\t o Registered Successfully " + uList[1] + " with Account Number " + account[counter - 1].accountNo;
                                     counter++;
                              }

                              if (flag == true)
                              {
                                    clients.Add(c);
                                    Account a = new Account("A00" + counter, 100.00, 100.00, 0.08, c);
                                    account.Add(a);

                                    Console.WriteLine(" >> " + uList[1] + " you are successfully registered  ");
                                    reg = "NotifyRegister" + "*" + "\n\t o Registered Successfully " + uList[1] + " with Account Number " + account[counter - 1].accountNo;
                                    counter++;
                   
                              }
                              else
                              {
                                    reg = "NotifyRegister" + "*" + "Registeration unsuccessful , your username must be invalid !";
                              }
                 
                          ClientSocket.Send(Encoding.ASCII.GetBytes(reg), reg.Length, SocketFlags.None);
                    
                    }

                    else if (uList[0] == "Login")
                    {

                        String username = uList[1];
                        String password = uList[2];
                        String msg;
                        if (validateUser(username, password))
                        {
                            msg = "NotifyLogin" + "*" + "\n\t  " +username +" successfully logged in ";
                           Console.WriteLine(" >> " + username + " logged in to the system ");
                        }
                        else
                        {
                            msg = "NotifyLogin" + "*" + " \n\t  Username or Password error ! ";
                            Console.WriteLine(" >> " + username + "login failed ");
                        }

                        ClientSocket.Send(Encoding.ASCII.GetBytes(msg), msg.Length, SocketFlags.None);



                    }

                    else if (uList[0] == "Deposit")
                    {

                        double amount = Convert.ToDouble(uList[1]);
                        String uname = uList[2];
                        string depositmsg = null;
                        foreach (Account acc in account)
                        {
                            if (acc.client.uName == uname)
                            {
                                acc.balance += amount;

                                depositmsg = "NotifyDeposit" + "*" + "\n\t Successfully Deposited " + amount + " to " + acc.accountNo;
                                Console.WriteLine(" >> " + uname +" has depoisted Rs."+amount+" to "+acc.accountNo);
                            }

                        }
               
                        ClientSocket.Send(Encoding.ASCII.GetBytes(depositmsg), depositmsg.Length, SocketFlags.None);

                    }
                    else if (uList[0] == "Withdraw")
                    {

                        double amount = Convert.ToDouble(uList[1]);
                        String uname = uList[2];
                        string withdrawmsg = null;
                        double dailywithdraw;
                        foreach (Account acc in account)
                        {
                               DateTime date = Convert.ToDateTime(acc.client.year + "-" + acc.client.month + "-" + acc.client.date);
                               var regday = float.Parse(date.ToString("yyyy.MMdd"));
                               var now = float.Parse(DateTime.Now.ToString("yyyy.MMdd"));
                          if ((now - regday) <= 1)
                            {
                                dailywithdraw = 20000.00;
                            }
                          else if((now - regday) > 1&& (now - regday) <= 5)
                           {
                                  dailywithdraw = 50000.00;
                           }
                           else
                            {
                                   dailywithdraw = 100000.00;
                            }
                    if (acc.client.uName == uname)
                            {
                                if (acc.balance < amount)
                                {
                                    withdrawmsg = "NotifyWithdraw" + "*" + " Insufficient Balance";
                                }
                                else if (acc.balance > amount)

                                {
                                  if (dailywithdraw >= amount)
                                  {

                                         acc.balance -= amount;
                                         withdrawmsg = "NotifyWithdraw" + "*" + "  Successfully Withdrawed " + amount + " from " + acc.accountNo + " your account balance is : " + acc.balance;
                                         Console.WriteLine(" >> " + uname + " has withdrawed Rs." + amount + " from " + acc.accountNo);
                                  }
                                  else if (dailywithdraw < amount)
                                  {
                                         withdrawmsg = "NotifyWithdraw" + "*" + " Your amount is greater than daily withdrwal amount ,\n\t # your daily withdrawal amount is : "+dailywithdraw;
                                  }
                                    
                                }

                            }

                        }

                        ClientSocket.Send(Encoding.ASCII.GetBytes(withdrawmsg), withdrawmsg.Length, SocketFlags.None);

                    }
                    else if (uList[0] == "Balance")
                    {

                        String uname = uList[1];

                        foreach (Account acc in account)
                        {
                            if (acc.client.uName == uname)
                            {
                                string balancemsg = "NotifyBalance" + "*"+"\n\t o " + acc.accountNo + " your available balance is " + string.Format("{0:0.00}",acc.balance);
                                ClientSocket.Send(Encoding.ASCII.GetBytes(balancemsg), balancemsg.Length, SocketFlags.None);
                            }
                        }


                    }
                    else if (uList[0] == "Transfer")
                    {

                        Account a1 = null;
                        Account a2 = null;
                        String uname = uList[1];
                        String accNo = uList[2];
                        double amount = Convert.ToDouble(uList[3]);
                        string tmsg = null;
                        foreach (Account acc in account)
                        {
                            if (acc.client.uName == uname)
                            {
                                a1 = acc;
                            }
                            if (acc.accountNo == accNo)
                            {
                                a2 = acc;
                            }
                        }
                        if (a2 == null)
                        {
                            tmsg = "NotifyTransfer" + "*" + " Account is unavailable ";

                        }
                        else if (a2 != null)
                        {
                            if (a1.balance > amount)
                            {
                                a1.balance -= amount;
                                a2.balance += amount;
                                tmsg = "NotifyTransfer" + "*" + " Transaction successful , " + " from " + a1.accountNo + " to " + a2.accountNo + " you've transfered " + amount;
                        Console.WriteLine(" >> " + uname +" has transfered Rs."+amount+" from "+a1.accountNo+" to "+a2.accountNo);
                            }
                            if (a1.balance < amount)
                            {

                                tmsg = "NotifyTransfer" + "*" + " Insufficient Balance";

                            }
                        }
                        ClientSocket.Send(Encoding.ASCII.GetBytes(tmsg), tmsg.Length, SocketFlags.None);


                    }
                    else if (uList[0] == "Update")
                    {

                        String uname = uList[1];
                        String email = uList[2];
                        String address = uList[3];
                        int age = Convert.ToInt32(uList[4]);
                        foreach (Account acc in account)
                        {
                            if (acc.client.uName == uname)
                            {
                                acc.client.email = email;
                                acc.client.address = address;
                                acc.client.age = age;

                            }
                        }
                        foreach (Client c in clients)
                        {
                            if (c.uName == uname)
                            {
                                c.email = email;
                                c.address = address;
                                c.age = age;

                            }
                        }
                        string umsg = "NotifyUpdate" + ","+"\n\t o " + uname + " your account details has been updated successfully";
                        Console.WriteLine(" >> " + uname +" has updated profile details .");
                        ClientSocket.Send(Encoding.ASCII.GetBytes(umsg), umsg.Length, SocketFlags.None);


                    }
                    else if (uList[0] == "View")
                    {


                        String uname = uList[1];
                        foreach (Client c in clients)
                        {
                            if (c.uName == uname)
                            {
                                string vmsg = "NotifyV" + "*" + "\n\t------- Your Profile -------" + "\n\ti.First Name : " + c.fName + "\n\tii.Email : " + c.email + "\n\tiii.Address : " + c.address + "\n\tiv.Phone : " + c.phone+"\n\tiv.year : "+c.year;
                                ClientSocket.Send(Encoding.ASCII.GetBytes(vmsg), vmsg.Length, SocketFlags.None);
                            }
                        }


                    }
                    else if (uList[0] == "Logout")
                    {
                        String uname = uList[1];
                        foreach (Client c in clients)
                        {
                            if (c.uName == uname)
                            {
                                String logoutmsg = "logout" + "*" + "\n\t o " + uname + "  you've successfully logged out !!!!";
                                Console.WriteLine(" >> " + uname +" has been logged out from the system");
                                ClientSocket.Send(Encoding.ASCII.GetBytes(logoutmsg), logoutmsg.Length, SocketFlags.None);
                            }
                        }


                    }
            else if (uList[0] == "Notifications")

            {

                String uname = uList[1];
                foreach(Account a in account)
                {
                    if (a.client.uName == uname)
                    {
                        DateTime date = Convert.ToDateTime(a.client.year + "-" + a.client.month + "-" + a.client.date);
                        var regday = float.Parse(date.ToString("yyyy.MMdd"));
                        var now = float.Parse(DateTime.Now.ToString("yyyy.MMdd"));
                        if (now > regday)

                        {
                            if((now - regday) >= 1) {

                                double interestYear = ((now - regday) * a.interestRate * a.balance);
                                double interestAmount = (a.interestRate * a.balance);
                                double creditbalance = a.balance += interestAmount;
                                double debitbalance = a.balance -= a.bankCharges;
                                String bankcharges = "bankcharges" + "*" + "\n\n\to " + uname + " we've credited " + string.Format("{0:0.00}", interestAmount) + " to your account as annual interest your avalable balance is " + string.Format("{0:0.00}", creditbalance) + " from " + regday + " to " + now + " we've credited " + string.Format("{0:0.00}", interestYear)
                                    + "\n\to Annual bank charges  has been debited from your account " + a.accountNo + " your avaialble balance is " + string.Format("{0:0.00}", debitbalance);
                                Console.WriteLine(" >> " + "Annual charges have been debited and credited from " + uname);
                                ClientSocket.Send(Encoding.ASCII.GetBytes(bankcharges), bankcharges.Length, SocketFlags.None);
                                
                            }
                            else
                            {
                                String error = "bankcharges" + "*" + "\n\n\t There is no notification !!!!";
                                ClientSocket.Send(Encoding.ASCII.GetBytes(error), error.Length, SocketFlags.None);

                            }  

                        }
                        else
                        {
                            String error = "bankcharges" + "*" + "\n\n\t There is no notification !!!!";
                            ClientSocket.Send(Encoding.ASCII.GetBytes(error), error.Length, SocketFlags.None);
                        }
                    }
                }
            }
              else if(uList[0]== "unRegister")
            {
                String uname = uList[1];
                for (int i = 0; i < clients.Count; i++)
                {
                    if (clients[i].uName == uname )
                    {
                        clients.Remove(clients[i]);
                    }
        
                }
                String remove = "unRegister" + "*" + "\n\n\t Successfully removed from the system";
                Console.WriteLine(" >> " + uname +" has been removed from the system ");
                ClientSocket.Send(Encoding.ASCII.GetBytes(remove), remove.Length, SocketFlags.None);

            } 


        }

        public bool validateUser(String username, String password)
        {
            bool flag = false;

            {
                
                {
                  
                    for (int i = 0; i < clients.Count; i++)
                    {
                        if ((clients[i].uName == username) && (clients[i].password == password))
                        {
                            flag = true;
                        }
                    }

                   
                }

                }
            return flag;
        }

       
     
     


    }
}
    


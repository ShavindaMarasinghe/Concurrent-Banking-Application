using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Model
{
   public class Account
    {
        public Client client;
       
        public string accountNo;
        public double balance;
        public double bankCharges;
        public double interestRate;

        public Account()
        {

        }

        public Account( double balance, double bankCharges, double interestRate)
        {
         
            this.balance = balance;
            this.bankCharges = bankCharges;
            this.interestRate = interestRate;
      

        }
        public Account(string accountNo,double balance, double bankCharges, double interestRate,Client client)
        {
            this.client = client;
            this.balance = balance;
            this.bankCharges = bankCharges;
            this.interestRate = interestRate;
            this.accountNo = accountNo;

        }
        public double Balance { get; set; }
        public double BankCharges { get; set; }
        public double InterestRate { get; set; }
        


    }
}

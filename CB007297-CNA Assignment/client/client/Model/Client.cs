﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client.Model
{
    [Serializable]
    public class Client
    {
        public String type;
        public String fName;
        public String lName;
        public String nic;
        public String address;
        public String gender;
        public String email;
        public int phone;
        public int age;
        public String uName;
        public String password;

        public Client(String type, String fName, String lName, String nic, String address, String gender, String email,int phone,int age, String uName, String password)
        {
            this.type = type;
            this.fName = fName;
            this.lName = lName;
            this.nic = nic;
            this.address = address;
            this.gender= gender;
            this.email = email;
            this.phone = phone;
            this.age = age;
            this.uName = uName;
            this.password = password;
        }

        public Client(String type,String uName, String password) {

            this.type =type;
            this.uName = uName;
            this.password = password;
        }
        public Client()
        {
           
           
        }

        public string AAA { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string NIC { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }

        public string Phone { get; set; }

        public string Age { get; set; }
        public string UName { get; set; }
        public string Password { get; set; }

        
     
      

    }
}

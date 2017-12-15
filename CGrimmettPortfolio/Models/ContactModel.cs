using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CGrimmettPortfolio.Models
{
    public class ContactModel
    {
        public ContactModel(string firstname, string email, string lastname, string message)
        {
            FirstName = firstname;
            LastName = lastname;
            Email = email;
            Message = message;
        }


        public int ID { get; set; }
        public string Sender { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Message { get; set; }
    }
}
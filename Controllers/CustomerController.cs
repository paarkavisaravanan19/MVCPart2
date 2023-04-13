using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.View;
using MVCPracticePart2.Models;
using System.Data;

namespace MVCPracticePart2.Controllers
{
    public class CustomerController : Controller
    {
        IConfiguration configuration;
        SqlConnection CONN;
        public CustomerController(IConfiguration configuration)
        {
            this.configuration = configuration;
            CONN = new SqlConnection(configuration.GetConnectionString("PracticeDB"));

        }

        public List<CustomerModel> GetCustomer()
        { 
            List<CustomerModel> Customers = new List<CustomerModel>();
            CONN.Open();
            SqlCommand cmd = new SqlCommand("FetchAllDetails", CONN);
            cmd.CommandType = CommandType.StoredProcedure;
            
            SqlDataReader reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                CustomerModel customer_ = new();
                customer_.CustomerID = (int)reader["CustomerID"];
                customer_.CustomerName = (string)reader["CustomerName"];
                customer_.ContactNo = (string)reader["ContactNo"];
                customer_.Location = (string)reader["Location"];
                customer_.emailid = (string)reader["emailId"];
                Customers.Add(customer_);
            }
            reader.Close();
            CONN.Close();
            return Customers;

        }
       

        // GET: CustomerController
        public ActionResult Index()
        {
            return View(GetCustomer());
        }

        // GET: CustomerController/Details/5
        public ActionResult Details(int id)
        {
            return View(GetCustomer(id));
        }

        // GET: CustomerController/Create
        public ActionResult Create()
        {
            return View();
        }

        void InsertCustomer(CustomerModel Customer)
        {
            CONN.Open();
            SqlCommand cmd = new SqlCommand("Add_Customer", CONN);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@CustomerId", Customer.CustomerID);
            cmd.Parameters.AddWithValue("@CustomerName", Customer.CustomerName);
            cmd.Parameters.AddWithValue("@ContactNo", Customer.ContactNo);
            cmd.Parameters.AddWithValue("@Location", Customer.Location);
            cmd.Parameters.AddWithValue("@emailId", Customer.emailid);
            cmd.ExecuteNonQuery();
            CONN.Close();
        }
        
        

        // POST: CustomerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CustomerModel Customer)
        {
            try
            {
                InsertCustomer(Customer);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }



        CustomerModel GetCustomer(int id)
        {
            CONN.Open();
            SqlCommand cmd = new SqlCommand("GET_CUSTOMER", CONN);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@CustomerID", id);

            SqlDataReader reader = cmd.ExecuteReader();

            CustomerModel customer = new();

            while (reader.Read())
            {
                customer.CustomerID = (int)reader["CustomerID"];
                customer.CustomerName = (string)reader["CustomerName"];
                customer.ContactNo = (string)reader["ContactNo"];
                customer.Location = (string)reader["Location"];
                customer.emailid = (string)reader["emailId"];
                
            }
            return customer;
        }

        // GET: CustomerController/Edit/5
        public ActionResult Edit(int id)
        {
            return View(GetCustomer(id));
        }

        void updateCustomer(int id, CustomerModel Customer)
        {
            CONN.Open();
            SqlCommand cmd = new SqlCommand("Edit_Customer", CONN);
            cmd.CommandType= CommandType.StoredProcedure;
            Console.WriteLine(Customer.CustomerName);
            cmd.Parameters.AddWithValue("@CustomerId", Customer.CustomerID);
            cmd.Parameters.AddWithValue("@CustomerName", Customer.CustomerName);
            cmd.Parameters.AddWithValue("@ContactNo", Customer.ContactNo);
            cmd.Parameters.AddWithValue("@Location", Customer.Location);
            cmd.Parameters.AddWithValue("@emailId", Customer.emailid);
            cmd.ExecuteNonQuery();
            CONN.Close();
        }
        // POST: CustomerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id,CustomerModel Customer)
        {
            try
            {
                updateCustomer(id, Customer);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        /*void DeleteCustomer(int id, CustomerModel Customer)
        {
            CONN.Open();
            SqlCommand cmd = new SqlCommand("Delete_Customer", CONN);
            cmd.CommandType = CommandType.StoredProcedure;
            Console.WriteLine(Customer.CustomerName);
            cmd.Parameters.AddWithValue("@CustomerId", Customer.CustomerID);

            cmd.ExecuteNonQuery();
            CONN.Close();
        }*/



        // GET: CustomerController/Delete/5
        public ActionResult Delete(int id)
        {
            return View(GetCustomer(id));
        }

        // POST: CustomerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, CustomerModel Customer)
        {
            try
            {
                CONN.Open();
                SqlCommand cmd = new SqlCommand("Delete FROM CustomerDetails where CustomerID= @CustomerId",CONN);

                cmd.Parameters.AddWithValue("@CustomerId", id);
                cmd.ExecuteNonQuery();

                CONN.Close();
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return View();
            }
        }
    }
}

using Newtonsoft.Json;
using StudentsWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace StudentsWebApp.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        
        //Hosted web API REST Service base url  
        string Baseurl = "http://localhost:51693/";
        public async Task<ActionResult> Index()
        {
            List<Student> EmpInfo = new List<Student>();

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/Students");

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    EmpInfo = JsonConvert.DeserializeObject<List<Student>>(EmpResponse);

                }
                //returning the employee list to view  
                return View(EmpInfo);
            }            
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Student student)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);

                //HTTP POST
                var postTask = client.PostAsJsonAsync<Student>("api/Students", student);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return View(student);
        }

        public ActionResult Details()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Details(string guid)
        {
            Student student = new Student();
            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/Students?guid=" + guid);

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var EmpResponse = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    student = JsonConvert.DeserializeObject<Student>(EmpResponse);

                }
                //returning the employee list to view  
                return View(student);
            }            
        }

        public ActionResult Delete(string guid)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);

                //HTTP DELETE
                var deleteTask = client.DeleteAsync("api/Students?guid=" + guid);
                deleteTask.Wait();

                var result = deleteTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<ActionResult> Edit(string guid, Student student)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                HttpResponseMessage responseMessage = await client.PutAsJsonAsync<Student>("api/Students?guid=", student);
                if (responseMessage.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Error");
        }

        public async Task<ActionResult> Edit(string guid)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                HttpResponseMessage responseMessage = await client.GetAsync("api/Students?guid=" + guid);
                if (responseMessage.IsSuccessStatusCode)
                {
                    var responseData = responseMessage.Content.ReadAsStringAsync().Result;

                    var Student = JsonConvert.DeserializeObject<Student>(responseData);

                    return View(Student);
                }
            }
            return View("Error");
        }
    }
}
using Newtonsoft.Json;
using StudentsWebApp.Models;
using StudentsWebApp.Properties;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace StudentsWebApp.Controllers
{
    public class HomeController : Controller
    {

        //Hosted web API REST Service base url  
        string Baseurl = System.Configuration.ConfigurationManager.AppSettings[ConfigStrings.BaseUrl];

        // GET: Home
        public async Task<ActionResult> Index()
        {
            
            List<Student> students = new List<Student>();

            using (var client = new HttpClient())
            {
                //Passing service base url  
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource get all students using HttpClient  
                HttpResponseMessage Res = await client.GetAsync("api/Students");

                //Checking the response is successful or not which is sent using HttpClient  
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var responseData = Res.Content.ReadAsStringAsync().Result;

                    //Deserializing the response recieved from web api and storing into the student list  
                    students = JsonConvert.DeserializeObject<List<Student>>(responseData);

                }
                //returning the student list to view  
                return View(students);
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
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();                
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));                
                HttpResponseMessage Res = await client.GetAsync("api/Students?guid=" + guid);
                
                if (Res.IsSuccessStatusCode)
                {                    
                    var dataResponse = Res.Content.ReadAsStringAsync().Result;                    
                    student = JsonConvert.DeserializeObject<Student>(dataResponse);

                }                
                return View(student);
            }            
        }

        [HttpDelete]
        public ActionResult Delete(string guid)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
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
        public async Task<ActionResult> Edit(Student student)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                HttpResponseMessage responseMessage = await client.PutAsJsonAsync<Student>("api/Students/"+student.GUID, student);
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
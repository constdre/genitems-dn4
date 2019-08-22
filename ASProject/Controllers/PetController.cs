using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ASProject.Models;
using ASProject.Repository;
using ASProject.DAL;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System.Web.Http;
using System.Diagnostics;
using ASProject.Models.ViewModels;
using ASProject.Models.DTO;
using Newtonsoft.Json;
using System.Data.Entity;


namespace ASProject.Controllers
{
    public class PetController : Controller
    {

        string userId;
        ApplicationUser activeUser;

        public ActionResult ManagePet(string status)
        {
            ViewBag.Status = status;
            return View();
        }


        /// <summary>
        /// Inserts a pet entity to the db from and success-handled by JavaScript AJAX post request.
        /// </summary>
        /// <param name="pet">The pet entity object from the POST request body</param>
        public void AddPet([FromBody]Pet pet)
        {
            //Not for form submitted requests.
            //[FromBody] to tell the request data is in the body of a JS AJAX post method.


            userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            activeUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(userId);
            string status = "";
            RedirectToRouteResult returnAction = null;
            
            using (SystemContext dbContext = new SystemContext())
            {
                Repository<Pet> repo = new Repository<Pet>(dbContext);
                pet.UserId = activeUser.Id;
                status = repo.Add(pet);

                Debug.WriteLine("STATUS: " + status);
                //Display the pets:
                foreach (Pet p in repo.GetAll().ToList())
                {
                    Debug.WriteLine("PET NAME: " + p.Name);
                }

            }
            Debug.WriteLine("Context disposed");
            if (status != "success")
            {
                Debug.WriteLine("Unsuccessful.");
                returnAction = RedirectToAction("ManagePet", new { message = status });
            }

            returnAction = RedirectToAction("Pets", "Home", new { message = status });
            //return returnAction;
            //return RedirectToAction("Pets", "Home", new { message = status });
        }


        public ActionResult AddPetThroughViewModel(ManagePetDynamicVM model)
        {

            if (ModelState.IsValid)
            {

                userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                activeUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(userId);
                Pet pet = new Pet { UserId = activeUser.Id, Kind = model.Kind, Breed = model.Breed, Name = model.Name, Constraints = model.Constraints };


                /*forge the petImages list with the other information PetImage requires*/
                List<PetImage> petImages = GetImagesFromRequest(Request.Files);
                pet.Images = petImages; //now EF assigns the id and foreign key then inserts them to the PetImages table IMPLICITLY.

                using (SystemContext dbContext = new SystemContext())
                {
                    Repository<Pet> repo = new Repository<Pet>(dbContext);
                    string status = repo.Add(pet);
                    if (status != "success")
                    {
                        Debug.WriteLine("Unsuccessful. STACK TRACE: \n" + status);
                        string internalErrorMessage ="An error occured when processing your request.";
                        return RedirectToAction("ManagePet", new { status = internalErrorMessage });
                    }
                }
                return RedirectToAction("Pets", "Home", new { description = model.Name + " was successfully added!" });
            }
            else
            {
                Debug.WriteLine("MODELSTATE IS INVALID");
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                .Select(x => new { x.Key, x.Value.Errors });//Select() forms a new collection 
                Debug.WriteLine(errors);
            }
            ModelState.AddModelError("myKey", "ModelState is invalid. Find the reason how and why."); 
            return RedirectToAction("ManagePet", new { status = "ERROR" });
        }

        public ActionResult DeletePet([FromBody] int petId)
        {
            Debug.WriteLine("PetId: " + petId);
            var statusDict = new Dictionary<string,object>();
            using (SystemContext dbContext = new SystemContext())
            {
                Repository<Pet> repo = new Repository<Pet>(dbContext);
                statusDict = repo.Delete(petId);
                
            }
            Debug.WriteLine(statusDict["status"]);
            Pet removedPet = (Pet)statusDict["removedObj"];
            string status = "Item deleted : " + removedPet.Name; 

            
            return RedirectToAction("Pets","Home", new { description = status });
            

        }
        public ActionResult GetDisplayImage(int petId)
        {
            byte[] fileBytes = null;
            string fileType = null;
            using(SystemContext dbContext = new SystemContext())
            {
                var image = dbContext.GetEntitySet<PetImage>().FirstOrDefault((img) => img.PetId == petId && img.IsDisplayPic == true);
                Debug.WriteLine("display image retrieved! filename: " + image.Name);
                if (image != null)
                {
                    fileBytes = image.Image;//the byte[]
                    fileType = image.Type;
                }
                else
                {
                    Debug.WriteLine("Image is null.");
                }
            }
            
            return File(fileBytes, fileType);
        }

        public ActionResult GetAllPets()
        {
            Repository<Pet> repo = new Repository<Pet>(new SystemContext());
            List<Pet> pets = repo.GetAll().ToList();


            var list = JsonConvert.SerializeObject(pets,
            Formatting.None,
            new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPetsIndex(string userId)
        {
            userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            activeUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(userId);

            DbSet<Pet> petSet = new SystemContext().GetEntitySet<Pet>();
            List<Pet> petsList = (activeUser == null) ? petSet.ToList() : petSet.AsQueryable().Where(p => p.UserId != activeUser.Id).ToList();


            var jsonString = JsonConvert.SerializeObject(petsList, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            var jsonResponse = Json(jsonString, JsonRequestBehavior.AllowGet);
            jsonResponse.MaxJsonLength = int.MaxValue;

            return jsonResponse;
        }

        
        public ActionResult GetPetsOfUser()
        {
            //DECOUPLE PET IMAGES WITH PET SO DURING RETRIEVAL OF PETS, THE IMAGES ARE NOT INCLUDED = OPTIMIZED PERFORMANCE
            userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            activeUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(userId);


            DbSet<Pet> dbSet = new SystemContext().GetEntitySet<Pet>();
            List<Pet> pets_user = dbSet.AsQueryable().Where(p => p.UserId == activeUser.Id).ToList();
            Debug.WriteLine("Pets of user numbers: " + pets_user.Count());


            var jsonList= JsonConvert.SerializeObject(pets_user, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore, 
            });


            var jsonResponse = Json(jsonList, JsonRequestBehavior.AllowGet);
            jsonResponse.MaxJsonLength = int.MaxValue;

            return jsonResponse;

        }

        public ActionResult GetSearchResults(string searchParam)
        {
            Debug.WriteLine($"searchParam:  {searchParam}");
            userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            activeUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(userId);

            IQueryable<Pet> dbSet = new SystemContext().GetEntitySet<Pet>().AsQueryable();

            List<Pet> petsList = (activeUser == null) ? dbSet.Where(p => p.Name.ToLower().Contains(searchParam.ToLower())).ToList() : dbSet.Where(p => p.Name == searchParam && p.UserId != activeUser.Id).ToList();
            
            var jsonString = JsonConvert.SerializeObject(petsList, Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            Response.ContentType = "application/json";

            var jsonResponse = Json(jsonString, JsonRequestBehavior.AllowGet);
            jsonResponse.MaxJsonLength = int.MaxValue;

            return jsonResponse;
        }

        private List<PetImage> GetImagesFromRequest(HttpFileCollectionBase files) 
        {
            List<PetImage> petImages = new List<PetImage>();
            for(int i=0; i<files.Count; i++)
            {

                //create the byte[] which is what EF uses for files.
                byte[] fileBytes = new byte[files[i].ContentLength];
                files[i].InputStream.Read(fileBytes, 0, fileBytes.Length);


                PetImage pi = new PetImage
                {
                    Name = files[i].FileName,
                    Size = files[i].ContentLength,
                    Type = files[i].ContentType,
                    Image = fileBytes,
                    CreatedOn = DateTime.Now,
                    IsDisplayPic = true
                };
                petImages.Add(pi); 
            }
            Debug.WriteLine("The petImages list: ");
            Debug.WriteLine(string.Join(",", petImages));
            return petImages;
        }
    }
}
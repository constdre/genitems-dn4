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
using Newtonsoft.Json;
using System.Data.Entity;

namespace ASProject.Controllers
{
    public class PetController : Controller
    {

        JsonSerializerSettings referenceLoopIgnore = new JsonSerializerSettings()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public ActionResult Pets(string description)
        {
            ViewBag.Message = "Cute creatures";
            ViewBag.Description = description;
            return View();
        }
        public ActionResult ManagePet(string status)
        {
            ViewBag.Status = status;
            return View();
        }

        public void AddPetAjax([FromBody] Pet pet)
        {
            //[FromBody] to tell the request data is in the body of a JS AJAX post method.
            string status = "";
            string userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ApplicationUser activeUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(userId);

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

            if (status != Utils.Constants.SUCCESS_MESSAGE)
            {
                Debug.WriteLine("Unsuccessful.");
            }

        }

        public ActionResult AddPetThroughViewModel(ManagePetDynamicVM model)
        {

            if (ModelState.IsValid)
            {

                string userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
                ApplicationUser activeUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(userId);

                Pet pet = new Pet
                {
                    UserId = activeUser.Id,
                    Kind = model.Kind,
                    Breed = model.Breed,
                    Name = model.Name,
                    Constraints = model.Constraints
                };


                //build the petImages list with the other information PetImage requires
                List<PetImage> petImages = GetImagesFromRequest(Request.Files);
                pet.Images = petImages;

                //Add to DB
                using (SystemContext dbContext = new SystemContext())
                {
                    Repository<Pet> repo = new Repository<Pet>(dbContext);
                    string status = repo.Add(pet);
                    if (status != Utils.Constants.SUCCESS_MESSAGE)
                    {
                        Debug.WriteLine("Failed Add Pet. STACK TRACE: \n" + status);
                        string internalErrorMessage = "An error occured while processing your request.";
                        return RedirectToAction("ManagePet", new { status = internalErrorMessage });
                    }
                }

                var description = model.Name + " was successfully added!";
                return RedirectToAction("Pets", "Pet", new { description });
            }
            else
            {
                Debug.WriteLine("MODELSTATE IS INVALID");
                var errors = ModelState.Where(x => x.Value.Errors.Any())
                .Select(x => new { x.Key, x.Value.Errors });
                Debug.WriteLine(errors);
            }

            ModelState.AddModelError("myKey", "ModelState is invalid. Find the reason how and why.");
            return RedirectToAction("ManagePet", new { status = Utils.Constants.FAILED_MESSAGE });
        }

        public ActionResult DeletePet([FromBody] int petId)
        {
            var statusDict = new Dictionary<string, object>();
            using (SystemContext dbContext = new SystemContext())
            {
                Repository<Pet> repo = new Repository<Pet>(dbContext);
                statusDict = repo.Delete(petId);

            }

            Debug.WriteLine(statusDict["status"]);
            Pet removedPet = (Pet)statusDict["removedObj"];//to do something with the deleted object

            string status = "Item deleted : " + removedPet.Name;
            return RedirectToAction("Pets", "Pet", new { description = status });

        }
        public ActionResult GetDisplayImage(int petId)
        {
            byte[] fileBytes = null;
            string fileType = null;
            using (SystemContext context = new SystemContext())
            {
                var image = context.GetEntitySet<PetImage>().FirstOrDefault((img) => img.PetId == petId && img.IsDisplayPic == true);
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
            List<Pet> pets;

            using (var context = new SystemContext())
            {
                Repository<Pet> repo = new Repository<Pet>(new SystemContext());
                pets = repo.GetAll().ToList();
            }

            var list = JsonConvert.SerializeObject(pets, Formatting.None, referenceLoopIgnore);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPetsHome(string userId)
        {
            //Gets items data, avoids user's items
            userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ApplicationUser activeUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(userId);
            List<Pet> petsList;

            using (var context = new SystemContext())
            {
                DbSet<Pet> petSet = new SystemContext().GetEntitySet<Pet>();
                petsList = (activeUser == null) ?
                    petSet.ToList() :
                    petSet.Where(p => p.UserId != activeUser.Id).ToList();
            }

            var jsonString = JsonConvert.SerializeObject(
                petsList,
                Formatting.None,
                referenceLoopIgnore
                );

            var jsonResponse = Json(jsonString, JsonRequestBehavior.AllowGet);
            jsonResponse.MaxJsonLength = int.MaxValue;

            return jsonResponse;
        }


        public ActionResult GetPetsOfUser()
        {

            string userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ApplicationUser activeUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(userId);

            DbSet<Pet> dbSet = new SystemContext().GetEntitySet<Pet>();
            List<Pet> pets_user = dbSet.Where(p => p.UserId == userId).ToList();

            var jsonList = JsonConvert.SerializeObject(pets_user, Formatting.None, referenceLoopIgnore);


            var jsonResponse = Json(jsonList, JsonRequestBehavior.AllowGet);
            jsonResponse.MaxJsonLength = int.MaxValue;

            return jsonResponse;
        }

        public ActionResult GetSearchResults(string searchParam)
        {
            Debug.WriteLine($"searchParam:  {searchParam}");
            string userId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            ApplicationUser activeUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(userId);

            IQueryable<Pet> dbSet = new SystemContext().GetEntitySet<Pet>().AsQueryable();

            List<Pet> petsList = (activeUser == null) ? dbSet.Where(p => p.Name.ToLower().Contains(searchParam.ToLower())).ToList() : dbSet.Where(p => p.Name.ToLower().Contains(searchParam.ToLower()) && p.UserId != activeUser.Id).ToList();

            var jsonString = JsonConvert.SerializeObject(petsList, Formatting.None, referenceLoopIgnore);

            Response.ContentType = "application/json";

            var jsonResponse = Json(jsonString, JsonRequestBehavior.AllowGet);
            jsonResponse.MaxJsonLength = int.MaxValue;

            return jsonResponse;
        }

        private List<PetImage> GetImagesFromRequest(HttpFileCollectionBase files)
        {
            List<PetImage> petImages = new List<PetImage>();

            for (int i = 0; i < files.Count; i++)
            {
                //create the byte[] which is what EF uses for files
                byte[] fileBytes = new byte[files[i].ContentLength];

                //copy byte array content of file 
                files[i].InputStream.Read(fileBytes, 0, fileBytes.Length);

                PetImage pi = new PetImage
                {
                    Name = files[i].FileName,
                    Size = files[i].ContentLength,
                    Type = files[i].ContentType,
                    Image = fileBytes,
                    CreatedOn = DateTime.Now,
                    IsDisplayPic = true,

                    //Implicitly assigned by EF: Id (PK) and PetId (FK). Lost of things happening implicitly with EF

                };

                petImages.Add(pi);
            }

            Debug.WriteLine("The petImages list: ");
            Debug.WriteLine(string.Join(",", petImages));

            return petImages;
        }
    }
}
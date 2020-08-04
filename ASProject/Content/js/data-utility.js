

/*Module Design Pattern */
var displayPet = (() => {
    
    return {
        execute: execute,
        displayPets: displayPets
    };

    async function execute(getPetsUrl, loader) {

        //gets pets and displays them

        loader.hidden = false;
        var pets = [];
       
        setTimeout(async function () {

            pets = await getResource(getPetsUrl);

            if (pets.length <= 0) {
                console.log("inside if statement");
                document.getElementById("div_empty").hidden = false;
            }

            displayPets(pets);
            loader.hidden = true;

        }, 200); //quarter second to sample the loading from MaterializeCSS

    }


    //APPROACH: through a template that's programmatically added to 
    function displayPets(pets) {

        var pet_collection = document.getElementById("pet_collection");
        console.log(pets);
        
        //clone the template content, fill with values, add the remaining dynamic data, and append to parent:
        for (var i = 0; i < pets.length; i++) {

            var template_pet = document.getElementById("template_pet").content.cloneNode(true);

            //img element:
            var pet_image = template_pet.getElementById("pet_image");


            console.log("Images for item" + i + pets[i].Images.length);

            //check if there's an uploaded image for the pet
            var isAvailable = pets[i].Images.length > 0 && pets[i].Images[0].Image != "";
            //show default if none
            var image = isAvailable ?
                "data:image;base64," + pets[i].Images[0].Image :
                "../Content/images/clickme.png";

            pet_image.setAttribute("src", image);
            
            var petId = pets[i].Id;
            //visible name span:
            var span_name = template_pet.getElementById("span_name");
            span_name.textContent = pets[i].Name;


            //details indicator:
            var italicizedText = document.createElement("i");
            italicizedText.setAttribute("class", "materialize-icons right");
            italicizedText.textContent = "details";
            span_name.appendChild(italicizedText);

            //to be revealed:
            var span_name_reveal = template_pet.getElementById("span_name_reveal");
            span_name_reveal.textContent = pets[i].Name;
            var italicizedClose = italicizedText.cloneNode(true);
            italicizedClose.textContent = "close";
            span_name_reveal.appendChild(italicizedClose);

            //item general information
            var p_kind = template_pet.getElementById("p_kind");
            p_kind.setAttribute("id", "item" + petId + "_kind");
            p_kind.textContent = "Category: " + pets[i].Kind;
            var p_breed = template_pet.getElementById("p_breed");
            p_breed.setAttribute("id", "pet" + petId + "_breed");
            p_breed.textContent = "Brand: " + pets[i].Breed;

            //the "additional info" vector:
            var ul_constraints = template_pet.getElementById("ul_extra_infos");
            pets[i].Constraints.forEach((constraint) => {
                var listItem = document.createElement("li");
                listItem.setAttribute("class", "collection-item black-text");
                listItem.textContent = constraint.Description;
                ul_constraints.appendChild(listItem);
            });

           
            
            var divReveal = template_pet.getElementById("div_reveal");
            var divButton = document.createElement("div");
            divButton.setAttribute("class", "center");
            divButton.setAttribute("style", "margin-bottom:20px;");

            var btnAction = document.createElement("button");
            btnAction.setAttribute("class", "btn orange black-text darken-2");

            //index has this search div, manage user items view doesn't; to distinguish the 2 views
            if (document.getElementById("div_search") != null) {
                btnAction.textContent = "add to favorites";
            } else {
                btnAction.textContent = "Delete Item";
                btnAction.addEventListener("click", deletePet(pets[i].Id)); 
            }
           
            divButton.appendChild(btnAction);
            divReveal.appendChild(divButton);
            pet_collection.appendChild(template_pet);//the parent element in the document

        }

    }

    //another version of display: 
    function displayPetData(pets, parentDiv) {
        pets.forEach((item, index) => {
            var template = document.getElementById('template-pet');
            var templateNode = template.content.cloneNode(true);
            templateNode.querySelector('#txt-petname').textContent = item.Name;
            templateNode.querySelector('#txt-constraint').textContent = item.Consts;
            parentDiv.appendChild(templateNode);
        });
    }
    
    function deletePet(petId) {

        //experimenting inner functions for closure purposes
        return function () {
            
            var xhr = new XMLHttpRequest();
            var url = "/Pet/DeletePet";
            var requestData = { petId: petId };
            xhr.open("POST", url);
            xhr.setRequestHeader("Content-Type", "application/json");
            xhr.onload = function () {
                //console.log("Status: " + xhr.response);
                console.log("response: " + xhr.responseURL);
                //console.log("Status: " + xhr.responseText);
                
                //var description = petName + " was successfully deleted.";
                //window.location.replace("/Home/Pets/?description=" + description);

                
                window.location.replace(xhr.responseURL);
            };
            xhr.onerror = function () {
                console.log("Status: " + xhr.responseText);
            };
            console.log("RIGHT BEFORE SENDING, PET ID : " + requestData.petId);
            xhr.send(JSON.stringify(requestData));
        };

    }

})();



//=========Utility Methods:
function removeChildNodes(parentNode) {
    while (parentNode.firstChild) {
        parentNode.removeChild(parentNode.firstChild);
    }
}


function getResource(url) {
    //project generic resource GETTER
    console.log("Inside getResource, url = " + url);
    return new Promise((resolve, reject) => {
        var xhttp = new XMLHttpRequest();
        xhttp.open("GET", url, true);

        //event handlers
        xhttp.onload = function () {
            console.log("Success!!!");
            console.log(xhttp.response);
            var response = JSON.parse(JSON.parse(xhttp.response));
            //console.log("GetResource response = " + response);
            //console.log("StatusText = " + xhttp.statusText);
            resolve(response);
        };
        xhttp.onerror = function () {
            reject(xhttp.statusText);
        };
        xhttp.send();
    });
}



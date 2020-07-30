
class DivPet extends HTMLElement {

    constructor() {

        super();
        let template = document.getElementById("template-pet");
        let templateNode = template.content.cloneNode(true); //true means deep cloning where the element, its attributes, and children are copied
        const shadowRoot = this.attachShadow({ mode: 'open' });
        shadowRoot.appendChild(templateNode); 

    }

}
customElements.define("div-pet", DivPet);

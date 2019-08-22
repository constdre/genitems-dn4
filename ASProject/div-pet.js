import {html, PolymerElement} from '@polymer/polymer/polymer-element.js';

/**
 * `div-pet`
 * custom element that displays the information of the pets
 *
 * @customElement
 * @polymer
 * @demo demo/index.html
 */
class DivPet extends PolymerElement {
  static get template() {
    return html`
      <style>
        :host {
          display: block;
        }
      </style>
      <h2>Hello [[prop1]]!</h2>
    `;
  }
  static get properties() {
    return {
      prop1: {
        type: String,
        value: 'div-pet',
      },
    };
  }
}

window.customElements.define('div-pet', DivPet);

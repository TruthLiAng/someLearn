import {sayHello} from "./greet";

function showHello(divName:string, name:string) {
    const elt = document.getElementById(divName);
    elt.innerText = sayHello(name);
}

showHello("greeting", "ts1")
console.log(sayHello("TypeScript"));
import * as THREE from "three";
import { OrbitControls } from "OrbitControls";
import { GLTFLoader } from "GLTFLoader";
import { RectAreaLightHelper } from "RectAreaLightHelper";
import { RectAreaLightUniformsLib } from "RectAreaLightUniformsLib";

let container = document.querySelector(".container");

//Scene
const scene = new THREE.Scene();
scene.background = new THREE.Color("#000000");

//Camera
const camera = new THREE.PerspectiveCamera(
  40,
  window.innerWidth / window.innerHeight,
  0.1,
  3000
);
camera.rotation.set(84, 0, 50);
camera.position.set(4, 1.5, 4);

//render
const renderer = new THREE.WebGLRenderer({ antialias: true });
renderer.setSize(window.innerWidth, window.innerHeight);
container.appendChild(renderer.domElement);

//Light
const ambientLight = new THREE.AmbientLight("#FFFFFF");
scene.add(ambientLight);

onWindowResize();
renderer.render(scene, camera);
//Model loading
const loader = new GLTFLoader();

const model = (await loader.loadAsync("res/models/lenin/scene.gltf")).scene;
scene.add(model);
let pos = new THREE.Vector3(0, 1, 0);
camera.lookAt(pos);

//Resize
window.addEventListener("resize", onWindowResize, false);

function onWindowResize() {
  camera.aspect = window.innerWidth / window.innerHeight;
  camera.updateProjectionMatrix();

  renderer.setSize(window.innerWidth, window.innerHeight);
}

function animate() {
  model.rotation.y += 0.1;
  renderer.render(scene, camera);
}

renderer.setAnimationLoop(animate);

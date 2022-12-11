const btn = document.querySelector(".btn_search");
const form = document.querySelector(".search_form");
const closeBtn = document.querySelector(".btn_close");
const overlay = document.querySelector(".search_overlay");

btn.addEventListener("click", showForm);

function showForm() {
  form.classList.add("search_form_active");
  overlay.classList.add("overlay_active");

  closeBtn.addEventListener("click", closeForm);
  overlay.addEventListener("click", closeForm);

  btn.removeEventListener("click", showForm);
  btn.addEventListener("click", closeForm);
}

function closeForm() {
  form.classList.remove("search_form_active");
  overlay.classList.remove("overlay_active");

  closeBtn.removeEventListener("click", closeForm);
  overlay.removeEventListener("click", closeForm);

  btn.removeEventListener("click", closeForm);
  btn.addEventListener("click", showForm);
}

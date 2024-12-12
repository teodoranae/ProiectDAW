// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// // Funcția care verifică dacă un element este vizibil în fereastra de vizualizare
// function isInViewport(element) {
//   const rect = element.getBoundingClientRect();
//   return rect.top >= 0 && rect.left >= 0 && rect.bottom <= (window.innerHeight || document.documentElement.clientHeight);
// }

// // Funcția care activează animația pentru fiecare element din secțiunea features
// function checkFeatures() {
//   const features = document.querySelectorAll('.feature-item');
//   features.forEach(feature => {
//     if (isInViewport(feature)) {
//       feature.classList.add('visible'); // Activează animația
//     }
//   });
// }

// // Verifică la fiecare scroll
// window.addEventListener('scroll', checkFeatures);

// // Verifică imediat ce pagina este încărcată
// document.addEventListener('DOMContentLoaded', checkFeatures);
// script.js

// script.js

// Adaugă un eveniment pentru scroll pe fereastra browserului
window.addEventListener('scroll', () => {
    // Selectează toate elementele cu clasa .feature-item
    const features = document.querySelectorAll('.feature-item');

    // Obține înălțimea ferestrei de vizualizare
    const windowHeight = window.innerHeight;

    // Parcurge fiecare feature-item pentru a verifica dacă este vizibil
    features.forEach(feature => {
        // Obține poziția elementului față de fereastra de vizualizare
        const rect = feature.getBoundingClientRect();

        // Dacă partea superioară a elementului este suficient de aproape de fereastra vizibilă
        if (rect.top <= windowHeight - 100) {
            // Adaugă clasa "visible" pentru a activa animația de apariție
            feature.classList.add('visible');
        }
    });
});

// Asigură-te că elementele vor apărea imediat ce sunt vizibile pe ecran la încărcarea paginii
window.addEventListener('load', () => {
    const features = document.querySelectorAll('.feature-item');
    features.forEach(feature => {
        const rect = feature.getBoundingClientRect();
        const windowHeight = window.innerHeight;

        // Verifică dacă elementul este deja vizibil atunci când pagina se încarcă
        if (rect.top <= windowHeight - 100) {
            feature.classList.add('visible');
        }
    });
});



function toggleMenu() {
    var menu = document.getElementById("fav");
    menu.style.display = (menu.style.display === "block") ? "none" : "block";
}

document.addEventListener("click", function (event) {
    var menu = document.getElementById("fav");
    var opcionesBtn = document.querySelector(".fav");

    if (menu.style.display === "block" && event.target !== menu && event.target !== opcionesBtn) {
        menu.style.display = "none";
    }
});

document.addEventListener("DOMContentLoaded", () => {
    const favButtons = document.querySelectorAll(".fav-btn");

    favButtons.forEach(button => {
        button.addEventListener("click", () => {
            const noticia = {
                id: button.dataset.id,
                title: button.dataset.title,
                url: obtenerURL(button.dataset.id)
            };

            let favoritas = JSON.parse(localStorage.getItem("favoritas")) || [];

            if (!favoritas.some(n => n.id === noticia.id)) {
                favoritas.push(noticia);
                localStorage.setItem("favoritas", JSON.stringify(favoritas));
            }
        });
    });

    function obtenerURL(id) {
        const urls = {
            "1": "/Index1",
            "2": "/Noticia 2",
            "3": "/index3",
            "4": "/index2",
            "5": "/Index4",
            "6": "/Index5"
        };
        return urls[id] || "#";
    }
});

document.addEventListener("DOMContentLoaded", () => {
    const menuOpciones = document.getElementById("fav");
    const opcionesBtn = document.querySelector(".opciones");

    function mostrarFavoritas() {
        const favoritas = JSON.parse(localStorage.getItem("favoritas")) || [];
        menuOpciones.innerHTML = "";

        if (favoritas.length === 0) {
            menuOpciones.innerHTML = "<p>No hay noticias guardadas.</p>";
            return;
        }

        favoritas.forEach(noticia => {
            const noticiaWrapper = document.createElement("div");
            noticiaWrapper.classList.add("favorita-wrapper");

            const noticiaItem = document.createElement("a");
            noticiaItem.href = noticia.url;
            noticiaItem.textContent = `⭐ ${noticia.title}`;
            noticiaItem.classList.add("favorita-item");

            const deleteBtn = document.createElement("span");
            deleteBtn.textContent = "❌";
            deleteBtn.classList.add("delete-fav");
            deleteBtn.addEventListener("click", (e) => {
                e.preventDefault();
                e.stopPropagation();
                eliminarFavorita(noticia.id);
            });

            noticiaWrapper.appendChild(noticiaItem);
            noticiaWrapper.appendChild(deleteBtn);
            menuOpciones.appendChild(noticiaWrapper);
        });
    }

    function eliminarFavorita(id) {
        let favoritas = JSON.parse(localStorage.getItem("favoritas")) || [];
        favoritas = favoritas.filter(n => n.id !== id);
        localStorage.setItem("favoritas", JSON.stringify(favoritas));
        mostrarFavoritas();
    }

    opcionesBtn.addEventListener("click", (e) => {
        e.stopPropagation();
        mostrarFavoritas();
    });

    menuOpciones.addEventListener("click", (e) => {
        if (e.target.classList.contains("delete-fav")) {
            e.stopPropagation();
        }
    });
});
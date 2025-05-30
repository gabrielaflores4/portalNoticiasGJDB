document.addEventListener('DOMContentLoaded', function () {
    const formulario = document.getElementById('noticiaForm');
    if (formulario) {
        formulario.addEventListener('submit', function (event) {
            if (validarNoticia(event)) {
                guardarNoticia(event);
            }
        });
    }

    if (document.getElementById('noticias-dinamicas')) {
        cargarNoticias('noticias-dinamicas');
    }
});

function guardarNoticia(event) {
    event.preventDefault();

    const titulo = document.getElementById('titulo').value;
    const contenido = document.getElementById('contenido').value;
    const imagenInput = document.getElementById('imagen');
    const imagenArchivo = imagenInput.files[0];

    const lector = new FileReader();
    lector.onload = function (e) {
        const imagenBase64 = e.target.result;

        const noticia = {
            titulo: titulo,
            contenido: contenido,
            imagen: imagenBase64,
            fecha: new Date().toISOString()
        };

        let noticias = JSON.parse(localStorage.getItem('noticias')) || [];
        noticias.push(noticia);
        localStorage.setItem('noticias', JSON.stringify(noticias));

        alert('Noticia registrada exitosamente.');
        window.location.href = '/Index';
    };

    if (imagenArchivo) {
        lector.readAsDataURL(imagenArchivo);
    } else {
        const noticia = {
            titulo: titulo,
            contenido: contenido,
            imagen: null,
            fecha: new Date().toISOString()
        };

        let noticias = JSON.parse(localStorage.getItem('noticias')) || [];
        noticias.push(noticia);
        localStorage.setItem('noticias', JSON.stringify(noticias));

        alert('Noticia registrada exitosamente.');
        window.location.href = '/Index';
    }
}

function cargarNoticias(idContenedor) {
    const contenedor = document.getElementById(idContenedor);
    const noticias = JSON.parse(localStorage.getItem('noticias')) || [];

    noticias.forEach(noticia => {
        const card = document.createElement('div');
        card.className = 'card mb-3';

        const cardBody = document.createElement('div');
        cardBody.className = 'card-body';

        const titulo = document.createElement('h5');
        titulo.className = 'card-title';
        titulo.textContent = noticia.titulo;

        const contenido = document.createElement('p');
        contenido.className = 'card-text';
        contenido.textContent = noticia.contenido;

        if (noticia.imagen) {
            const imagen = document.createElement('img');
            imagen.src = noticia.imagen;
            imagen.className = 'img-fluid';
            imagen.alt = 'Imagen de la noticia';
            cardBody.appendChild(imagen);
        }

        const fecha = document.createElement('p');
        fecha.className = 'card-text';
        fecha.innerHTML = `<small class="text-muted">${new Date(noticia.fecha).toLocaleString()}</small>`;

        cardBody.appendChild(titulo);
        cardBody.appendChild(contenido);
        cardBody.appendChild(fecha);
        card.appendChild(cardBody);
        contenedor.appendChild(card);
    });
}
